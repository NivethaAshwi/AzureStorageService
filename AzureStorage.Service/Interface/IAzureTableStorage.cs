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
        /// <summary>
        /// This method uploads a file submitted with the request
        /// </summary>
        /// <param name="file">File for Upload</param>
        /// <returns>blob with status</returns>
        Task<Models.BlobStorageModel.BlobResponseDto> UploadAsync(IFormFile file);
        /// <summary>
        /// This method downloads a file with the specified file name
        /// </summary>
        /// <param name="blobFileName">Filename</param>
        /// <returns>Blob</returns>
        Task<BlobDto> DownloadAsync(string blobFileName);
        /// <summary>
        /// This method deleted a file with the specified filename
        /// </summary>
        /// <param name="blobFileName"></param>
        /// <returns>blob with status</returns>
        Task<Models.BlobStorageModel.BlobResponseDto> DeleteAsync(string blobFileName);
    

    }
}
