using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.Models.Stock
{
    public class AddPart
    {
        [Key]
        public int PartID { get; set; }
        public string PartName { get; set; }
        public string PartDescription { get; set; }
    }
}
