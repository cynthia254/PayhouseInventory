using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.DTOs.Stock
{
    public class AllProductNumberingm
    {
        public int ProductNumberID { get; set; }
        public string Type { get; set; }
        public string Reference_Number { get; set; }
        public string Status { get; set; }
        public int NumberValue { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}
