using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.Models.Stock
{
    public class ProductNumbering
    {
        [Key]
        public int ProductNumberID { get; set; }
        public string Type { get; set; }
        public string Reference_Number { get; set; }
        public string Status { get; set; }
        public int NumberValue { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}
