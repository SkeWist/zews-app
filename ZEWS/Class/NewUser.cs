using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ZEWS.Class
{
    internal class NewUser
    {
        string phone {  get; set; }
        string password { get; set; }
        string passwordRepeat { get; set; }
        string role { get; set; }
        DateTime birthday { get; set; }
        string surname { get; set; }
        string name { get; set; }
        string patronymic { get; set; }
        string pass_number { get; set; }
        string pass_authority_code { get; set; }
        string pass_authority_name { get; set; }
        string pass_birth_address { get; set; }  
        DateTime pass_issue_date { get; set; }
        bool sex { get; set; }
    }
               
}
