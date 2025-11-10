using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.DTOs.Stock
{
    public class IssueProcessvm
    {
        public string IssuedBy { get; set; } = "Unknown";
        public int FormID { get; set; }
        public string IssueStatus { get; set; } = "Waiting to be issued";
        public int Quantity { get; set; }
        public DateTime DateIssued { get; set; }=DateTime.Now;
        public string ItemName { get; set; } = "nothing to show here";
        public string SerialNumber { get; set; } = "N/A";

    }
}
