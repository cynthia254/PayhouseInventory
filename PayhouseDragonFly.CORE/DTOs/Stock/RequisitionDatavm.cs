using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.DTOs.Stock
{
    public class RequisitionDatavm
    {
        public string Description { get; set; }
        public string stockNeed { get; set; }
        public string clientName { get; set; }
        public string Department { get; set; }
        public string Purpose { get; set; }
        public string ReferenceNumber { get;set; }
        public string Currency { get; set; } = "KES";
        public string DeviceBeingRepaired { get; set; }
    }
}
