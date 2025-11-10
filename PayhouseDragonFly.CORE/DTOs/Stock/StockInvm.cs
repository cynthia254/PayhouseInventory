using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.DTOs.Stock
{
    public class StockInvm
    {
       // public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set;}
      //  public string LPONumber { get; set; }
        public DateTime LPODate { get; set; }
       // public DateTime StockInDate { get; set; }=DateTime.Now;
        //public string Status { get; set; }
        public string SupplierName { get; set; }  



    }
}
