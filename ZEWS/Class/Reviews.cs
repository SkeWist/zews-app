using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZEWS.Class
{
    public class Review
    {
        public long Id { get; set; }
        public DateTime Created { get; set; }
        public int Grade { get; set; }
        public string Text { get; set; }
        public int Moderated { get; set; }
        public NewUser User { get; set; }
        public HotelRoom Room { get; set; }
    }
}
