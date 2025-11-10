using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.Models.Stock
{
    public class ApprovalPODelivery
    {
        [Key]
        public int id { get; set; }
        public string PONumber { get; set; }
        public string ApprovalStatus { get; set; } = "Unknown";
        public string selectedOption { get; set; }
        public DateTime AprrovedDate { get; set; } = DateTime.Now;
        public string RejectedReason { get; set; }
        public string ApprovedBy { get; set; } = "Unknown";
    }
}
