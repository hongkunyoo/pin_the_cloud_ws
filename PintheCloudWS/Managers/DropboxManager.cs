using PintheCloudWS.Converters;
using PintheCloudWS.Helpers;
using PintheCloudWS.Models;
using PintheCloudWS.Pages;
using PintheCloudWS.Models;
using PintheCloudWS.Utilities;
using PintheCloudWS.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using PintheCloudWS.Locale;
using Windows.Storage.Streams;
using PintheCloudWS.Exceptions;

namespace PintheCloudWS.Managers
{
    public class DropboxManager : IStorageManager
    {
        #region Variables
        private const string DROPBOX_CLIENT_KEY = "gxjfureco8noyle";
        private const string DROPBOX_CLIENT_SECRET = "iskekcebjky6vbm";
        public const string DROPBOX_AUTH_URI = "http://www.pinthecloud.com/index.html";

        private const string DROPBOX_USER_KEY = "DROPBOX_USER_KEY";
        private const string DROPBOX_SIGN_IN_KEY = "DROPBOX_SIGN_IN_KEY";

        private const string DROPBOX_IMAGE_URI = "/Assets/pajeon/pin/png/ico_dropbox.png";
        private const string DROPBOX_COLOR_HEX_STRING = "26A4DD";

        //private DropNetClient _client = null;
        private StorageAccount CurrentAccount = null;
        private TaskCompletionSource<bool> tcs = null;
        #endregion


        private async Task<StorageAccount> GetMyAccountAsync()
        {
            //TaskCompletionSource<StorageAccount> tcs = new TaskCompletionSource<StorageAccount>();
            //AccountInfo info = await this._client.AccountInfo();
            //StorageAccount account = new StorageAccount(info.Uid.ToString(), StorageAccount.StorageAccountType.DROPBOX, info.DisplayName, 0.0);
            //return account;
            return null;
            //this._client.AccountInfoAsync((info) =>
            //{
            //    tcs.SetResult(new StorageAccount(info.uid.ToString(), StorageAccount.StorageAccountType.DROPBOX, info.display_name, 0.0));
            //}, (fail) =>
            //{
            //    tcs.SetException(new Exception("Account Info Get Failed"));
            //});
        }


        public async Task<bool> SignIn()
        {
            //// Get dropbox _client.
            //this.tcs = new TaskCompletionSource<bool>();
            //this._client = new DropNetClient(DROPBOX_CLIENT_KEY, DROPBOX_CLIENT_SECRET);

            //// If dropbox user exists, get it.
            //// Otherwise, get from user.
            //UserLogin dropboxUser = null;
            //if (App.ApplicationSettings.Contains(DROPBOX_USER_KEY))
            //{
            //    //dropboxUser = (UserLogin)App.ApplicationSettings[DROPBOX_USER_KEY];
            //    dropboxUser = RestoreDropBoxUserKey();
            //}
                
            //if (dropboxUser != null)
            //{
            //    this._client.SetUserToken(dropboxUser);
            //    //this._client.UserLogin = dropboxUser;
            //    //this.CurrentAccount = await this.GetMyAccountAsync();
            //    await this.GetMyAccountAsync();
            //}
            //else
            //{
            //    UserLogin userLogin = await this._client.GetRequestToken();
            //    string authUri = this._client.BuildAuthorizeUrl(userLogin, DROPBOX_AUTH_URI);
            //    DropboxWebBrowserTask webBrowser = new DropboxWebBrowserTask(authUri);
            //    await webBrowser.ShowAsync();

            //    UserLogin accesToken = await this._client.GetAccessToken();
            //    this._client.UserLogin = accesToken;
            //    this.CurrentAccount = await this.GetMyAccountAsync();
            //    StorageAccount account = await App.AccountManager.GetStorageAccountAsync(this.CurrentAccount.Id);
            //    if (account == null)
            //    {
            //        await App.AccountManager.CreateStorageAccountAsync(this.CurrentAccount);
            //    }

            //    App.ApplicationSettings[DROPBOX_SIGN_IN_KEY] = true;
            //    SaveToDropBoxUserKey(accesToken);
            //    //App.ApplicationSettings[DROPBOX_USER_KEY] = accesToken;
            //    App.ApplicationSettings.Save();

            //   // this._client.GetTokenAsync(async (userLogin) =>
            //   // {
            //   //     string authUri = this._client.BuildAuthorizeUrl(DROPBOX_AUTH_URI);
            //   //     DropboxWebBrowserTask webBrowser = new DropboxWebBrowserTask(authUri);
            //   //     await webBrowser.ShowAsync();

            //   //     this._client.GetAccessTokenAsync(async (accessToken) =>
            //   //     {
            //   //         UserLogin user = new UserLogin();
            //   //         user.Token = accessToken.Token;
            //   //         user.Secret = accessToken.Secret;
            //   //         this._client.UserLogin = user;

            //   //         // Save dropbox user got and sign in setting.
            //   //         this.CurrentAccount = await this.GetMyAccountAsync();
            //   //         StorageAccount account = await App.AccountManager.GetStorageAccountAsync(this.CurrentAccount.Id);
            //   //         if (account == null)
            //   //         {
            //   //             await App.AccountManager.CreateStorageAccountAsync(this.CurrentAccount);
            //   //         }

            //   //         App.ApplicationSettings[DROPBOX_SIGN_IN_KEY] = true;
            //   //         App.ApplicationSettings[DROPBOX_USER_KEY] = user;
            //   //         App.ApplicationSettings.Save();
            //   //         TaskHelper.AddTask(PtcPage.STORAGE_EXPLORER_SYNC + this.GetStorageName(), StorageExplorer.Synchronize(this.GetStorageName()));
            //   //         tcs.SetResult(true);
            //   //     },
            //   //     (error) =>
            //   //     {
            //   //         Debug.WriteLine(error.ToString());
            //   //         tcs.SetResult(false);
            //   //     });
            //   // },
            //   //(error) =>
            //   //{
            //   //    //var keys = error.Data.Keys;
            //   //    //for (var i = 0; i < keys.Count; i++ )
            //   //    //{
            //   //    //    Debug.WriteLine(error.Data[i]);
            //   //    //}
            //   //    //Debug.WriteLine(error.Message);
            //   //    //Debug.WriteLine(error.StackTrace);
            //   //    tcs.SetResult(false);
            //   //});
            //}
            //TaskHelper.AddTask(StorageExplorer.STORAGE_EXPLORER_SYNC + this.GetStorageName(), StorageExplorer.Synchronize(this.GetStorageName()));
            return true;
        }


        public bool IsSigningIn()
        {
            if (this.tcs != null)
                return !this.tcs.Task.IsCompleted && !App.ApplicationSettings.Contains(DROPBOX_SIGN_IN_KEY);
            else
                return false;
        }


        public bool IsPopup()
        {
            return true;
        }


        // Remove user and record
        public void SignOut()
        {
            App.ApplicationSettings.Remove(DROPBOX_USER_KEY);
            App.ApplicationSettings.Remove(DROPBOX_SIGN_IN_KEY);
            StorageExplorer.RemoveKey(this.GetStorageName());
            //this._client = null;
            this.CurrentAccount = null;
        }


        public bool IsSignIn()
        {
            return App.ApplicationSettings.Contains(DROPBOX_SIGN_IN_KEY);
        }


        public string GetStorageName()
        {
            return AppResources.Dropbox;
        }


        public string GetStorageImageUri()
        {
            return DROPBOX_IMAGE_URI;
        }


        public string GetStorageColorHexString()
        {
            return DROPBOX_COLOR_HEX_STRING;
        }


        //public Stack<FileObjectViewItem> GetFolderRootTree()
        //{
        //    return this.FolderRootTree;
        //}


        //public Stack<List<FileObject>> GetFoldersTree()
        //{
        //    return this.FoldersTree;
        //}


        public async Task<StorageAccount> GetStorageAccountAsync()
        {
            TaskCompletionSource<StorageAccount> tcs = new TaskCompletionSource<StorageAccount>();
            if (this.CurrentAccount == null)
            {
                await TaskHelper.WaitSignInTask(this.GetStorageName());
            }
            tcs.SetResult(this.CurrentAccount);
            return tcs.Task.Result;
        }


        public Task<FileObject> GetRootFolderAsync()
        {
            return this.GetFileAsync("/");
        }


        public Task<List<FileObject>> GetRootFilesAsync()
        {
            return this.GetFilesFromFolderAsync("/");
        }


        public async Task<FileObject> GetFileAsync(string fileId)
        {
            ////MetaData metaTask = await this._client.GetMetaDataTask(fileId);
            //MetaData metaTask = await this._client.GetMetaData(fileId);
            //return ConvertToFileObjectHelper.ConvertToFileObject(metaTask);
            return null;
        }


        public async Task<List<FileObject>> GetFilesFromFolderAsync(string folderId)
        {
            ////MetaData metaTask = await this._client.GetMetaDataTask(folderId);
            //MetaData metaTask = await this._client.GetMetaData(folderId);
            //List<FileObject> list = new List<FileObject>();
            //if (metaTask.Contents == null) return list;
            //foreach (MetaData m in metaTask.Contents)
            //    list.Add(ConvertToFileObjectHelper.ConvertToFileObject(m));
            //return list;
            return null;
        }


        public async Task<Stream> DownloadFileStreamAsync(string sourceFileId)
        {
            //TaskCompletionSource<Stream> tcs = new TaskCompletionSource<Stream>();
            //MemoryStream ms = new MemoryStream(await this._client.GetFile(sourceFileId));
            ////this._client.GetFileAsync(sourceFileId, new Action<IRestResponse>((response) =>
            ////{
            ////    MemoryStream stream = new MemoryStream(response.RawBytes);
            ////    tcs.SetResult(stream);
            ////}), new Action<DropNet.Exceptions.DropboxException>((ex) =>
            ////{
            ////    tcs.SetException(new Exception("failed"));
            ////    throw new ShareException(sourceFileId, ShareException.ShareType.DOWNLOAD);
            ////}));
            //return ms;
            return null;
        }


        public async Task<bool> UploadFileStreamAsync(string folderIdToStore, string fileName, Stream outstream)
        {
            //try
            //{
            //    MemoryStream ms = new MemoryStream();
            //    byte[] buffer = new byte[102400];
            //    int count = 0;
            //    while((count = outstream.Read(buffer,0,buffer.Length)) != 0)
            //    {
            //        ms.Write(buffer,0,count);
            //    }
            //    //MetaData d = await _client.UploadFileTask(folderIdToStore, fileName, CreateStream(outstream).ToArray());
            //    MetaData meta = await this._client.Upload(folderIdToStore, fileName, ms.ToArray());
            //    //MetaData d = await this._client.UploadFileTask(folderIdToStore, fileName, outstream);
            return true;
            //}
            //catch
            //{
            //    throw new ShareException(fileName, ShareException.ShareType.UPLOAD);
            //}
        }

        public async Task<FileObject> Synchronize()
        {
            FileObject fileObject = await GetRootFolderAsync();
            fileObject.FileList = await _GetChildAsync(fileObject);
            return fileObject;
        }

        #region Private Methods
        private async Task<List<FileObject>> _GetChildAsync(FileObject fileObject)
        {
            if (FileObjectViewModel.FOLDER.Equals(fileObject.Type.ToString()))
            {
                List<FileObject> list = await this.GetFilesFromFolderAsync(fileObject.Id);
                foreach (FileObject file in list)
                {
                    file.FileList = await _GetChildAsync(file);
                }
                return list;
            }
            else
            {
                return null;
            }
        }


        private bool _Streaming(Stream input, Stream output)
        {
            byte[] buffer = new byte[1024000];
            int count = 0;
            try
            {
                while ((count = input.Read(buffer, 0, buffer.Length)) != 0)
                {
                    output.Write(buffer, 0, count);
                }
                input.Dispose();
                output.Dispose();
                return true;
            }
            catch
            {
                throw new Exception("Create Streaming Failed");
            }
        }


        //private string key1 = "key1_fordropbox";
        //private string key2 = "key2_fordropbox";
        //private void SaveToDropBoxUserKey(UserLogin userlogin)
        //{
        //    App.ApplicationSettings[key1] = userlogin.Token;
        //    App.ApplicationSettings[key2] = userlogin.Secret;
        //}

        //private UserLogin RestoreDropBoxUserKey()
        //{
        //    UserLogin user = new UserLogin();
        //    user.Token = (string)App.ApplicationSettings[key1];
        //    user.Secret = (string)App.ApplicationSettings[key2];
        //    return user;
        //}
        #endregion
    }
}
