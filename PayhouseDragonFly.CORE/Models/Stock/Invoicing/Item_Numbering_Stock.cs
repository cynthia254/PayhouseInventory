using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.Models.Stock.Invoicing
{
    public  class Item_Numbering_Stock
    {
        [Key]
        public int invoice_numbering_id  { get; set; }
        public int Product_No { get; set; }
        public string Invoice_quantity_Id { get; set; }
        public string Status { get; set; }

    }
}
