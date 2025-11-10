using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.DTOs.Stock
{
    public class PurchaseStatusvm
    {
        public int PurchaseId { get; set; }
        public string PurchaseStatus { get; set; }
        public string ReasonforStatus { get; set; } = "No  reason to show";
        public DateTime DateAdded { get; set; }=DateTime.Now;
    }
}
