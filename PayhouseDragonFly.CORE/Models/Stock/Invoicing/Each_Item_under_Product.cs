using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.Models.Stock.Invoicing
{
    public  class Each_Item_under_Product
    {
        [Key]
        public int Each_Item { get; set; }
        public string Type { get; set; }
        public string Serial_Number { get; set; }
        public int IMEI_no { get; set; }
        public int item_Quantity_Id { get; set; }
    }
}
