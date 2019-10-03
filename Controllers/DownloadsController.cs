using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using apiuploads.Models;

namespace apiuploads.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DownloadsController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> DownloadFile([FromQuery]ImageDownload imageDownload)
        {
            // if (fileLocation == null) return BadRequest();
            // var fileDescription = _fileRepository.GetFileDescription(id);
            // var path = Path.Combine("resources", "uploads", "2019", "03", fileLocation);
            // string xx = imageDownload.storeName+".jpg";

            string fileLocation = Path.Combine(imageDownload.year, imageDownload.month, imageDownload.storeName);
            string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "resources", "uploads", fileLocation);

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
