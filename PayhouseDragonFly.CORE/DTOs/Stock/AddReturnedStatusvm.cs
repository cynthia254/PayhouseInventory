using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.DTOs.Stock
{
    public class AddReturnedStatusvm
    {
        public int ReturnedID { get; set; }
        public string ReturnedStatus { get; set; }
        public string ReturnedDescription { get; set; }

    }
}
