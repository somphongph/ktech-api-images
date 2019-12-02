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
                    folderSize = "1920";
                    break;

            }

            string subFolder = Request.Host.Host.ToString() + ".posts";

            string fileLocation = Path.Combine(imageDownload.year, imageDownload.month, folderSize, imageDownload.storeName);
            string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "../../resources", subFolder, fileLocation);

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
