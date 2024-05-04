using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZEWS.Class
{
    public class Photo
    {
        public long Id {  get; set; }
        public string Name { get; set; }
        public Photo(long id, string name)
        {
            Id = id;
            Name = name;
        }
        public override string ToString()
        {
            return APIconfig.APIstorage + "/" + Name;
        }
        
        public string URL { 
            get 
            {
                return APIconfig.APIstorage + "/" + Name;
            } }
    }
}
