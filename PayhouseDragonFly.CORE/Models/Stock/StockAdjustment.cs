using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.Models.Stock
{
    public class StockAdjustment
    {
        [Key]
        public int Adjustedid { get; set; }
        public string SerialNumber { get; set; }
        public string ConditionStatus { get; set; } = "Okay";
        public string BatchNumber { get; set; }
        public int QuantityDamaged { get; set; }
        public string Description { get; set; }
        public int ItemID { get; set; }
        public string CategoryName { get; set; }
        public int TotalDamaged { get; set; }
        public DateTime DateCreated { get; set; }= DateTime.Now;
        public int TotalQuantityDamaged { get; set;  }
        public string ItemName { get; set; }
        public string brandName { get; set; }
    }
}
