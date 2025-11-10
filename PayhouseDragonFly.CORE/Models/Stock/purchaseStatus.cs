using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.Models.Stock
{
    public class purchaseStatus
    {
        [Key]
        public int purchaseId { get; set; } 
        public string PurchaseStatus { get; set; }
        public string ReasonforStatus { get; set; } = "No  reason to show";
        public DateTime DateAdded { get; set; } = DateTime.Now;
    }
}
