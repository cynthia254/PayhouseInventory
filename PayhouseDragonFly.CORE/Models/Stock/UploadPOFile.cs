using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.Models.Stock
{
    public class UploadPOFile
    {
        [Key]
        public int ID { get; set; }
        public string PONumber { get; set; }
        public int Warranty { get; set; }
        public DateTime WarrantyStartDate { get; set; }
        public string Amount { get; set; }= "Unknown";
        public string Reference_Number { get; set; }
        public string Rate { get; set; } = "Unknown";
        public string ItemName { get; set; }
       public string BrandName { get; set; }
        public int ReOrderLevel { get; set; }
        public string CategoryName { get; set; }
        public int Quantity { get; set; }
        public DateTime WarrantyEndDate { get; set; }
        public int AjustedQuantity { get; set; }
        public string Status { get; set; } = "Not yet";
        public string WarrantyStatus { get; set; } = "None";
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public int OpeningStock { get; set; }
        public int AvailableStock { get; set; }
        public int StockOut { get; set; }
        public string FileName { get; set; } = "unknown";
        public int TotalStockIn { get; set; }
        public int ActualQuantity { get; set; }
        public string ProductStatus { get; set; }
        public int TotalDelivered { get; set; }
        public int OutstandingQuantity { get; set; }
        public int OKQuantity { get; set; }
        public int TotalClosed { get; set; }
        public int UnitPrice { get; set; }
        public string Currency { get; set; }
        public string UpdatedBy { get; set; } = "Nothing to show here";
        public DateTime UpdatedOn { get; set; } = DateTime.Now;
        public int TotalUnitPrice { get; set; }
        public string CaptureStatus { get; set; } = "Incomplete";
        public int TotalDamages { get; set; }
        public int totalDeliveredForAllItems { get; set; }
        public int Year { get; set; } = 0;
        public int Month { get; set; }=0;
        public int Value { get; set; } = 0;
        //public string Status { get; set; } = "Incomplete";
        //public string Reference_Number { get; set; }

    }
}
