using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZEWS.Class
{

    public class Reservation
    {
        public long Id { get; set; }
        public DateTime Entry { get; set; }
        public DateTime Exit { get; set; }
        public DateTime Created { get; set; }

        public NewUser User { get; set; }
        public HotelRoom Room { get; set; }
    }
}
