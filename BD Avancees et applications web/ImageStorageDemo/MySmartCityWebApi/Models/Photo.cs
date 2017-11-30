using System;


namespace MySmartCityWebApi.Models
{
    public class Photo
    {
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public long Size { get; set; }
        public string Uri { get; set; }

    }
}