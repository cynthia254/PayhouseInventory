using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.ConnectorClasses.Response.StockResponse
{
    public class StockResponse
    {
        public Boolean isTrue { get; set; }
        public string Message { get; set; }
        public object Body { get; set; }



        public StockResponse(Boolean isTrue,string Message,object Body) 
        { 
            this.isTrue = isTrue;
            this.Message = Message;
            this.Body = Body;
        }
    }
}
