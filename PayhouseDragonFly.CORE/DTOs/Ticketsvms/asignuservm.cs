using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.DTOs.Ticketsvms
{
    public  class asignuservm
    {
        public int ticketid { get; set; }
        public  string status { get; set; }
        public Guid userId { get; set; }

    }
}
