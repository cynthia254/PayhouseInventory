using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.Models.Stock
{
    public class AddReturnedStatus
    {
        [Key]
        public int ReturnedID { get; set; }
        public string ReturnedStatus { get; set; }
        public string ReturnedDescription { get; set; }
    }
}
