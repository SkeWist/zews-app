using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZEWS.Class
{
    public class ImageViewModel
    {
        public ImageViewModel(string name, string path)
        {
            Name = name;
            Path = path;
        }

        public string Name { get; set; }
        public string Path { get; set; }
    }
}
