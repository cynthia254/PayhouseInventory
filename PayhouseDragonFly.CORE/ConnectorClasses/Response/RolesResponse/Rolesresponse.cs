using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.ConnectorClasses.Response.RolesResponse
{
    public class Rolesresponse
    {
        public bool IsTrue { get; set; }
        public string Message { get; set; }
        public object Body { get; set; }

        public Rolesresponse(bool istrue, string message, object body)
        {
            IsTrue= istrue;
            Message= message;
            Body= body;

        }
    }
}
