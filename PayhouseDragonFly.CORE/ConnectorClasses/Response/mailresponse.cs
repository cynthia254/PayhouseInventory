using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.ConnectorClasses.Response
{
    public class mailresponse
    {

        public bool IsSent { get; set; }
        public string Message { get; set; }
        
        public mailresponse(bool issent,string message)
        {
            IsSent = issent;  
            Message = message;
            
        }
    }

}
