using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.Models.Stock
{
    public class RequisitionApplication
    {
        [Key]
        public int RequisitionID { get; set; }
        public string? itemName { get; set; } = "None";
        public string? BrandName { get; set; } = "None";
        public int Quantity { get; set; } = 0;
        public string? CategoryName { get; set; } 

        // Pricing and reference
        public string? Currency { get; set; } = "KES"; // Default if not for sale
        public string? ReferenceNumber { get; set; }   // Provided during application

        // Descriptive and functional info
        public string? Description { get; set; }
        public string? stockNeed { get; set; }
        public string? Purpose { get; set; }
        public string? DeviceBeingRepaired { get; set; }

        // Requestor info
        public string? clientName { get; set; }
        public string? Department { get; set; }
        public string? Requisitioner { get; set; }
        public string? useremail { get; set; }
        public string NameToUse { get; set; } = "None";

        // Tracking and system fields
        public string? OrderNumber { get; set; }
        public string ApprovedStatus { get; set; } = "Pending";
        public string? ApprovedBy { get; set; } = "Unknown";
        public string? IssuedBy { get; set; } = "Unknown";
        public string DispatchStatus { get; set; } = "Incomplete";
        public string ApplicationStatus { get; set; } = "Pending";
        public string? RejectReason { get; set; }
        public DateTime DateRequisitioned { get; set; } = DateTime.Now;

        // Dates
        public DateTime IssuedByDate { get; set; } = DateTime.Now;
        public DateTime? ApprovedDate { get; set; }
        public DateTime? DateApproved { get; set; }
        public DateTime? DateIssued { get; set; }

        // Dispatch quantities
        public int QuantityDispatched { get; set; }
        public int OutStandingBalance { get; set; }

        // UI/UX helper field
        public string? selectedOption { get; set; }

    }
}
