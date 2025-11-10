using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.DTOs.userStatusvm
{
    public class userStatusvm
    {
        public string userId { get; set; }
        public bool UsertActive { get; set; } = false;
        public string StatusDescription { get; set; } = "Currently  active";
        public string ReasonforStatus { get; set; } = "No  reason to show";
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
