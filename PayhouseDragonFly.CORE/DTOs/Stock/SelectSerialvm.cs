using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.DTOs.Stock
{
    public class SelectSerialvm
    {
        public int IssueID { get; set; }
        public string SerialStatus { get; set; } = "Not Issued";
        public int QuantityDispatched { get; set; }
        public string Reason { get; set; }
    }
}
