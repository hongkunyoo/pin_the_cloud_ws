using Microsoft.Live;
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
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;

namespace PintheCloudWS.Managers
{
    public class OneDriveManager : IStorageManager
    {
        #region Variables
        // Summary:
        //     Object to communicate with OneDrive.
        private const string LIVE_CLIENT_ID = "0000000044110129";
        private const string ONE_DRIVE_SIGN_IN_KEY = "ONE_DRIVE_SIGN_IN_KEY";

        private const string ONE_DRIVE_IMAGE_URI = "/Assets/pajeon/at_here/png/navi_ico_skydrive.png";
        private const string ONE_DRIVE_COLOR_HEX_STRING = "2458A7";

        private Stack<List<FileObject>> FoldersTree = new Stack<List<FileObject>>();
        private Stack<FileObjectViewItem> FolderRootTree = new Stack<FileObjectViewItem>();

        private LiveConnectClient LiveClient = null;
        private StorageAccount CurrentAccount = null;
        private TaskCompletionSource<bool> tcs = null;
        #endregion


        public async Task<bool> SignIn()
        {
            this.tcs = new TaskCompletionSource<bool>();

            // If it haven't registerd live client, register
            LiveConnectClient liveClient = await this.GetLiveConnectClientAsync();
            if (liveClient != null)
            {
                this.LiveClient = liveClient;

                // Get id and name.
                LiveOperationResult operationResult = await this.LiveClient.GetAsync("me");
                string accountId = (string)operationResult.Result["id"];
                string accountUserName = (string)operationResult.Result["name"];

                // Register account
                StorageAccount storageAccount = App.AccountManager.GetPtcAccount().GetStorageAccountById(accountId);
                if (storageAccount == null)
                {
                    storageAccount = new StorageAccount();
                    storageAccount.Id = accountId;
                    storageAccount.StorageName = this.GetStorageName();
                    storageAccount.UserName = accountUserName;
                    storageAccount.UsedSize = 0.0;
                    await App.AccountManager.GetPtcAccount().CreateStorageAccountAsync(storageAccount);
                }

                this.CurrentAccount = storageAccount;

                // Save sign in setting.
                App.ApplicationSettings.Values[ONE_DRIVE_SIGN_IN_KEY] = true;
                tcs.SetResult(true);
            }
            else
            {
                tcs.SetResult(false);
            }
            return tcs.Task.Result;
        }


        public bool IsSigningIn()
        {
            if (this.tcs != null)
                return !this.tcs.Task.IsCompleted && !App.ApplicationSettings.Values.ContainsKey(ONE_DRIVE_SIGN_IN_KEY);
            else
                return false;
        }


        public bool IsPopup()
        {
            return false;
        }


        // Remove user and record
        public void SignOut()
        {
            LiveAuthClient liveAuthClient = new LiveAuthClient(LIVE_CLIENT_ID);
            liveAuthClient.Logout();
            App.ApplicationSettings.Values.Remove(ONE_DRIVE_SIGN_IN_KEY);
            this.FoldersTree.Clear();
            this.FolderRootTree.Clear();
            this.LiveClient = null;
            this.CurrentAccount = null;
        }


        public bool IsSignIn()
        {
            return App.ApplicationSettings.Values.ContainsKey(ONE_DRIVE_SIGN_IN_KEY);
        }


        public string GetStorageName()
        {
            return App.ResourceLoader.GetString(ResourcesKeys.OneDrive);
        }


        public string GetStorageImageUri()
        {
            return ONE_DRIVE_IMAGE_URI;
        }


        public string GetStorageColorHexString()
        {
            return ONE_DRIVE_COLOR_HEX_STRING;
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


        // Summary:
        //     Gets Root Folder of OneDrive storage.
        //     It will be used to access the storage in the begining.
        //
        // Returns:
        //     Root Folder of OneDrive.
        public async Task<FileObject> GetRootFolderAsync()
        {
            FileObject root = ConvertToFileObjectHelper.ConvertToFileObject((await this.LiveClient.GetAsync("me/skydrive")).Result);
            root.Name = "";
            return root;
        }


        // Summary:
        //     Gets files in Root Folder of OneDrive storage.
        //
        // Returns:
        //     List of FileObject in root folder.
        public async Task<List<FileObject>> GetRootFilesAsync()
        {
            return _GetDataList((await this.LiveClient.GetAsync("me/skydrive/files")).Result);
        }


        // Summary:
        //     Gets the mete information of the file(such as id, name, size, etc.) by file id.
        //
        // Parameters:
        //  fildId:
        //      The id of the file you want the get the file meta information.
        //
        // Returns:
        //     FileObject of the certain file id.
        public async Task<FileObject> GetFileAsync(string fileId)
        {
            return ConvertToFileObjectHelper.ConvertToFileObject((await this.LiveClient.GetAsync(fileId)).Result);
        }


        // Summary:
        //     Gets list of meta information by folder id.
        //
        // Parameters:
        //  fildId:
        //      The id of the folder you want the get the list of file meta information.
        //
        // Returns:
        //     List of FileObject of the folder id.
        public async Task<List<FileObject>> GetFilesFromFolderAsync(string folderId)
        {
            return _GetDataList((await this.LiveClient.GetAsync(folderId + "/files")).Result);
        }


        // Summary:
        //     Download a file by file id.
        //
        // Parameters:
        //  sourceFileId:
        //      The id of the file you want to download.
        //
        // Returns:
        //     The input stream to download file.
        public async Task<Stream> DownloadFileStreamAsync(string sourceFileId)
        {
            System.Threading.CancellationTokenSource ctsDownload = new System.Threading.CancellationTokenSource();
            LiveDownloadOperationResult result = null;
            try
            {
                //result = await this.LiveClient.DownloadAsync(sourceFileId + "/content");
                result = await this.LiveClient.BackgroundDownloadAsync(sourceFileId + "/content");
            }
            catch
            {
                throw new ShareException(sourceFileId, ShareException.ShareType.DOWNLOAD);
            }

            return result.Stream.AsStreamForRead();
        }


        // Summary:
        //     Upload files by output stream.
        //
        // Parameters:
        //  sourceFolderId:
        //      The id of the place you want to upload.
        //
        //  fileName:
        //      The name you want to use after upload.
        //
        // Returns:
        //     The StorageFolder where you downloaded folder.
        public async Task<bool> UploadFileStreamAsync(string folderIdToStore, string fileName, Stream outstream)
        {
            ProgressBar progressBar = new ProgressBar();
            progressBar.Value = 0;
            var progressHandler = new Progress<LiveOperationProgress>(
                (progress) => { progressBar.Value = progress.ProgressPercentage; });

            System.Threading.CancellationTokenSource ctsUpload = new System.Threading.CancellationTokenSource();
            try
            {
                //LiveOperationResult result = await this.LiveClient
                //    .UploadAsync(folderIdToStore, fileName, outstream, OverwriteOption.Overwrite, ctsUpload.Token, progressHandler);
                LiveOperationResult result = await this.LiveClient
                    .BackgroundUploadAsync(folderIdToStore, fileName, outstream.AsInputStream(), OverwriteOption.Overwrite, ctsUpload.Token, progressHandler);
            }
            catch
            {
                ctsUpload.Cancel();
                throw new ShareException(fileName, ShareException.ShareType.UPLOAD);
            }
            return true;
        }


        #region Private Methods
        ///////////////////
        // Private Methods
        ///////////////////

        private async Task<LiveConnectClient> GetLiveConnectClientAsync()
        {
            LiveAuthClient liveAuthClient = new LiveAuthClient(LIVE_CLIENT_ID);
            string[] scopes = new[] { "wl.basic", "wl.signin", "wl.offline_access", "wl.skydrive", "wl.skydrive_update", "wl.contacts_skydrive" };
            LiveLoginResult liveLoginResult = null;

            // Get Current live connection session
            try
            {
                liveLoginResult = await liveAuthClient.InitializeAsync(scopes);
            }
            catch (LiveAuthException)
            {
                return null;
            }

            // If session doesn't exist, get new one.
            // Otherwise, get the session.
            if (liveLoginResult.Status != LiveConnectSessionStatus.Connected)
            {
                try
                {
                    liveLoginResult = await liveAuthClient.LoginAsync(scopes);
                }
                catch (LiveAuthException)
                {
                    return null;
                }
            }

            // Get Client using session which we get above
            if (liveLoginResult.Session == null)
                return null;
            else
                return new LiveConnectClient(liveLoginResult.Session);
        }


        // Summary:
        //      List mapping method
        //
        // Returns:
        //      List of FileObject from a dictionary.
        private List<FileObject> _GetDataList(IDictionary<string, object> dic)
        {
            List<object> data = (List<object>)dic["data"];
            List<FileObject> list = new List<FileObject>();
            foreach (IDictionary<string, object> content in data)
            {
                FileObject fileObject = ConvertToFileObjectHelper.ConvertToFileObject(content);
                if (fileObject != null)
                    list.Add(fileObject);
            }
            return list;
        }


        // Summary:
        //      Gets the children of the FileObject recursively.
        //
        // Returns:
        //      Children list of given FileObject.
        private async Task<List<FileObject>> _GetChildAsync(FileObject fileObject)
        {
            if (FileObjectViewModel.FOLDER.Equals(fileObject.Type))
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


        // Summary:
        //     Get the file meta information from the root to the node of the file tree.
        //
        // Returns:
        //     Root FileObject of OneDrive.
        private async Task<FileObject> Synchronize()
        {
            FileObject fileObject = await GetRootFolderAsync();
            fileObject.FileList = await _GetChildAsync(fileObject);
            return fileObject;
        }
        #endregion

    }
}
