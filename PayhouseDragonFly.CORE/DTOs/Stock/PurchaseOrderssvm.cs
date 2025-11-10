using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.DTOs.Stock
{
    public class PurchaseOrderssvm
    {
        public string PONumber { get; set; }
        public DateTime PODate { get; set; }
        public string Vendor { get; set; }
    }

}
