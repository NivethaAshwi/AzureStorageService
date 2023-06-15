using Azure;
using AzureStorage.Models.BlobStorageModel;
using AzureStorage.Service.Interface;
using Microsoft.AspNetCore.Mvc;


namespace AzureStorageswithDemoApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlobStorageController : ControllerBase
    {
        private readonly IAzureBlobStorage _storage;
        public BlobStorageController(IAzureBlobStorage blobStorage)
        {
            _storage = blobStorage;
        }
        [HttpPost(nameof(Upload))]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            BlobResponseDto? response = await _storage.UploadAsync(file);
             if (response.Error == true)
            {
                 return StatusCode(StatusCodes.Status500InternalServerError, response.Status);
            }
            else
            {
                 return StatusCode(StatusCodes.Status200OK, response);
            }
            
        }
        [HttpGet("{filename}")]
        public async Task<IActionResult> Download(string filename)
        {
            BlobDto? file = await _storage.DownloadAsync(filename);
            if(file == null)
            {
               return StatusCode(StatusCodes.Status500InternalServerError, $"File{filename} could not be download");
            }
            else
            {
                return File(file.Content, file.ContentType, file.Name);
            }
        }
        [HttpDelete("filename")]
        public async Task<IActionResult> Delete(String filename)
        {
            BlobResponseDto response = await _storage.DeleteAsync(filename);
          
            if(response.Error == true)
            {
             return StatusCode(StatusCodes.Status500InternalServerError, $"File{filename} could not be download");
            }
            else
            {
               return StatusCode(StatusCodes.Status200OK, response);
            }
        }

    }
}
