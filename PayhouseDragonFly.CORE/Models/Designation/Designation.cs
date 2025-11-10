using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.Models.Designation
{
    public class Designation
    {
         
        [Key]
        public int PostionId { get; set; }
        public string PositionName { get; set; }
        public string PositionDescription { get; set; }


    
}
}
