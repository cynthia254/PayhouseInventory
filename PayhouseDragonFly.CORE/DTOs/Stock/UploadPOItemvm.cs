using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.DTOs.Stock
{
    public class UploadPOItemvm
    {
       
        public string ItemName { get; set; }
        public string Amount { get; set; }
        public string Rate { get; set; }
        public string Quantity { get; set; }
    }
}
