using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.Models.statusTable
{
    public  class UserStatusTable
    {
        [Key]
        public int userstatusId { get; set; }
        public string userId { get; set; }
        public bool UsertActive { get; set; } = false;
        public DateTime CreatedOn { get; set; }
        public string StatusDescription { get; set; } = "Currently  active";
        public string ReasonforStatus { get; set; } = "No  reason to show";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Totaldays { get; set; } = 0;

    }
}
