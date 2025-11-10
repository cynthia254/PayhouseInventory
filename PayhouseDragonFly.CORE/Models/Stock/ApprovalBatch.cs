using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.Models.Stock
{
    public class ApprovalBatch
    {
        [Key]
        public int ID { get; set; }
        public int itemID { get; set; }
        public int ClosedQuantity { get; set; }
        public string selectedOption { get; set; }
        public string RejectedReason { get; set; }
        public string BatchNumber { get; set; }
        public int TotalClosed { get; set; }
        public string BrandName { get; set; }
        public string ItemName { get; set; }
        public DateTime DateApproved { get; set; }=DateTime.Now;
        public int TotalAvalialbeStock { get; set; }
    }
}
