using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using SkiaSharp;
using System.Collections.Generic;
using System.Linq;

namespace ktech.images.Controllers
{
    [Route("v1/place-images")]
    [ApiController]
    public class PlaceImagesController : Controller
    {
        [HttpGet("{filename}")]
        public async Task<IActionResult> Download(string filename, [FromQuery]string size)
        {
            if (filename == null) return NotFound();

            string[] folderSize = { "xxs", "xs", "sm", "md", "lg", "xl"};

            if (folderSize.Contains(size) == false )  return BadRequest();

            string filenameWithoutExtension = Path.GetFileNameWithoutExtension(filename);

            string folder = Request.Host.Host.ToString();
            string subFolder =  "places";
            string folderName = Path.Combine("resources", folder, subFolder);

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), folderName, size, filenameWithoutExtension);

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, GetContentType(filename));
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }

        [HttpPost]
        public async Task<ActionResult<string>> Create(IFormFile file)
        {
            if (file == null || file.Length == 0) {
                return BadRequest();
            }

            try
            {
                Dictionary<string, int> dimensionWidth = new Dictionary<string, int>()
                {
                    { "xxs", 10 }, 
                    { "xs", 100 },
                    { "sm", 300 },
                    { "md", 600 },
                    { "lg", 1024 },
                    { "xl", 1920 }
                };

                string folder = Request.Host.Host.ToString();
                string subFolder =  "places";
                string folderName = Path.Combine("resources", folder, subFolder);     
                
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), folderName);                
                string storeName = Path.GetFileNameWithoutExtension(file.FileName) + "-" + Guid.NewGuid().ToString("N");                   

                var memoryStream  = new MemoryStream();
                file.CopyTo(memoryStream);                     
                byte[] fileBytes = memoryStream .ToArray();

                var ms = new MemoryStream(fileBytes);       
                var sourceBitmap = SKBitmap.Decode(ms);
                float ratio =   (float)sourceBitmap.Width / (float)sourceBitmap.Height;                        

                foreach (KeyValuePair<string, int> dimension in dimensionWidth) { 
                    string path = Path.Combine(filePath, dimension.Key.ToString());
                    if ( ! Directory.Exists(path)) {
                        Directory.CreateDirectory(path);
                    }

                    string fullPath = Path.Combine(path, storeName);
                    using (var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write)) {
                        int targetWidth = dimension.Value;
                        int targetHeight =  (int)((float)targetWidth / ratio);

                        var skImageInfo = new SKImageInfo(targetWidth, targetHeight);
                        var quality = SKFilterQuality.High;

                        var scaledBitmap = sourceBitmap.Resize(skImageInfo, quality);
                        var scaledImage = SKImage.FromBitmap(scaledBitmap);
                        var data = scaledImage.Encode();
                        byte[] fileResize = data.ToArray();

                        await fs.WriteAsync(fileResize, 0, fileResize.Length);
                    }
                }                    

                var imageUrl = $"place-images/{storeName}{Path.GetExtension(file.FileName).ToLowerInvariant()}";

                return Ok(imageUrl);
                
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}