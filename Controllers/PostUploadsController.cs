using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkiaSharp;
using ktech.images.Models;

namespace ktech.images.Controllers
{
    [Route("v1/post-uploads")]
    public class PostUploadsController : Controller
    {
        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<ActionResult> UploadFile(IFormFile file)
        {
            try
            {
                int[] dimensionWidth = {75, 150, 300, 600, 1920};    

                string folder = Request.Host.Host.ToString();
                string subFolder =  "posts";
                string folderName = Path.Combine("resources", folder, subFolder);     
                
                string currentYear = DateTime.Now.Year.ToString();
                string currentMonth = DateTime.Now.Month.ToString().PadLeft(2, '0');
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), folderName, currentYear, currentMonth);

                if (file.Length > 0) {
                    string storeName = Guid.NewGuid().ToString("N");                   

                    var memoryStream  = new MemoryStream();
                    file.CopyTo(memoryStream);                     
                    byte[] fileBytes = memoryStream .ToArray();

                    var ms = new MemoryStream(fileBytes);       
                    var sourceBitmap = SKBitmap.Decode(ms);
                    float ratio =   (float)sourceBitmap.Width / (float)sourceBitmap.Height;                        

                    foreach (int width in dimensionWidth) { 
                        string path = Path.Combine(filePath, width.ToString());
                        if ( ! Directory.Exists(path)) {
                            Directory.CreateDirectory(path);
                        }

                        string fullPath = Path.Combine(path, storeName);
                        using (var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write)) {
                            int targetWidth = width;
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

                    var imageLocation = new ImageLocation(){
                        name = file.FileName,
                        year = currentYear,
                        month = currentMonth,
                        storeName = storeName,
                        extension = Path.GetExtension(file.FileName),
                        contentType = file.ContentType
                    };

                    return Ok(imageLocation);
                } else {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
