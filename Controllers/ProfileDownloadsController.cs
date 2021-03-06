using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ktech.images.Models;

namespace ktech.images.Controllers
{
    [Route("v1/profile-downloads")]
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
            string folder =  Request.Host.Host.ToString();
            string subFolder =  "profiles";
            string folderName = Path.Combine("resources", folder, subFolder);
            
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), folderName, profileImage.storeName);

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, profileImage.contentType);
        }
    }
}