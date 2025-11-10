using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.Models.Stock
{
    public class IssueProcess
    {
        [Key]
        public int IssueId { get; set; }
        public string IssuedBy { get; set; } = "Unknown";
        public string IssueStatus { get; set; } = "Issued";
        public int Quantity { get; set; }
        public int FormID { get; set; }
        public DateTime DateIssued { get; set; } = DateTime.Now;
        public string ItemName { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
        public string ClientName { get; set; }
        public string Requisitiioner { get; set; }
        public string StockNeed { get; set; }

        //public string SerialNumber { get; set; } = "N/A";
        //public string SerialStatus { get; set; }
    }
}
