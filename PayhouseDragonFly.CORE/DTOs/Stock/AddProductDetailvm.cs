using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.DTOs.Stock
{
    public class AddProductDetailvm
    {
        public string SerialNumber { get; set; }
        public string IMEI1 { get; set; }
        public string IMEI2 { get; set; }
        public string BrandName { get; set; } = "nothinh";
        public string ItemName { get; set; } = "nothing";
        public int BatchID { get; set; }
        public int Product_No { get; set; } = 0;
        public int invoiceItemId { get; set; }
        public string  reference_number { get; set; }
        public string ProductStatus { get; set; } = "INCOMPLETE";
        public string BatchNumber { get; set;  }

    }
}
