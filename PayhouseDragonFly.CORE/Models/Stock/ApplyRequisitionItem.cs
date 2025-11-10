using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.Models.Stock
{
    public class ApplyRequisitionItem
    {
        [Key]
        public int RequisitionItemID { get; set; }
        // Foreign key to the header
        public int RequisitionId { get; set; }
        public string ItemID { get; set; }
        public string Requisitooner { get; set; }
        public string Status { get; set; } = "Incomplete";

        [Required]
        public string ItemName { get; set; }

        [Required]
        public string BrandName { get; set; }

        [Required]
        public int Quantity { get; set; }

        public string ReferenceNumber { get; set; }
        public string CategoryName { get; set; } = "None";

        public decimal UnitPrice { get; set; }
        public string EditedBy { get; set; } ="None";
        public DateTime? DateEdited { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsApprover { get; set; } = false;
        public string Reason { get; set; } = "None";
        public string Currency { get; set; }
        public string RequisitionName { get; set; }
        public int QuantityDispatched { get; set; }
        public int OutstandingBalance { get; set; }

        // Discount optional
        public int? DiscountNumerator { get; set; }

        public int? DiscountDenominator { get; set; }
        public string DispatchComment { get; set; } = "None";


        public DateTime DateAdded { get; set; } = DateTime.Now;
    }
}
