using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.DTOs.RegisterVms
{
    public  class RegisterVms
    {
        public string? FirstName { get; set; } 
        public string? LastName { get; set; }
         
        public string? DepartmentName { get; set; } 
        public string? Email { get; set; } 
        public string? PhoneNumber { get; set; } 
        public string? Address { get; set; } 
        public string? Salutation { get; set; }
        public string? AdditionalInformation { get; set; }
        public string? BusinessUnit { get; set; } 
        public string? Site { get; set; }
   


        public string? Password { get; set; } 
        public string? Position { get; set; }
        public string? EditorId { get; set; }

    }
}
