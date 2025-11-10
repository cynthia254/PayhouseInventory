using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.Models.Tickets
{
    public  class Tickets
    {
        [Key]
        public int TicketsId { get; set; }
        public string TicketTitle { get; set; }
       
        public string TicketDescriptiom { get; set; }
        public string  CreatorEmail { get; set; }
        public string TicketType { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime DateUpdated { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public string AssignedTo { get; set; }
        public string AssignedFrom { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime ClosedDate { get; set; }
        public string Resolution { get; set; }
        public string Escalation { get; set; }
  

    }
}
