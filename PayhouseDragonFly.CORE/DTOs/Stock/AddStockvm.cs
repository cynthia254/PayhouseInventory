using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.DTOs.Stock
{
    public class AddStockvm
    {
        public string BrandName { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public int ReOrderLevel { get; set; }
        public int BuyingPrice { get; set; }
        public int SellingPrice { get; set; }
        public int AvailableStock { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public string Currency { get; set; }
        public int StockInTransit { get; set; }
 
        public string SalesCurrency { get; set; }
        public int OpeningStock { get; set; }
        public string ReorderRequired { get; set; } = "Nothing to show here";
        public int StockOut { get; set; }
        public int TotalReturnedStock { get; set; }
        public int StockIn { get; set; }




    }
}
