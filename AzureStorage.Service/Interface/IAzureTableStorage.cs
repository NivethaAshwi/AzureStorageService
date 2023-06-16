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
        Task<List<EmployeeEntity>> CreateTable(EmployeeEntity employeeDetails);
        Task<List<EmployeeEntity>> GetEmployeeTable();
        Task<List<EmployeeEntity>> UpdateEmployeeTable(EmployeeEntity employeeDetails);
        Task<List<EmployeeEntity>> DeleteEmployeeTable(EmployeeEntity employeeDetails);

    }
    public interface IAzureBlobStorage
    {
          Task<BlobResponseDto> UploadAsync(IFormFile file);
          Task<BlobDto> DownloadAsync(string blobFileName);
          Task<BlobResponseDto> DeleteAsync(string blobFileName);
    

    }
}
