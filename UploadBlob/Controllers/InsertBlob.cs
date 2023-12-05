using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace UploadBlob.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class  InsertBlob : Controller
    {
        private readonly IConfiguration _configuration;

        public InsertBlob(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("upload")]
        public async Task<ActionResult> Upload([FromForm] ICollection<IFormFile> files)
        {
            try
            {
                foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {
                        using (var stream = new MemoryStream())
                        {
                            await formFile.CopyToAsync(stream);

                            // Set your Azure Storage connection string in appsettings.json
                            string connectionString = _configuration.GetConnectionString("AzureBlobStorageConnection");

                            // Set the name of your container
                            string containerName = "your-container-name";

                            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
                            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

                            await container.CreateIfNotExistsAsync();

                            // Set the name of the blob (you can customize this as per your requirement)
                            string blobName = Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName);

                            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);

                            using (var blobStream = await blockBlob.OpenWriteAsync())
                            {
                                await blobStream.WriteAsync(stream.ToArray(), 0, stream.ToArray().Length);
                                await blobStream.CommitAsync();
                            }
                        }
                    }
                }

                return Ok("Files uploaded successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
