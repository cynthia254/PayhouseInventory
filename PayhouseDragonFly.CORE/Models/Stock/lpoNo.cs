using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.Models.Stock
{
    public class lpoNo
    {
        [Key]
        public int LpoKey { get; set; }
        public int LpoNo { get; set; }
        public DateTime DateCreated { get; set; }= DateTime.Now;
    }
}
