using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.DTOs.Stock
{
    public class EditRequisitionItemDto
    {
            public int RequisitionItemID { get; set; }
            public int Quantity { get; set; }
            public int? DiscountNumerator { get; set; }
            public int? DiscountDenominator { get; set; }
        public string EditedBy { get; set; }

        

    }
}
