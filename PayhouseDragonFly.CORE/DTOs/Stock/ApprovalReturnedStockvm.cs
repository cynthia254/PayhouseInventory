using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.DTOs.Stock
{
    public class ApprovalReturnedStockvm
    {
        public int Id { get; set; }
        public string ApprovalStatus { get; set; } = "Unknown";
        public string selectedOption { get; set; }
        public string RejectedReason { get; set; } = "None";
    }
}
