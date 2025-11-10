using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.DTOs.Stock
{
    public class EditItemvm
    {
        public int? ItemId { get; set; }
        public string? ItemName { get; set; }
        public string? Category { get; set; }
        public string? BrandName { get; set; }
        public int ReOrderLevel { get; set; }
        public string? Currency { get; set; }
        public int IndicativePrice { get; set; }
        public string? ItemDescription { get; set; }

    }
}
