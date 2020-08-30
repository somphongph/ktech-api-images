using System;
using System.IO;
using System.Threading.Tasks;
using ktech.images.Models;
using SkiaSharp;

namespace ktech.images.Services
{
    public class ImagePoolService
    {    
        public async Task<ImageLocation> Upload(MemoryStream memoryStream,  string filePath, int[] dimensionWidth) {
        
            string storeName = Guid.NewGuid().ToString("N");                   

            // var memoryStream  = new MemoryStream();
            // file.CopyTo(memoryStream);                     
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
                // name = file.FileName,
                // year = currentYear,
                // month = currentMonth,
                storeName = storeName,
                // extension = Path.GetExtension(file.FileName),
                // contentType = file.ContentType
            };

            return imageLocation;
        }
    }
}