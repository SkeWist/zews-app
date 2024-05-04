using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ZEWS.Class
{
    public class RoomType
    {
        public long Id {  get; set; }
        public string Name { get; set; }

        public RoomType(long id, string name)
        {
            Id = id;
            Name = name;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
