using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.Models.Stock
{
    public class selectedSetialNumber
    {
        [Key]
        public int SerialID { get; set; }
        public int IssueID { get; set; }
        public string IMEII1 { get; set; } = "N/A";
        public string IMEI2 { get; set; } = "N/A";
        public string SerialNumber { get; set; }= "N/A";
        public string SerialStatus { get; set; }
        public string ReferenceNumber { get; set; }
        public string ClientName { get; set; } = "None";
        public DateTime DateUpdated { get; set; }
        public int IssuedNo { get; set; }
    }
}
