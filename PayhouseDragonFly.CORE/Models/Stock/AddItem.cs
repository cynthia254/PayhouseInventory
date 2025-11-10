using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.Models.Stock
{
    public class AddItem
    {
        [Key]
        public int ItemID { get; set; }
             public string ItemName { get; set; }
        public string Category { get; set; }
        public string BrandName { get; set; }
        public int ReOrderLevel { get; set; }
        public string Currency { get; set; }
        public int IndicativePrice { get; set; }
        public string ItemDescription { get; set; }
    }
}
