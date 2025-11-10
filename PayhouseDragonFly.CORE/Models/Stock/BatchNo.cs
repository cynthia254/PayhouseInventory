using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.Models.Stock
{
    public class BatchNo
    {
        [Key]
        public int BatchKey { get; set; }
        public int BatchNumber { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}
