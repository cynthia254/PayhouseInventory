using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.ConnectorClasses.Response
{
    public  class Changepasswordvm
    {
        public string userEmail { get; set; }
        public string Currentpassword { get; set; }
        public string Password { get; set; }
        public string Renterpassord { get; set; }
    }
}
