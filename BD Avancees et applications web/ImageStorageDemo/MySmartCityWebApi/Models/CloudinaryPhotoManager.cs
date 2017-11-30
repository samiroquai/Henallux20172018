using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
namespace MySmartCityWebApi.Models
{
    /// <summary>
    /// Stocke les photos dans le service Cloudinary.
    /// Voir http://cloudinary.com/documentation/dotnet_integration
    /// </summary>
    public class CloudinaryPhotoManager : IPhotoManager
    {

        readonly Cloudinary _cloudinary;
        public CloudinaryPhotoManager()
        {
            var account = new Account(
 "my_cloud_name",
 "my_api_key",
 "my_api_secret");
            _cloudinary = new Cloudinary(account);
        }
        public async Task<IEnumerable<Photo>> Add(HttpRequestMessage request)
        {
            var streamProvider = new MultipartMemoryStreamProvider();
            await request.Content.ReadAsMultipartAsync(streamProvider);
            List<Photo> photosSaved = new List<Photo>();
            foreach (HttpContent ctnt in streamProvider.Contents)
            {
                using (Stream imageStream = await ctnt.ReadAsStreamAsync())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(ctnt.Headers.ContentDisposition.FileName, imageStream),
                        
                    };
                    ImageUploadResult uploadResult = await _cloudinary.UploadAsync(uploadParams);
                 
                    photosSaved.Add(new Photo { Uri = uploadResult.SecureUri.ToString() });
                }
            }
            return photosSaved;

        }

        public Task<PhotoActionResult> Delete(string fileName)
        {
            throw new NotImplementedException();
        }

        public bool FileExists(string fileName)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Photo>> Get()
        {
            throw new NotImplementedException();
        }
    }
}