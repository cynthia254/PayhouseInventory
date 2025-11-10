using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.DTOs.RegisterVms
{
    public class suspendUservm
    {
        public string useremail { get; set; }
        public string UserStatus { get; set; }
        public string StatusReason { get; set; }
        public string SuspensionReason { get; set; }
        public int Duration { get; set; }
    }
}
