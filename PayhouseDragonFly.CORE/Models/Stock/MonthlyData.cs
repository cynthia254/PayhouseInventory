using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.Models.Stock
{
    public class MonthlyData
    {
        [Key]
       public int id { get; set; }
            public int Year { get; set; }
            public int Month { get; set; }
            public int Value { get; set; }
        

    }
}
