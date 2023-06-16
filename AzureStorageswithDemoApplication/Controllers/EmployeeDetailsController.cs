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
        private readonly IAzureTableStorage _tableStorageService;
        private readonly ILogger<EmployeeDetailsController> _logger;
        public EmployeeDetailsController(IConfiguration configuration,IAzureTableStorage azureTable, ILogger<EmployeeDetailsController> logger)
        {
          
            _config = configuration;
            _tableStorageService = azureTable;
            _logger = logger;
        }
        [HttpPost]
        public async Task <IActionResult> CreateAzureTable([FromBody] EmployeeEntity emp)
        {
            try
            {
                var createdEntity = await _tableStorageService.CreateTable(emp);
                return CreatedAtAction(nameof(CreateAzureTable), createdEntity);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error try to add New Table :{ex.StackTrace}{ex.InnerException}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                                    "Error creating new Table record");
            }

        }
        [HttpPut]
       public async Task<IActionResult> UpdateEnployeeEntity([FromBody] EmployeeEntity emp)
        {
            try
            {
                var updateEntity = await _tableStorageService.UpdateEmployeeTable(emp);
                if (updateEntity == null)
                {
                    _logger.LogError($"Error while try to get Employee details records from Azure Table");
                    return NotFound("Employee Entity Table details Not Found in Azure Table");
                }
                return Ok("Updated Employee Data in Azure Table Successfully.");
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error try to updaate exisiting Table :{ex.StackTrace}{ex.InnerException}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                                    "Error update data in existing Table record");
            }
       }

        [HttpGet]
        public async Task<IActionResult> GetEmployeeEntity()
        {
            try
            {
                var getEntity = await _tableStorageService.GetEmployeeTable();
                if (getEntity == null)
                {
                    _logger.LogError($"Error while try to get Employee details records from Azure Table");
                    return NotFound("Employee Entity Table details Not Found in Azure Table");
                }
                return Ok(getEntity);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error try to get Employee details records from Azure Table :{ex.StackTrace}{ex.InnerException}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                                    "Error retrieving data from the Azure Table");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteEmployeeEntity([FromBody] EmployeeEntity emp)
        {
            try
            {
                var deleteEntity = await _tableStorageService.DeleteEmployeeTable(emp);
                if(deleteEntity == null)
                {
                    _logger.LogError($"Error while try to get Employee details records from Azure Table");
                    return NotFound("Employee Entity Table details Not Found in Azure Table");
                }
                return Ok("Deleted Employee Entity in Azure Table Sucessfully.");
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error try to get Employee details records from Azure Table :{ex.StackTrace}{ex.InnerException}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                                    "Error retrieving data from the Azure Table");

            }
           }



    }
}
