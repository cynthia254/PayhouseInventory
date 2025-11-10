using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.DTOs.Stock
{
    public class StockAdjustvm
    {
        public string SerialNumber { get; set; } = "N/A";
        public string ConditionStatus { get; set; } = "Okay";
        public string BatchNumber { get; set; }
        public int QuantityDamaged { get; set; }
        public string Description { get; set; }
        public int ItemID { get; set; }
    }
}
