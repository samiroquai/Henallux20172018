using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace MySmartCityWebApi.Models
{
    public interface IPhotoManager
    {
        Task<IEnumerable<Photo>> Get();
        Task<PhotoActionResult> Delete(string fileName);
        Task<IEnumerable<Photo>> Add(HttpRequestMessage request);
        bool FileExists(string fileName);
    }
}

