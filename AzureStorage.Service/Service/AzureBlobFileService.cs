using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using AzureStorage.Models.BlobStorageModel;
using AzureStorage.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Azure;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;

namespace AzureStorage.Service.Service
{
    public class AzureBlobFileService : IAzureBlobStorage
    {
        #region Dependency Injection/Constructor
        private readonly string _storageConnecctionString;
        private readonly string _storageContainerName;
        private readonly ILogger<AzureBlobFileService> _logger;
        public AzureBlobFileService(IConfiguration configuration, ILogger<AzureBlobFileService> logger)
        {
            _storageConnecctionString = configuration.GetValue<string>("StorageConnectionString");
            _storageContainerName = configuration.GetValue<string>("BlobContainerName");
            _logger = logger;

        }

        public async Task<BlobResponseDto> DeleteAsync(string blobFileName)
        {
            BlobContainerClient client = new BlobContainerClient(_storageConnecctionString, _storageContainerName);
            BlobClient file = client.GetBlobClient(blobFileName);
            try
            {
                 await file.DeleteAsync();
            }
            catch(RequestFailedException ex) when (ex.ErrorCode == BlobErrorCode.BlobNotFound)
            {
                _logger.LogError($"File{blobFileName} not found.");
                return new BlobResponseDto { Error = true, Status = $"File with name{blobFileName} not found." };
            }
             return new BlobResponseDto { Error = false, Status = $"File:{blobFileName} has been successfully deleted." };
           
        }

        public async Task<BlobDto> DownloadAsync(string blobFileName)
        {
             BlobContainerClient client = new BlobContainerClient(_storageConnecctionString, _storageContainerName);
            try
            {
                BlobClient file = client.GetBlobClient(blobFileName);
                if(await file.ExistsAsync())
                {
                    var data = await file.OpenReadAsync();
                    Stream blobContent = data;
                    var content = await file.DownloadContentAsync();
                     string name = blobFileName;
                    string contentType = content.Value.Details.ContentType;
                    return new BlobDto { Content = blobContent, Name = name, ContentType = contentType };

                }
            }
            catch (RequestFailedException ex) when (ex.ErrorCode == BlobErrorCode.BlobNotFound)
            {
                _logger.LogError($"File {blobFileName} was not found.");

            }
            return null;
        }

        public async Task<BlobResponseDto> UploadAsync(IFormFile file)
        {
            BlobResponseDto response = null;
             BlobContainerClient container = new BlobContainerClient(_storageConnecctionString, _storageContainerName);
            try
            {
                BlobClient client = container.GetBlobClient(file.FileName);
                await using (Stream? data = file.OpenReadStream())
                {
                    await client.UploadAsync(data);
                }
                response.Status = $"File{file.FileName}Uploaded Successfully.";
                response.Error = false;
                response.Blob.Uri = client.Uri.AbsoluteUri;
                response.Blob.Name = client.Name;
               
            }
            catch(RequestFailedException ex)
            when (ex.ErrorCode == BlobErrorCode.BlobAlreadyExists)
            {
                _logger.LogError($"File with name {file.FileName} already exists in container.Set another name to store the file in the container:'{_storageContainerName}.'");
                response.Status = ($"File with name {file.FileName} already exists. Please use another name to store your file");
                response.Error = true;
                return response;
            }
            catch(RequestFailedException ex)
            {
                _logger.LogError($"Unhandled Exception. ID: {ex.StackTrace} - Message:{ex.Message}");
                response.Status = $"Unexpected error:{ex.StackTrace}.Check log with StackTrace ID";
                response.Error = true;
                return response;
            }
            return response;
        }
        #endregion
    }
}
