using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ZEWS.Class
{
    public class NewUser
    {
        public long id { get; set; }
        public long phone { get; set; }
        public string password { get; set; }
        public string passwordRepeat { get; set; }
        public string role { get; set; }
        public DateTime birthday { get; set; }
        public string surname { get; set; }
        public string name { get; set; }
        public string patronymic { get; set; }
        public long pass_number { get; set; }
        public int pass_authority_code { get; set; }
        public string pass_authority_name { get; set; }
        public string pass_birth_address { get; set; }
        public DateTime pass_issue_date { get; set; }
        public bool sex { get; set; }

        public string GetFIO 
        {  
            get 
            { 
                return surname + " " + name + " " + patronymic; 
            }
        }

    }
}
