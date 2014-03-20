using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using PintheCloudWS.Helpers;
using PintheCloudWS.Models;
using PintheCloudWS.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace PintheCloudWS.Managers
{
    public class BlobStorageManager
    {
        # region variables
        /// <summary>
        /// connection string to connect with Windows Azure Cloud Storage
        /// </summary>
        private string BLOB_CONNECTION = "DefaultEndpointsProtocol=http;AccountName=pinthecloud;AccountKey=BjLf1uc1g7Ll1rTlZJ4GR/gsAjmLx/7oHlQNnsd1olxrmHpNtDc91+cEXbMo1HyCDF+O//uc1V8uS1YK02Ad6Q==";
        /// <summary>
        /// Container that files will be stored in
        /// </summary>
        private string CONTAINER_NAME = "spot-container";

        private CloudStorageAccount storageAccount;
        private CloudBlobClient blobClient;
        private CloudBlobContainer container;
        # endregion


        /// <summary>
        /// Constructor. Connecting account, aquiring client connection and container.
        /// </summary>
        public BlobStorageManager()
        {
            this.storageAccount = CloudStorageAccount.Parse(this.BLOB_CONNECTION);
            this.blobClient = this.storageAccount.CreateCloudBlobClient();
            this.container = blobClient.GetContainerReference(this.CONTAINER_NAME);
        }


        /// <summary>
        /// Gets file meta information from the spot.
        /// </summary>
        /// <param name="id">The id of the file</param>
        /// <returns>FileObject containing file meta information</returns>
        public async Task<FileObject> GetFileAsync(string id)
        {
            CloudBlockBlob blockBlob = (CloudBlockBlob)await container.GetBlobReferenceFromServerAsync(id);
            return ConvertToFileObjectHelper.ConvertToFileObject(blockBlob);
        }


        /// <summary>
        /// Gets file meta information from the spot, using account id, spot id and path.
        /// </summary>
        /// <param name="account">The account id of the spot.</param>
        /// <param name="spotId">The spot id of the spot.</param>
        /// <param name="sourcePath">The path of the file</param>
        /// <returns>FileObject containing file meta information.</returns>
        public async Task<FileObject> GetFileAsync(string account, string spotId, string sourcePath)
        {
            sourcePath = ParseHelper.TrimSlash(sourcePath);
            return await this.GetFileAsync(account + "/" + spotId + "/" + sourcePath);
        }


        /// <summary>
        /// Gets List of file meta information with directory.
        /// </summary>
        /// <param name="account">The account id of the spot</param>
        /// <param name="spotId">The spot id</param>
        /// <returns>List of the FileObject containing file meta information</returns>
        public Task<List<FileObject>> GetFilesFromSpotAsync(string account, string spotId)
        {
            return this._GetFilesFromFolderByIdAsync(account + "/" + spotId);
        }


        /// <summary>
        /// Downloads file to the given StorageFile
        /// </summary>
        /// <param name="id">The id of the file to download</param>
        /// <param name="downloadFile">The file downloaded to</param>
        /// <returns>The downloaded file</returns>
        public async Task<StorageFile> DownloadFileAsync(string id, StorageFile downloadFile)
        {
            try
            {
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(id);
                using (Stream s = await downloadFile.OpenStreamForWriteAsync())
                {
                    await blockBlob.DownloadToStreamAsync(s.AsOutputStream());
                }
                return downloadFile;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw new ShareException(id, ShareException.ShareType.DOWNLOAD);
            }
        }


        /// <summary>
        /// Downloads file to the given StorageFile using directory.
        /// </summary>
        /// <param name="id">The id of the file to download</param>
        /// <param name="sourcePath">The path to download</param>
        /// <param name="downloadFile">The file downloaded to</param>
        /// <returns>The downloaded file</returns>
        public async Task<StorageFile> DownloadFileAsync(string account, string spotId, string sourcePath, StorageFile downloadFile)
        {
            sourcePath = ParseHelper.TrimSlash(sourcePath);
            return await this.DownloadFileAsync(account + "/" + spotId + "/" + sourcePath, downloadFile);
        }


        /// <summary>
        /// Gets Download Stream
        /// </summary>
        /// <param name="id">The file id to download</param>
        /// <returns>The input stream for downloading file</returns>
        public async Task<Stream> DownloadFileStreamAsync(string id)
        {
            try
            {
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(id);

                // Windows Phone 8
                //return await blockBlob.OpenReadAsync();

                var fStream = await blockBlob.OpenReadAsync();
                var reader = new DataReader(fStream.GetInputStreamAt(0));
                var bytes = new byte[fStream.Size];
                await reader.LoadAsync((uint)fStream.Size);
                reader.ReadBytes(bytes);
                return new MemoryStream(bytes);
            }
            catch
            {
                throw new ShareException(id, ShareException.ShareType.DOWNLOAD);
            }
        }


        /// <summary>
        /// Upload File to a given stream.
        /// </summary>
        /// <param name="account">The account id to upload</param>
        /// <param name="spotId">The spot id to upload</param>
        /// <param name="file">The name to be used after uploaded</param>
        /// <param name="stream">The stream to upload</param>
        /// <returns>The file id</returns>
        public async Task<string> UploadFileStreamAsync(string account, string spotId, string fileName, Stream stream)
        {
            try
            {
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(account + "/" + spotId + "/" + fileName);
                using (Stream s = stream)
                {
                    await blockBlob.UploadFromStreamAsync(s.AsInputStream());
                }
            }
            catch
            {
                throw new ShareException(account + "/" + spotId + "/" + fileName, ShareException.ShareType.UPLOAD);
            }
            return account + "/" + spotId + "/" + fileName;
        }


        /// <summary>
        /// Deletes the file
        /// </summary>
        /// <param name="id">The id of the file to delete</param>
        /// <returns>True if succeeded, false if not.</returns>
        public async Task<bool> DeleteFileAsync(string id)
        {
            try
            {
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(id);
                await blockBlob.DeleteAsync();
            }
            catch
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// Deleting every files in given SpotId
        /// </summary>
        /// <param name="accountId">The account</param>
        /// <param name="spotId">The spot id to delete</param>
        /// <returns>True if suceed otherwise false</returns>
        public async Task<bool> DeleteSpotAsync(string accountId, string spotId)
        {
            try
            {
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(accountId + "/" + spotId);
                await blockBlob.DeleteAsync();
            }
            catch
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// Deleting every files in given accountId
        /// </summary>
        /// <param name="accountId">The account</param>
        /// <returns>True if suceed otherwise false</returns>
        public async Task<bool> DeleteAccountAsync(string account)
        {
            try
            {
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(account);
                await blockBlob.DeleteAsync();
            }
            catch
            {
                return false;
            }
            return true;
        }


        # region Private Methods
        private async Task<List<FileObject>> _GetFilesFromFolderByIdAsync(string id)
        {
            return await this._GetFileObjectListFromBlobClient(this.CONTAINER_NAME + "/" + id + "/");
        }


        private async Task<string> _UploadFileAsyncById(string id, StorageFile uploadfile)
        {
            try
            {
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(id);
                using (Stream s = await uploadfile.OpenStreamForReadAsync())
                {
                    await blockBlob.UploadFromStreamAsync(s.AsInputStream());
                }
            }
            catch
            {
                return null;
            }
            return id;
        }


        private async Task<List<FileObject>> _GetFileObjectListFromBlobClient(string prefix)
        {
            List<FileObject> list = new List<FileObject>();
            BlobContinuationToken token = null;

            do
            {
                BlobResultSegment blobListSegment = await this.blobClient.ListBlobsSegmentedAsync(prefix, token);
                list.AddRange(this._GetDataList(blobListSegment.Results));
                token = blobListSegment.ContinuationToken;
            } while (token != null);
            return list;
        }


        private FileObject _GetData(IListBlobItem item)
        {
            if (item.GetType() == typeof(CloudBlockBlob))
            {
                return ConvertToFileObjectHelper.ConvertToFileObject((CloudBlockBlob)item);
            }
            else if (item.GetType() == typeof(CloudBlobDirectory))
            {
                return ConvertToFileObjectHelper.ConvertToFileObject((CloudBlobDirectory)item);
            }
            return null;
        }


        private List<FileObject> _GetDataList(IEnumerable<IListBlobItem> result)
        {
            List<FileObject> list = new List<FileObject>();
            foreach (IListBlobItem item in result)
            {
                FileObject fo = this._GetData(item);
                if (fo != null) list.Add(fo);
            }
            return list;
        }
        # endregion
    }
}
