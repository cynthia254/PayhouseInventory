using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.DTOs.Stock
{
    public class ApplyRequistionvm
    {
        public string itemName { get; set; }
        public string BrandName { get; set; }
        public string Description { get; set; }
        public string stockNeed { get; set; }
        public int Quantity { get; set; }
        public string DeviceBeingRepaired { get; set; } = "N/A";
        public string clientName { get; set; }
        public string Department { get; set; }
        public string Purpose { get; set; }
        public string Requisitioner { get; set; } = "Unknown";
        public string IssuedBy { get; set; } = "Unknown";
        public DateTime IssuedByDate { get; set; } = DateTime.Now;
        public string ApprovedBy { get; set; } = "Unknown";
        public string ApprovedStatus { get; set; } = "Unknown";
        public string RejectReason { get; set; }="No reason";
    }
}
