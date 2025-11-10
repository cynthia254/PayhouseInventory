using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.DTOs.Stock
{
    public class AddRequisitionItemDto
    {
        public string RequisitionId { get; set; }
        public string ItemID { get; set; }
        public string ItemName { get; set; }
        public string BrandName { get; set; }
        public int Quantity { get; set; }

        // Optional discount
        public int? DiscountNumerator { get; set; }
        public int? DiscountDenominator { get; set; }
        public string ReferenceNumber { get; set; }
    }
}
