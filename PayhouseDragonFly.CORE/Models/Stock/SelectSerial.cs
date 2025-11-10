using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.Models.Stock
{
    public class SelectSerial
    {
        [Key]
        public int Id { get; set; }
        public int IssueID { get; set; }
        public string SerialNumber { get; set; } = "N/A";
        public int IssuedIID { get; set; }
        public string SerialStatus { get; set; } = "Not Issued";
        public string ItemName { get; set; }
        public string BrandName { get; set; }
        public string IMEII1 { get; set; } = "N/A";
        public string IMEI2 { get; set; } = "N/A";
        public string clientName { get; set; }
        public string StockNeed { get; set; }
        public string Requisitioner { get; set; }
        public string CategoryName { get; set; }
       public string IssuedBy { get; set; }
        public string IssueStatus { get; set; } = "Issued";
        public DateTime DateIssued { get; set; } = DateTime.Now;
        public int QuantityOrdered { get; set; }
        public int QuantityDispatched { get; set; }
        public string Reason { get; set; }
        public string Comments { get; set; } = "Working OK";
        public int OutStandingBalance { get; set; }
        public string DispatchStatus { get;set; }
        public int TotalStockOut { get; set; }
        public int TotalQuantityDispatchedForItem { get; set; }
        public int TotalQuantityDispatchedForAnId { get; set; }
        public string OrderNumber { get; set; }
        public string OrderNumbers { get; set; } = "None";
        public string QuantityDispatchStatus { get; set; } = "Incomplete";
        public string NameToUse { get; set; } = "None";
    }
}
