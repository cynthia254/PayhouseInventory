using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.DTOs.Stock
{
    public class MonthlyRecordvm
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int TotalDelivered { get; set; }
        public int TotalIssued { get; set; }
        public int TotalDamaged { get; set; }

    }
}
