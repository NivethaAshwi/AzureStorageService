using Azure;
using AzureStorage.Models.BlobStorageModel;
using AzureStorage.Service.Interface;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
            //Check if we got an error
            if (response.Error == true)
            {
                //We got an error during upload,return an error with details to the client
                return StatusCode(StatusCodes.Status500InternalServerError, response.Status);
            }
            else
            {
                //Return a success message to the client about Sucessfull upload
                return StatusCode(StatusCodes.Status200OK, response);
            }
            
        }
        [HttpGet("{filename}")]
        public async Task<IActionResult> Download(string filename)
        {
            BlobDto? file = await _storage.DownloadAsync(filename);
            //Check if file was found
            if(file == null)
            {
                //Was not,retutn error message to client
                return StatusCode(StatusCodes.Status500InternalServerError, $"File{filename} could not be download");
            }
            else
            {
                //File was found, return it to client
                return File(file.Content, file.ContentType, file.Name);
            }
        }
        [HttpDelete("filename")]
        public async Task<IActionResult> Delete(String filename)
        {
            BlobResponseDto response = await _storage.DeleteAsync(filename);
            //Check if we got an error
            if(response.Error == true)
            {
                //Return an errror message to the client
                return StatusCode(StatusCodes.Status500InternalServerError, $"File{filename} could not be download");
            }
            else
            {
                //File has been sucessfully deleted
                return StatusCode(StatusCodes.Status200OK, response);
            }
        }

    }
}
