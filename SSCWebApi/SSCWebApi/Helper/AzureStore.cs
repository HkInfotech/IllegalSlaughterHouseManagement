using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;


namespace SSCWebApi.Helper
{
    public static class AzureStore
    {
        public static string[] UploadFile(Stream fs, string name, string mediatype, bool isImage, string extension)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(ConfigurationManager.AppSettings["StorageContainer"]);
            container.CreateIfNotExists();
            container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

            string[] RetValue = new string[] { "", "" };
            string blobName = Guid.NewGuid().ToString();
            blobName = String.Format("{0}{1}", blobName, extension);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);

            blockBlob.Properties.ContentType = mediatype;
            fs.Seek(0, SeekOrigin.Begin);
            blockBlob.UploadFromStream(fs);
            RetValue[0] = blockBlob.Uri.AbsoluteUri;

            return RetValue;
        }
    }
}