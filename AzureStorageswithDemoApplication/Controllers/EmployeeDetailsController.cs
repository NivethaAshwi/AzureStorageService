using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AzureStorage.Service;
using AzureStorage.Service.Service;
using AzureStorage.Models;
using Microsoft.Azure.Cosmos.Table;
using AzureStorage.Models.TableStorageModel;
using EmployeeEntity = AzureStorage.Models.TableStorageModel.EmployeeEntity;
using Microsoft.Extensions.Configuration;
using Azure.Data.Tables;
using Azure;
using System.Net;
using AzureStorage.Service.Interface;

namespace AzureStorageswithDemoApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeDetailsController : ControllerBase
    {
        
        private readonly IConfiguration _config;
        private readonly IAzureTableStorage __storageService;
        public EmployeeDetailsController(IConfiguration configuration,IAzureTableStorage azureTable)
        {
          
            _config = configuration;
            __storageService = azureTable;
        }
        [HttpPost]
        public async Task <IActionResult> Post([FromBody] EmployeeEntity emp)
        {

            // Method 2
            var createdEntity = await __storageService.Create(emp);
            return CreatedAtAction(nameof(Post), createdEntity);

        }


    }
}
