using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using ktech.images.Models;


namespace ktech.images.Controllers
{
    [Route("v1/profile-uploads")]
    public class ProfileUploadsController : Controller
    {
        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<ActionResult> UploadFile(IFormFile file)
        {
            try
            {
                string folder =  Request.Host.Host.ToString();
                string subFolder =  "profiles";
                string folderName = Path.Combine("resources", folder, subFolder);
                
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                if ( ! Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

                if (file.Length > 0){
                    string storeName = Guid.NewGuid().ToString("N");
                    string fullPath = Path.Combine(filePath, storeName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var profileImage = new ProfileImage(){
                        storeName = storeName,
                        contentType = file.ContentType
                    };

                    return Ok(profileImage);
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