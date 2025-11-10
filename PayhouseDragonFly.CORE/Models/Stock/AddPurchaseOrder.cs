using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.Models.Stock
{
    public class AddPurchaseOrder
    {
        [Key]
        public int PurchaseId { get; set; }
        public string BrandName { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public int BuyingPrice { get; set; }
        public int TotalPurchase { get; set; }
        public string SupplierName { get; set; }
        public DateTime DeliveryDate { get; set; }= DateTime.Now;
        public DateTime PurchaseDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Nothing to show here";    
        public string PurchaseStatus { get; set; } = "Nothing to show here";    
        public string ReasonforStatus { get; set; }= "Nothing to show here";
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public string Currency { get; set; }


    }
}
