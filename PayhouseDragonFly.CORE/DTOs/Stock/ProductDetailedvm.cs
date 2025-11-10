using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.DTOs.Stock
{
    public class ProductDetailedvm
    {
        public string SerialNumber { get; set; }
        public int IMEI1 { get; set; }
        public int IMEI2 { get; set; }
        public int ProductItemID { get; set; }
    }
}
