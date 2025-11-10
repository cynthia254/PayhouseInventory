using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.ConnectorClasses.Response
{
    public  class RegisterResponse
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public object Body { get; set; }

        public RegisterResponse( string code , string message, object body)
        {
            Code = code;
            Message = message;
            Body = body;
        }
    }
}
