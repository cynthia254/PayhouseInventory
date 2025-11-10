using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.Models.Stock.Invoicing
{
    public  class Invoice_Item_Quantity
    {
        [Key]
        public int Invoic_item_quantity_id { get; set; }
        public string  Invoce_No { get; set; }
        public int Quantity { get; set; }
        public string  Type { get; set; }
        public string Invoice_quantity_Unique_id { get; set; }
    }
}
