using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.DTOs.Stock
{
    public class editPartVm
    {
        public int PartID { get; set; }  // Required to locate the part in DB

        public string PartName { get; set; } = string.Empty;

        public string PartDescription { get; set; } = string.Empty;
    }
}

