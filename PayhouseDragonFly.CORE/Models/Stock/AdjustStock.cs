using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.Models.Stock
{
    public class AdjustStock
    {
        [Key]
        public int Id { get; set; }
        public int QuantityDamaged { get; set; }
        public int ItemID { get; set; }
        public string Description { get; set; }
    }
}
