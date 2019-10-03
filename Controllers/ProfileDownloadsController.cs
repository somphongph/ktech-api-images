using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using apiuploads.Models;

namespace apiuploads.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProfileDownloadsController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> DownloadFile([FromQuery]ProfileImage profileImage)
        {
            // if (fileLocation == null) return BadRequest();
            // var fileDescription = _fileRepository.GetFileDescription(id);
            // var path = Path.Combine("resources", "uploads", "2019", "03", fileLocation);
            // string xx = imageDownload.storeName+".jpg";

            string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "resources", "profiles", profileImage.storeName);

            var memory = new MemoryStream();
            using (var stream = new FileStream(uploadPath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, profileImage.contentType);
        }
    }
}