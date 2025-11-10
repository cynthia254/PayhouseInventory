using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.ConnectorClasses.Response.resetpassword
{
    public  class ResetPasswordvm
    {
        public string verificationtoken { get; set; }
        public string Password { get; set; }
        public string RetypePassword { get; set; }
    }
}
