using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureStorage.Models.BlobStorageModel;
using AzureStorage.Models.BlobStorageModel;
using AzureStorage.Models.TableStorageModel;
using Microsoft.AspNetCore.Http;


namespace AzureStorage.Service.Interface
{
    public interface IAzureTableStorage
    {
        Task<Models.TableStorageModel.GroceryItemEntity> GetEntityAsync(string category, string id);
        
        Task<Models.TableStorageModel.GroceryItemEntity> UpsertEntityAsync(Models.TableStorageModel.GroceryItemEntity employeeDetails);
        Task DeleteEntityAsync(string category, string id);


        Task<List<EmployeeEntity>> Create(EmployeeEntity employeeDetails);

    }
    public interface IAzureBlobStorage
    {
          Task<Models.BlobStorageModel.BlobResponseDto> UploadAsync(IFormFile file);
          Task<BlobDto> DownloadAsync(string blobFileName);
         Task<Models.BlobStorageModel.BlobResponseDto> DeleteAsync(string blobFileName);
    

    }
}
