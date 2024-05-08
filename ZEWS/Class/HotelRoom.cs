using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ZEWS.Class
{
    public class HotelRoom
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Type { get; set; }
        public List<Photo> Photos { get; set; } // список фотографий
        public static List<Photo> AddPhotos (JObject photosArray)
        {
            var photoList = new List<Photo>();
            if (photosArray is JObject)
            {
                foreach (var Photo in photosArray.Properties())
                {
                    photoList.Add(new Photo(Convert.ToInt64(Regex.Replace(Photo.Name, @"[^\d]", "")), Photo.Value.ToString()));
                }
            }
            return photoList;
        }
    }
}
