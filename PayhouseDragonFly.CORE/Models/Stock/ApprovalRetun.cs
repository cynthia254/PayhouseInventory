using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.Models.Stock
{
    public class ApprovalRetun
    {
        [Key]
        public int AprrovedID { get; set; }
        public int Id { get; set; }
        public string ApprovalStatus { get; set; } = "Unknown";
        public string selectedOption { get; set; }
        public DateTime AprrovedDate { get; set; } = DateTime.Now;
        public string RejectedReason { get; set; } = "None";
        public string ApprovedBy { get; set; } = "Unknown";
        public int ReturnedQuantity { get; set; }
        public string ReturnedStatus { get; set; } 
        public string SerialNumber { get; set; }
        public string ItemName { get; set; }
        public string BrandName { get; set; }
        public int TotalReturnedQuantity { get; set; }
    }
}
