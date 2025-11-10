using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.Models.TicketRegistration
{
    public class serviceissue
    {
        [Key]
        public int serviceId { get; set; }
        public string ServiceIssue { get; set; }
        public string serviceName { get; set; }
        public string Level { get; set; }

    }
}
