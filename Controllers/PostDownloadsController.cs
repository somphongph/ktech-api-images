using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using ktech.images.Models;

namespace ktech.images.Controllers
{
    [Route("v1/post-downloads")]
    [ApiController]
    public class PostDownloadsController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> DownloadFile([FromQuery]ImageDownload imageDownload)
        {
            string folderSize = "";
            switch (imageDownload.size) {
                case "sm":
                    folderSize = "150";
                    break;

                case "md":
                    folderSize = "300";
                    break;

                case "lg":
                    folderSize = "600";
                    break;

                case "xs":
                    folderSize = "75";
                    break;

                case "xl":
                    folderSize = "1920";
                    break;

                default:
                    folderSize = "150";
                    break;

            }

            string folder = Request.Host.Host.ToString();
            string subFolder =  "posts";
            string folderName = Path.Combine("resources", folder, subFolder);

            string fileLocation = Path.Combine(imageDownload.year, imageDownload.month, folderSize, imageDownload.storeName);
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), folderName, fileLocation);

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, imageDownload.contentType);
        }
    }
}
