using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.ConnectorClasses.Response.roleresponse
{
    public class RolesResponse
    {
        public bool IsTrue { get; set; }
        public string Message { get; set; }
        public object Body { get; set; }
        public string V1 { get; }
        public string V2 { get; }

        public RolesResponse(bool istrue, string message, object body)
        {
            IsTrue = istrue;
            Message = message;

            Body = body;   

        }

        public RolesResponse(string v1, string v2)
        {
            V1 = v1;
            V2 = v2;
        }
    }
}
