using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.Models.Stock
{
    public class AddProductDetailsmodel
    {
        [Key]
        public int ProductID { get; set; }
        public string SerialNumber { get; set; }
        public int IMEI1 { get; set; }
        public int IMEI2 { get; set; }
        public int ProductItemID { get; set; }
        public int invoiceItemId { get; set; }
    }
}
