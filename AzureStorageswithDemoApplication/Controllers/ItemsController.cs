using AzureStorage.Service.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AzureStorage.Models.TableStorageModel;
using AzureStorage.Service.Interface;
namespace AzureStorageswithDemoApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly IAzureTableStorage _tablestorageService;
        private readonly ILogger<ItemsController> _logger;
        public ItemsController(IAzureTableStorage storageService, ILogger<ItemsController> logger)
        {
            _tablestorageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
            _logger = logger;

        }
        [HttpGet]
        [ActionName(nameof(GetGroceryItems))]
        public async Task<IActionResult> GetGroceryItems([FromQuery] string category, string id)
        {
            try
            {
                return Ok(await _tablestorageService.GetGroceryDetails(category, id));
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error try to Get Grocery item Table details :{ex.StackTrace}{ex.InnerException}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                                    "Error retrieving data from Azure Table");
            }
        }
        [HttpPost]
        public async Task<IActionResult> InsertGroceryEntity([FromBody] GroceryItemEntity entity)
        {
            try
            {
                entity.PartitionKey = entity.Category;
                string Id = Guid.NewGuid().ToString();
                entity.Id = Id;
                entity.RowKey = Id;
                var createdEntity = await _tablestorageService.GrocerryEntityDetails(entity);
                return CreatedAtAction(nameof(InsertGroceryEntity), createdEntity);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error try to add Grocery in Azure Table storage :{ex.StackTrace}{ex.InnerException}");
                return StatusCode(StatusCodes.Status500InternalServerError,
           "Error creating new grocery record in Azure Table");
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateGroceryEntity([FromBody] GroceryItemEntity entity)
        {
            try
            {
                entity.PartitionKey = entity.Category;
                entity.RowKey = entity.Id;
                await _tablestorageService.GrocerryEntityDetails(entity);
                return Ok("Updated GroceryEntity Details in Azure Table Successfully.");
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error try to update Update GroceryEntity in Azure Table :{ex.StackTrace}{ex.InnerException}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                "Error updating Update GroceryEntity data in Azure Table");
            }
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteGroceryEntity([FromQuery] string category, string id)
        {
            try
            {
                await _tablestorageService.DeleteGroceryDetails(category, id);
                return Ok($"Deleted Grocery Details in Azure Table Based on id:{id},Category:{category}");
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error try to Delete GroceryEntity Details by category, id  in Azure Table:{id}{ex.StackTrace}{ex.InnerException}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                                "Error Delete GroceryEntity data in Azure table");
            }
        }

    }
}
