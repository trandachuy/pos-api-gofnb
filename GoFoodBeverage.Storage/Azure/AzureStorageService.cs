using System;
using System.IO;
using Azure.Storage;
using Azure.Storage.Blobs;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using GoFoodBeverage.Domain.Settings;
using GoFoodBeverage.Storage.Azure.Models;

namespace GoFoodBeverage.Storage.Azure
{
    public class AzureStorageService : IAzureStorageService
    {
        private readonly AppSettings _appSettings;
        private readonly AzureStorageSettings _azureStorageSettings;

        public AzureStorageService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            _azureStorageSettings = _appSettings.AzureStorageSettings;
        }

        public async Task<string> UploadFileToStorageAsync(Stream fileStream, string fileName)
        {
            // Create a URI to the blob
            string url = $"{_azureStorageSettings.BlobUri}/{fileName}";
            Uri blobUri = new (url);

            // Create StorageSharedKeyCredentials object by reading
            StorageSharedKeyCredential storageCredentials = new (_azureStorageSettings.AccountName, _azureStorageSettings.AccountKey);

            // Create the blob client.
            BlobClient blobClient = new (blobUri, storageCredentials);

            // Upload the file
            await blobClient.UploadAsync(fileStream);

            return await Task.FromResult(url);
        }

        public async Task<string> UploadFileToStorageAsync(FileUploadRequestModel request)
        {
            var fileSizeLimit = request.FileSizeLimit;
            if (request.File.Length > fileSizeLimit)
            {
                throw new Exception("File size maximum is 5MB.");
            }

            // Create a URI to the blob
            var fileExtension = Path.GetExtension(request.File.FileName);
            string url = $"{_azureStorageSettings.BlobUri}/{request.FileName}{fileExtension}";
            Uri blobUri = new(url);

            // Create StorageSharedKeyCredentials object by reading
            StorageSharedKeyCredential storageCredentials = new(_azureStorageSettings.AccountName, _azureStorageSettings.AccountKey);

            // Create the blob client.
            BlobClient blobClient = new(blobUri, storageCredentials);

            // Upload the file
            using Stream fileStream = request.File.OpenReadStream();
            await blobClient.UploadAsync(fileStream);

            return await Task.FromResult(url);
        }
    }
}
