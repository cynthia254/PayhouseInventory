using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.ConnectorClasses.Response.BseResponse
{
    public  class BaseResponse
    {
        public string Code { get; set; }

        public string Message { get; set; }
        public object Body { get; set; }

        public BaseResponse(string code, string message, object body)
        {
            Code=code;
            Message=message;
            Body=body;

        }
      
       
    }
}
