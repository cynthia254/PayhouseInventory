using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.DTOs.Stock
{
    public class EditPurchasevm
    {
        public string? BrandName { get; set; }
        public string? ItemName { get; set; }
        public int? Quantity { get; set; }
        public string? SupplierName { get; set; }
        public int? PurchaseId { get; set; }
    }
}
