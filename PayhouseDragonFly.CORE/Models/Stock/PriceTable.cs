using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.Models.Stock
{
    public class PriceTable
    {
        [Key]
            public int ItemPriceID { get; set; }
            public string ItemId { get; set; }
            public decimal SellingPrice { get; set; }
            public string Currency { get; set; }
            public DateTime EffectiveFrom { get; set; }
            public DateTime? EffectiveTo { get; set; }
        public string AddedBy { get; set; }  // New field
        public DateTime DateAdded { get; set; } = DateTime.Now;  // New field
        public string Status { get; set; } = "Active"; //Active,Expired,Pending
        public bool IsDeleted { get; set; }

        // New fields
        public DateTime? DateUpdated { get; set; } = null;
        public DateTime? DateDeleted { get; set; } // Correct

        public string UpdatedBy { get; set; } = "null";
        public string ReactivatedBy { get; set; } = "null";
        public DateTime? DateActivated { get; set; } = null;
        public DateTime? DateReactivated { get; set; } = null;
        public string ActivatedBy { get; set; } = "null";
        public string DeletedBy { get; set; } = "null";
    }

    }

