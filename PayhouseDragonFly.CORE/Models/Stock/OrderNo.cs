using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.Models.Stock
{
    public class OrderNo
    {
        [Key]
        public int OrderKey { get; set; }
        public int OrderNumber { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public string ClientName { get; set; }
    }

}
