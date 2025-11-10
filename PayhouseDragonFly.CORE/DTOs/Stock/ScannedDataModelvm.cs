using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.DTOs.Stock
{
    public class ScannedDataModelvm
    {
        public string SerialNumber { get; set; }
        public string IMEI1 { get; set; }
        public string IMEI2
        {
            get; set;
        }
    }
}
