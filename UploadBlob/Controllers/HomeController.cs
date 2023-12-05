using Microsoft.AspNetCore.Mvc;

namespace UploadBlob.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : Controller
    {
        [HttpPost("upload")]
        public async Task<ActionResult> Upload([FromForm] ICollection<IFormFile> files)
        {
            List<byte[]> data = new();
            
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        await formFile.CopyToAsync(stream);
                        data.Add(stream.ToArray());
                    }
                }
            }
            return Ok();
        }
       
    }
}
