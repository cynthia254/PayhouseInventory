using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.Models.Stock
{
    public class ChangeSalesStatus
    {
         
        [Key]
        public int salesId { get; set; }
        public string SalesStatus { get; set; }
        public string ReasonForSalesStatus { get; set; } = "No  reason to show";
        public DateTime DateAdded { get; set; } = DateTime.Now;
    }
}

