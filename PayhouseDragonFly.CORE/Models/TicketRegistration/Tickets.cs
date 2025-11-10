using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.Models.TicketRegistration
{
    public class Tickets
    {
        [Key]
        public int Id { get; set; }
        public string ServiceName { get; set; }
        public string itemName { get; set; }
        public string siteArea { get; set; }
        public string clientLocation { get; set; }
        public string description { get; set; }
        public string subject { get; set; }
        public string priorityName { get; set; }
        public string TicketTitle { get; set; }
        public string ServiceIssue { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public string Status { get; set; }
        public string CreatorEmail { get; set; }
        public DateTime DateAsigned { get; set; }
        public string assignedtoNames { get; set; }
        public string ClientName { get; set; }
        public string Resolution { get; set; }
        public string Escalation { get; set; }





    }

}

