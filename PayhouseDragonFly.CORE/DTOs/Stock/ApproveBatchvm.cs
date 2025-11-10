using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.DTOs.Stock
{
    public class ApproveBatchvm
    {
        public string BatchNumber { get; set; }
        public string ApprovalStatus { get; set; } = "Unknown";
        public string selectedOption { get; set; }
        public DateTime AprrovedDate { get; set; } = DateTime.Now;
        public string RejectedReason { get; set; } = "None";
        public string ApprovedBy { get; set; } = "Unknown";
        public int ItemID { get; set; }
        public int closedQuantity { get; set;}
    }
}
