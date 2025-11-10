using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.Models.Stock
{
    public class InvoiceNo
    {
        [Key]
        public int InvoiceKey { get; set; }
        public int InvoieNo { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}
