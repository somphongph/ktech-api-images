using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using api.images.Models;
using System;

namespace api.images.Controllers
{
    [Route("v1/post-downloads")]
    [ApiController]
    public class PostDownloadsController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> DownloadFile([FromQuery]ImageDownload imageDownload)
        {

            // if (fileLocation == null) return BadRequest();
            // var fileDescription = _fileRepository.GetFileDescription(id);
            // var path = Path.Combine("resources", "uploads", "2019", "03", fileLocation);
            // string xx = imageDownload.storeName+".jpg";

            string subFolder = Request.Host.Host.ToString() + ".posts";

            string fileLocation = Path.Combine(imageDownload.year, imageDownload.month, imageDownload.size, imageDownload.storeName);
            string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "resources", subFolder, fileLocation);

            var memory = new MemoryStream();
            using (var stream = new FileStream(uploadPath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, imageDownload.contentType);
        }
    }
}
