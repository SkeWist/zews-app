using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZEWS.Class
{
    internal class Sex
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public Sex(string code, string name)
        {
            Code = code;
            Name = name;
        }
        public override string ToString()
        {
            return Code;
        }
    }
}
