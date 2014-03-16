using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Services;
using Newtonsoft.Json;
using PintheCloudWS.Helpers;
using PintheCloudWS.Locale;
using PintheCloudWS.Models;
using PintheCloudWS.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace PintheCloudWS.Managers
{
    public class GoogleDriveManager : IStorageManager
    {
        #region Variables
        private const string GOOGLE_DRIVE_CLIENT_ID = "109786198225-m8fihmv82b2fmf5k4d69u9039ebn68fn.apps.googleusercontent.com";
        private const string GOOGLE_DRIVE_CLIENT_SECRET = "Tk8M01zlkBRlIsv-1fa9BKiS";

        private const string GOOGLE_DRIVE_USER_KEY = "GOOGLE_DRIVE_USER_KEY";
        private const string GOOGLE_DRIVE_SIGN_IN_KEY = "GOOGLE_DRIVE_SIGN_IN_KEY";

        private const string GOOGLE_DRIVE_IMAGE_URI = "/Assets/pajeon/at_here/png/navi_ico_googledrive.png";
        private const string GOOGLE_DRIVE_COLOR_HEX_STRING = "F1AE1D";

        private Stack<List<FileObject>> FoldersTree = new Stack<List<FileObject>>();
        private Stack<FileObjectViewItem> FolderRootTree = new Stack<FileObjectViewItem>();

        public static Dictionary<string, string> GoogleDocMapper;
        public static Dictionary<string, string> MimeTypeMapper;
        public static Dictionary<string, string> ExtensionMapper;

        private DriveService service;
        private UserCredential credential;
        private StorageAccount CurrentAccount;
        private User user;
        private string rootFodlerId = "";
        private TaskCompletionSource<bool> tcs = null;
        #endregion

        public GoogleDriveManager()
        {
            // Converting strings from google-docs to office files
            GoogleDriveManager.GoogleDocMapper = new Dictionary<string, string>();
            GoogleDriveManager.ExtensionMapper = new Dictionary<string, string>();

            // Document file
            GoogleDriveManager.GoogleDocMapper.Add("application/vnd.google-apps.document", "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
            // SpreadSheet file
            GoogleDriveManager.GoogleDocMapper.Add("application/vnd.google-apps.spreadsheet", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            // Image file
            GoogleDriveManager.GoogleDocMapper.Add("application/vnd.google-apps.drawing", "image/png");
            // Presentation file
            GoogleDriveManager.GoogleDocMapper.Add("application/vnd.google-apps.presentation", "application/vnd.openxmlformats-officedocument.presentationml.presentation");
            // Not using
            //GoogleDocMapper.Add("application/vnd.google-apps.form", "Not Supported");
            //GoogleDocMapper.Add("application/vnd.google-apps.folder", "Folder");

            GoogleDriveManager.ExtensionMapper.Add("application/vnd.google-apps.document", "doc");
            GoogleDriveManager.ExtensionMapper.Add("application/vnd.google-apps.spreadsheet", "xls");
            GoogleDriveManager.ExtensionMapper.Add("application/vnd.google-apps.drawing", "png");
            GoogleDriveManager.ExtensionMapper.Add("application/vnd.google-apps.presentation", "ppt");
            //GoogleDriveManager.ExtensionMapper.Add("application/vnd.google-apps.formr", "");

            Task setMimeTypeMapperTask = this.SetMimeTypeMapper();
        }


        public async Task<bool> SignIn()
        {
            this.tcs = new TaskCompletionSource<bool>();
            // Add application settings before work for good UX
            try
            {
                //credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                //    new ClientSecrets
                //    {
                //        ClientId = GOOGLE_DRIVE_CLIENT_ID,
                //        ClientSecret = GOOGLE_DRIVE_CLIENT_SECRET
                //    },
                //    new[] { DriveService.Scope.Drive },
                //    this._GetUserSession(),
                //    CancellationToken.None
                //);

                this.service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "athere",
                });
                AboutResource aboutResource = service.About;
                About about = await aboutResource.Get().ExecuteAsync();
                this.user = about.User;

                string name = this.user.DisplayName;
                string id = about.PermissionId;

                // Register account
                StorageAccount account = App.AccountManager.GetPtcAccount().GetStorageAccountById(id);
                if (account == null)
                {
                    account = new StorageAccount(id, StorageAccount.StorageAccountType.GOOGLE_DRIVE, name, 0.0);
                    await App.AccountManager.GetPtcAccount().CreateStorageAccountAsync(account);
                }
                this.CurrentAccount = account;

                // Save sign in setting.
                App.ApplicationSettings.Values[GOOGLE_DRIVE_SIGN_IN_KEY] = true;
                tcs.SetResult(true);
            }
            //catch (Microsoft.Phone.Controls.WebBrowserNavigationException ex)
            //{
            //    Debug.WriteLine(ex.ToString());
            //    tcs.SetResult(false);
            //}
            catch (Google.GoogleApiException e)
            {
                Debug.WriteLine(e.ToString());
                tcs.SetResult(false);
            }
            catch (System.Threading.Tasks.TaskCanceledException)
            {
                tcs.SetResult(false);
            }
            return tcs.Task.Result;
        }


        public bool IsSigningIn()
        {
            if (this.tcs != null)
                return !this.tcs.Task.IsCompleted && !App.ApplicationSettings.Values.ContainsKey(GOOGLE_DRIVE_SIGN_IN_KEY);
            else
                return false;

        }


        // Remove user and record
        public void SignOut()
        {
            App.ApplicationSettings.Values.Remove(GOOGLE_DRIVE_USER_KEY);
            App.ApplicationSettings.Values.Remove(GOOGLE_DRIVE_SIGN_IN_KEY);
            this.FoldersTree.Clear();
            this.FolderRootTree.Clear();
            this.CurrentAccount = null;
        }


        public bool IsPopup()
        {
            return false;
        }


        public bool IsSignIn()
        {
            return App.ApplicationSettings.Values.ContainsKey(GOOGLE_DRIVE_SIGN_IN_KEY);
        }


        public string GetStorageName()
        {
            return App.ResourceLoader.GetString(ResourcesKeys.GoogleDrive);
        }


        public string GetStorageImageUri()
        {
            return GOOGLE_DRIVE_IMAGE_URI;
        }


        public string GetStorageColorHexString()
        {
            return GOOGLE_DRIVE_COLOR_HEX_STRING;
        }


        public Stack<FileObjectViewItem> GetFolderRootTree()
        {
            return this.FolderRootTree;
        }


        public Stack<List<FileObject>> GetFoldersTree()
        {
            return this.FoldersTree;
        }


        public StorageAccount GetStorageAccount()
        {
            return this.CurrentAccount;
        }


        public async Task<FileObject> GetRootFolderAsync()
        {
            FileObject rootFile = new FileObject();
            AboutResource aboutResource = service.About;
            About about = await aboutResource.Get().ExecuteAsync();
            rootFile.Id = about.RootFolderId;
            this.rootFodlerId = about.RootFolderId;
            rootFile.Name = "/";
            return rootFile;
        }


        public async Task<List<FileObject>> GetRootFilesAsync()
        {
            FileList fileList = await this.service.Files.List().ExecuteAsync();
            List<FileObject> childList = new List<FileObject>();
            foreach (Google.Apis.Drive.v2.Data.File file in fileList.Items)
            {
                Debug.WriteLine(file.Title);
                if (this._IsRoot(file) && this._IsValidFile(file))
                {
                    childList.Add(ConvertToFileObjectHelper.ConvertToFileObject(file));
                }
            }
            return childList;
        }


        public async Task<FileObject> GetFileAsync(string fileId)
        {
            Google.Apis.Drive.v2.Data.File file = await service.Files.Get(fileId).ExecuteAsync();
            if (this._IsValidFile(file))
            {
                return ConvertToFileObjectHelper.ConvertToFileObject(file);
            }
            return null;
        }


        public async Task<List<FileObject>> GetFilesFromFolderAsync(string folderId)
        {
            //if (this.rootFodlerId.Equals(folderId))
            //{
            //    return await GetRootFilesAsync();
            //}
            List<FileObject> list = new List<FileObject>();
            ChildList childList = await service.Children.List(folderId).ExecuteAsync();
            foreach (ChildReference child in childList.Items)
            {
                list.Add(await this.GetFileAsync(child.Id));
            }
            list.RemoveAll(item => item == null);
            return list;
        }


        public async Task<Stream> DownloadFileStreamAsync(string fileId)
        {
            byte[] inarray = await service.HttpClient.GetByteArrayAsync(fileId);
            return new MemoryStream(inarray);
        }


        public async Task<bool> UploadFileStreamAsync(string folderId, string fileName, Stream inputStream)
        {
            try
            {
                Google.Apis.Drive.v2.Data.File file = new Google.Apis.Drive.v2.Data.File();
                file.Title = fileName;

                ParentReference p = new ParentReference();
                p.Id = folderId;
                file.Parents = new List<ParentReference>();
                file.Parents.Add(p);

                string extension = fileName.Split('.').Last();
                var insert = service.Files.Insert(file, inputStream, GoogleDriveManager.MimeTypeMapper[extension]);
                var task = await insert.UploadAsync();
            }
            catch
            {
                return false;
            }
            return true;
        }


        #region Private Methods
        private async Task SetMimeTypeMapper()
        {
            StorageFile js = await (await Package.Current.InstalledLocation.GetFolderAsync("Assets")).GetFileAsync("mimeType.js");
            JsonTextReader jtr = new JsonTextReader(new StreamReader(await js.OpenStreamForReadAsync()));
            Newtonsoft.Json.JsonSerializer s = new JsonSerializer();
            GoogleDriveManager.MimeTypeMapper = s.Deserialize<Dictionary<string, string>>(jtr);
        }


        private string _GetUserSession()
        {
            if (App.ApplicationSettings.Values.ContainsKey(GOOGLE_DRIVE_USER_KEY))
            {
                return (string)App.ApplicationSettings.Values[GOOGLE_DRIVE_USER_KEY];
            }
            else
            {
                var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                var random = new Random(DateTime.Now.Millisecond);
                var result = new string(
                    Enumerable.Repeat(chars, 8)
                              .Select(s => s[random.Next(s.Length)])
                              .ToArray()
                );
                App.ApplicationSettings.Values[GOOGLE_DRIVE_USER_KEY] = result;
                return result;
            }
        }


        private bool _IsValidFile(Google.Apis.Drive.v2.Data.File file)
        {
            return (this._IsMine(file) && !this._IsTrashed(file) && !"application/vnd.google-apps.form".Equals(file.MimeType));
        }


        private bool _IsRoot(Google.Apis.Drive.v2.Data.File file)
        {
            bool result = true;
            IList<ParentReference> parents = file.Parents;
            if (parents == null) return false;
            foreach (ParentReference parent in parents)
            {
                result &= parent.IsRoot.Value;
            }
            return result;
        }


        private bool _IsMine(Google.Apis.Drive.v2.Data.File file)
        {
            bool result = true;
            IList<User> owners = file.Owners;

            foreach (User user in owners)
            {
                // TODO Get that values from converted account.
                result &= ((this.user.DisplayName.Equals(user.DisplayName)) && user.IsAuthenticatedUser.Value);
            }
            return result;
        }


        private bool _IsTrashed(Google.Apis.Drive.v2.Data.File file)
        {
            if (file.ExplicitlyTrashed == null) return false;
            return file.ExplicitlyTrashed.Value;
        }


        private bool _IsGoolgeDoc(Google.Apis.Drive.v2.Data.File file)
        {
            if (file.MimeType.Contains("application/vnd.google-apps")) return true;
            return false;
        }


        private bool _IsAbleToDownload(Google.Apis.Drive.v2.Data.File file)
        {
            return !string.Empty.Equals(file.DownloadUrl);
        }
        #endregion
    }
}
