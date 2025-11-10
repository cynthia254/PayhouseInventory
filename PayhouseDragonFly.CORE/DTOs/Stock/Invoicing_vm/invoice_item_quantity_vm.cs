using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.DTOs.Stock.Invoicing_vm
{
    public  class invoice_item_quantity_vm
    {
        public string Invoce_No { get; set; }
        public int Quantity { get; set; }
        public string Type { get; set; }
    }
}
