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
                //delete file
                await file.DeleteAsync();
            }
            catch(RequestFailedException ex) when (ex.ErrorCode == BlobErrorCode.BlobNotFound)
            {
                _logger.LogError($"File{blobFileName} not found.");
                return new BlobResponseDto { Error = true, Status = $"File with name{blobFileName} not found." };
            }
            //Return a new BlobResponseDto to the requesting method.
            return new BlobResponseDto { Error = false, Status = $"File:{blobFileName} has been successfully deleted." };
           
        }

        public async Task<BlobDto> DownloadAsync(string blobFileName)
        {
            //Get a reference to a container named in appsetting.json
            BlobContainerClient client = new BlobContainerClient(_storageConnecctionString, _storageContainerName);
            try
            {
                //Get a reference to teh blob uploadedearlier from the Api in the container from config settong
                BlobClient file = client.GetBlobClient(blobFileName);
                //Check if the file exists in the container
                if(await file.ExistsAsync())
                {
                    var data = await file.OpenReadAsync();
                    Stream blobContent = data;
                    //Download the file details async
                    var content = await file.DownloadContentAsync();
                    //Add data to variables in order to return a blob dto
                    string name = blobFileName;
                    string contentType = content.Value.Details.ContentType;
                    //Create new BlobDto with blob data from variables
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
            //Create new upload response object that we can return tothe requesting method
            BlobResponseDto response = null;
            //Get a reference to a container named in app settings.json and then create it
            BlobContainerClient container = new BlobContainerClient(_storageConnecctionString, _storageContainerName);
            //await container.CreateAsync
            try
            {
                //Get a reference to the blob just uploaded from the Api in a container from configuraation setting
                BlobClient client = container.GetBlobClient(file.FileName);
                //Open a stream for the file we want to upload
                await using (Stream? data = file.OpenReadStream())
                {
                    //upload the file async
                    await client.UploadAsync(data);
                }
                //Everything is Ok and file got uploaded
                response.Status = $"File{file.FileName}Uploaded Successfully.";
                response.Error = false;
                response.Blob.Uri = client.Uri.AbsoluteUri;
                response.Blob.Name = client.Name;
               
            }
            //If the file already exists,we catch the exception and do not upload it
            catch(RequestFailedException ex)
            when (ex.ErrorCode == BlobErrorCode.BlobAlreadyExists)
            {
                _logger.LogError($"File with name {file.FileName} already exists in container.Set another name to store the file in the container:'{_storageContainerName}.'");
                response.Status = ($"File with name {file.FileName} already exists. Please use another name to store your file");
                response.Error = true;
                return response;
            }
            //If we get an unexpected error,we catch it here and return the error message.
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
