using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.Models.Stock
{
    public class AddDeliveryNote
    {
        [Key]
        public int DeliveryId { get; set; }
        public int ItemID { get; set; }
        public string DeliveryNumber { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int BatchQuantity { get; set; }
        public string AirWayBillNumber { get; set; }
        public string MeansOfDelivery { get; set; }
        public string BatchNumber { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public string CategoryName { get; set; }
        public string ProductStatus { get; set; }
        public string Reference_Number { get; set; }
        public int TotalQuantity { get; set; }
        public int quantityDamaged{get;set;}
        public string PONumber { get; set; }
        public int OKQuantity { get; set; }
        public int TotalDamages { get; set; }
        public string ApprovalStatus { get; set; } = "Unknown";
        public string selectedOption { get; set; } = "NONE";
        public DateTime AprrovedDate { get; set; } = DateTime.Now;
        public string RejectedReason { get; set; }="Unknown";
        public string ApprovedBy { get; set; } = "Unknown";
        public int TotalClosed { get; set; }
        public string ItemName { get; set; }
        public string BrandName { get; set; }
        public int totalDeliveredForAllItems { get; set; }
        //public int ExcessQuantity { get; set; }
        public int ExcessQuantityInBatch { get; set; }
    }
}
