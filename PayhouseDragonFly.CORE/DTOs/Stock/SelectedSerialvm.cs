using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.DTOs.Stock
{
    public class SelectedSerialvm
    {
        public int IssueID { get; set; }
        public string IMEII1 { get; set; } = "N/A";
        public string IMEI2 { get; set; } = "N/A";
        public List<string> SerialNumbers { get; set; } = new List<string>();
        public string SerialStatus { get; set; } = "Issued";
    }
}
