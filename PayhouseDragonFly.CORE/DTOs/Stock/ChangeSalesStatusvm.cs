using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.DTOs.Stock
{
    public class ChangeSalesStatusvm
    {
        public int salesId { get; set; }
        public string SalesStatus { get; set; }
        public string ReasonForSalesStatus { get; set; } = "No  reason to show";
        public DateTime DateAdded { get; set; } = DateTime.Now;
    }
}
