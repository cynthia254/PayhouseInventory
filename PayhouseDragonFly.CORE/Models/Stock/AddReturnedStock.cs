using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.Models.Stock
{
    public class AddReturnedStock
    {
        [Key]
        public int ReturnedId { get; set; }
        public string ReturnedStatus { get; set; }
        public string ReturnReason { get; set; }
        public int ReturnedQuantity { get; set; }

        public string BrandName { get; set; }
        public string ItemName { get; set; }
        public DateTime DateReturned { get; set; } = DateTime.Now;
    }
}
