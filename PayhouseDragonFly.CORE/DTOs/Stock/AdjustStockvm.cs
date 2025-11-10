using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.DTOs.Stock
{
    public class AdjustStockvm
    {
        public int QuantityDamaged { get; set; }
        public string Description { get; set; }
        public int ItemID { get; set; }
    }
}
