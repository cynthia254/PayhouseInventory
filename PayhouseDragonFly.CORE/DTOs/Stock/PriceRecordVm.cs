using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.DTOs.Stock
{
    public class PriceRecordVm
    {
            public string ItemId { get; set; }
            public decimal SellingPrice { get; set; }
            public string Currency { get; set; }
            public DateTime EffectiveFrom { get; set; }
            public DateTime? EffectiveTo { get; set; }
        public string AddedBy { get; set; }
        

    }
}
