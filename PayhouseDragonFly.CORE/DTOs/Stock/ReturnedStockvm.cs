using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.DTOs.Stock
{
    public class ReturnedStockvm
    {
        public string ReasonForReturn { get; set; }
        public string ReturnedBy { get; set; }
        public string ReturnedCondition { get; set; }
        public int ReturnedQuantity { get; set; }
        public string SerialNumber { get; set; } = "None";
        public int IssuedId { get; set; }
        public string FaultyDescription { get; set; } = "None";
        public int FaultyQuantity { get; set; } = 0;
        public string SerialFaulty { get; set; } = "None";
        public string CategoryName { get; set; }
    }
}
