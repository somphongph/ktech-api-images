using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using apiuploads.Models;


namespace apiuploads.Controllers
{
    [Route("api/v1/[controller]")]
    public class UploadsController : Controller
    {
        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<ActionResult> UploadFile(IFormFile file)
        {
            try
            {
                string subFolder = "uploads";
                string currentYear = DateTime.Now.Year.ToString();
                string currentMonth = DateTime.Now.Month.ToString().PadLeft(2, '0');
                string folderName = Path.Combine("resources", subFolder, currentYear, currentMonth);
                string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                if ( ! Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                if (file.Length > 0){
                    string storeName = Guid.NewGuid().ToString("N");
                    string fullPath = Path.Combine(uploadPath, storeName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
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
