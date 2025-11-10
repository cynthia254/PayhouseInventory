using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.ConnectorClasses.Response.roleresponse
{
    public  class RoleClaimsResponse
    {

        public bool IsTrue { get; set; }
        public string Message { get; set; }
        public string Role { get; set; }

        public object Claims { get; set; }
        public RoleClaimsResponse(bool isTrue, string message, string role, object claims)
        {
            IsTrue = isTrue;
            Message = message;
            Role = role;
            Claims = claims;    

        }
    }
}
