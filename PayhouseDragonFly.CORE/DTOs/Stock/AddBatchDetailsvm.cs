using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.DTOs.Stock
{
    public class AddBatchDetailsvm
    {
       
        public string CategoryName { get; set; } = "Nothing to show here";
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public int UnitPrice { get; set; }
        public string Currency { get; set; }
        public int Warranty { get; set; }
        public string UpdatedBy { get; set; }="Nothing to show here";
        public DateTime UpdatedOn { get; set; } = DateTime.Now;
        public DateTime WarrantyStartDate { get; set; }
        public string Status { get; set; } = "InComplete";
        public string InvoiceNumber { get; set; }
        public string BrandName { get; set; }
    }
}
