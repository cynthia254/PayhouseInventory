using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.DTOs.EmaillDtos
{
    public class emailbody
    {

        public string ToEmail { get; set; }
        public string UserName { get; set; }
        public string PayLoad { get; set; }
        public string Password { get; set; }
    }
}
