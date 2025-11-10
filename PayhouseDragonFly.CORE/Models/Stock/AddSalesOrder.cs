using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.Models.Stock
{
    public class AddSalesOrder
    {
        [Key]
        public int SalesId { get; set; }
        public string BrandName { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public string CustomerName { get; set; }
        public string Status { get; set; } = "Nothing to show here";
        public int SellingPrice { get; set; }

        public int TotalSales { get; set; }

        public string SalesStatus { get; set; } = "Nothing to show here";
        public string ReasonForSalesStatus { get; set; } = "No  reason to show";
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public string Comments { get; set; }
        public string Department { get; set; }

    }
}
