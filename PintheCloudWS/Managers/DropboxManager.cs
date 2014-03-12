using DropNetRT;
using DropNetRT.Models;
using PintheCloudWS.Helpers;
using PintheCloudWS.Locale;
using PintheCloudWS.Models;
using PintheCloudWS.Utilities;
using PintheCloudWS.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PintheCloudWS.Managers
{
    public class DropboxManager : IStorageManager
    {
        #region Variables
        private const string DROPBOX_CLIENT_KEY = "gxjfureco8noyle";
        private const string DROPBOX_CLIENT_SECRET = "iskekcebjky6vbm";
        public const string DROPBOX_AUTH_URI = "http://54.214.19.198";

        private const string DROPBOX_USER_KEY = "DROPBOX_USER_KEY";
        private const string DROPBOX_SIGN_IN_KEY = "DROPBOX_SIGN_IN_KEY";

        private const string DROPBOX_IMAGE_URI = "/Assets/pajeon/at_here/png/navi_ico_dropbox.png";
        private const string DROPBOX_COLOR_HEX_STRING = "26A4DD";

        private Stack<List<FileObject>> FoldersTree = new Stack<List<FileObject>>();
        private Stack<FileObjectViewItem> FolderRootTree = new Stack<FileObjectViewItem>();

        private DropNetClient _client = null;
        private Account CurrentAccount = null;
        private TaskCompletionSource<bool> tcs = null;
        #endregion


        private async Task<Account> GetMyAccountAsync()
        {
            TaskCompletionSource<Account> tcs = new TaskCompletionSource<Account>();
            
            // Windows Phone 8
            //this._client.AccountInfoAsync((info) =>
            //{
            //    tcs.SetResult(new Account(info.uid.ToString(), Account.StorageAccountType.DROPBOX, info.display_name, 0.0, AccountType.NORMAL_ACCOUNT_TYPE));
            //}, (fail) =>
            //{
            //    tcs.SetException(new Exception("Account Info Get Failed"));
            //});
            AccountInfo info = await this._client.AccountInfo();

            // Windows Phone 8
            //return tcs.Task;
            return new Account(info.Uid.ToString(), Account.StorageAccountType.DROPBOX, info.DisplayName, 0.0, AccountType.NORMAL_ACCOUNT_TYPE);
        }


        public async Task<bool> SignIn()
        {
            // Get dropbox _client.
            this.tcs = new TaskCompletionSource<bool>();
            this._client = new DropNetClient(DROPBOX_CLIENT_KEY, DROPBOX_CLIENT_SECRET);

            // If dropbox user exists, get it.
            // Otherwise, get from user.
            UserLogin dropboxUser = null;
            if (App.ApplicationSettings.Values.ContainsKey(DROPBOX_USER_KEY))
                dropboxUser = (UserLogin)App.ApplicationSettings.Values[DROPBOX_USER_KEY];
            if (dropboxUser != null)
            {
                this._client.UserLogin = dropboxUser;
                //this.CurrentAccount = await this.GetMyAccountAsync();
                // Windows Phone 8
                //this._client.AccountInfoAsync((info) =>
                //{
                //    //tcs.SetResult(new Account(info.uid.ToString(), Account.StorageAccountType.DROPBOX, info.display_name, 0.0, AccountType.NORMAL_ACCOUNT_TYPE));
                //    this.CurrentAccount = new Account(info.uid.ToString(), Account.StorageAccountType.DROPBOX, info.display_name, 0.0, AccountType.NORMAL_ACCOUNT_TYPE);
                //    tcs.SetResult(true);
                //}, (fail) =>
                //{
                //    //tcs.SetException(new Exception("Account Info Get Failed"));
                //    this.CurrentAccount = null;
                //    tcs.SetResult(false);
                //});
                AccountInfo info = await this._client.AccountInfo();
                this.CurrentAccount = new Account(info.Uid.ToString(), Account.StorageAccountType.DROPBOX, info.DisplayName, 0.0, AccountType.NORMAL_ACCOUNT_TYPE);
            }
            else
            {
                // Windows Phone 8
               // _client.GetTokenAsync(async (userLogin) =>
               // {
               //     string authUri = _client.BuildAuthorizeUrl(DROPBOX_AUTH_URI);
               //     DropboxWebBrowserTask webBrowser = new DropboxWebBrowserTask(authUri);
               //     await webBrowser.ShowAsync();

               //     _client.GetAccessTokenAsync(async (accessToken) =>
               //     {
               //         UserLogin user = new UserLogin();
               //         user.Token = accessToken.Token;
               //         user.Secret = accessToken.Secret;
               //         _client.UserLogin = user;

               //         // Save dropbox user got and sign in setting.
               //         this.CurrentAccount = await this.GetMyAccountAsync();
               //         App.ApplicationSettings.Values[DROPBOX_SIGN_IN_KEY] = true;
               //         App.ApplicationSettings.Values[DROPBOX_USER_KEY] = user;
               //         tcs.SetResult(true);
               //     },
               //     (error) =>
               //     {
               //         tcs.SetResult(false);
               //     });
               // },
               //(error) =>
               //{
               //    tcs.SetResult(false);
               //});
                UserLogin userLogin = await this._client.GetRequestToken();
                userLogin = await this._client.GetAccessToken();
                this._client.UserLogin = userLogin;

                this.CurrentAccount = await this.GetMyAccountAsync();
                App.ApplicationSettings.Values[DROPBOX_SIGN_IN_KEY] = true;
                App.ApplicationSettings.Values[DROPBOX_USER_KEY] = userLogin;
            }
            // Windows Phone 8
            //return tcs.Task;
            return true;
        }


        public bool IsSigningIn()
        {
            if (this.tcs != null)
                return !this.tcs.Task.IsCompleted && !App.ApplicationSettings.Values.ContainsKey(DROPBOX_SIGN_IN_KEY);
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
            App.ApplicationSettings.Values.Remove(DROPBOX_USER_KEY);
            App.ApplicationSettings.Values.Remove(DROPBOX_SIGN_IN_KEY);
            this.FoldersTree.Clear();
            this.FolderRootTree.Clear();
            this._client = null;
            this.CurrentAccount = null;
        }


        public bool IsSignIn()
        {
            return App.ApplicationSettings.Values.ContainsKey(DROPBOX_SIGN_IN_KEY);
        }


        public string GetStorageName()
        {
            return App.ResourceLoader.GetString(ResourcesKeys.Dropbox);
        }


        public string GetStorageImageUri()
        {
            return DROPBOX_IMAGE_URI;
        }


        public string GetStorageColorHexString()
        {
            return DROPBOX_COLOR_HEX_STRING;
        }


        public Stack<FileObjectViewItem> GetFolderRootTree()
        {
            return this.FolderRootTree;
        }


        public Stack<List<FileObject>> GetFoldersTree()
        {
            return this.FoldersTree;
        }


        public Account GetAccount()
        {
            return this.CurrentAccount;
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
            // Windows Phone 8
            //MetaData metaTask = await this._client.GetMetaDataTask(fileId);
            MetaData metaTask = await this._client.GetMetaData(fileId);
            return ConvertToFileObjectHelper.ConvertToFileObject(metaTask);
        }


        public async Task<List<FileObject>> GetFilesFromFolderAsync(string folderId)
        {
            // Windows Phone 8
            //MetaData metaTask = await this._client.GetMetaDataTask(folderId);
            MetaData metaTask = await this._client.GetMetaData(folderId);
            List<FileObject> list = new List<FileObject>();

            if (metaTask.Contents == null) return list;

            foreach (MetaData m in metaTask.Contents)
            {
                list.Add(ConvertToFileObjectHelper.ConvertToFileObject(m));
            }

            return list;
        }


        // Windows Phone 8
        public async Task<Stream> DownloadFileStreamAsync(string sourceFileId)
        {
            TaskCompletionSource<Stream> tcs = new TaskCompletionSource<Stream>();

            // Windows Phone 8
            //this._client.GetFileAsync(sourceFileId, new Action<IRestResponse>((response) =>
            //{
            //    MemoryStream stream = new MemoryStream(response.RawBytes);
            //    tcs.SetResult(stream);
            //}), new Action<DropNet.Exceptions.DropboxException>((ex) =>
            //{
            //    tcs.SetException(new Exception("failed"));
            //    throw new ShareException(sourceFileId, ShareException.ShareType.DOWNLOAD);
            //}));
            //return tcs.Task;
            return new MemoryStream(await this._client.GetFile(sourceFileId));
        }


        public async Task<bool> UploadFileStreamAsync(string folderIdToStore, string fileName, Stream outstream)
        {
            try
            {
                //MetaData d = await _client.UploadFileTask(folderIdToStore, fileName, CreateStream(outstream).ToArray());

                // Windows Phone 8
                //MetaData d = await this._client.UploadFileTask(folderIdToStore, fileName, outstream);
                MetaData d = await this._client.Upload(folderIdToStore, fileName, outstream);
                return true;
            }
            catch
            {
                throw new ShareException(fileName, ShareException.ShareType.UPLOAD);
            }
        }

        #region Private Methods
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

                // Windows Phone 8
                //input.Close();
                //output.Close();
                return true;
            }
            catch
            {
                throw new Exception("Create Streaming Failed");
            }
        }
        #endregion
    }
}
