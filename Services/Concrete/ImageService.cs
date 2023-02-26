using System;
using RoofSafety.Models;
using RoofSafety.Services.Abstract;
using RoofSafety.Options;
using Microsoft.Extensions.Options;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace RoofSafety.Services.Concrete
{
	public class ImageService:IImageService
	{

        private readonly AzureOptions _azureOptions;
		public ImageService(IOptions<AzureOptions> azureOptions)
		{
            _azureOptions = azureOptions.Value  ;
		}

        public string UploadImageToAzure(IFormFile file)
        {
            string fileExtension = Path.GetExtension(file.FileName);
            using MemoryStream fileUploadStream = new MemoryStream();
            file.CopyTo(fileUploadStream);
            fileUploadStream.Position = 0;
            BlobContainerClient blobContainreClient = new BlobContainerClient(_azureOptions.ConnectionString,_azureOptions.Container);
            var UniqueName = Guid.NewGuid().ToString()+fileExtension;
            BlobClient blobClient = blobContainreClient.GetBlobClient(UniqueName);
            blobClient.Upload(fileUploadStream, new BlobUploadOptions()
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = "image/bitmap" }

            }, cancellationToken: default);
            return UniqueName;
        }

        public string GetImageURL(string UniqueName)
        {
            BlobContainerClient blobContainreClient = new BlobContainerClient(_azureOptions.ConnectionString, _azureOptions.Container);
            BlobClient blobClient = blobContainreClient.GetBlobClient(UniqueName);

            return blobClient.Uri.AbsoluteUri;
        }
    }
}

