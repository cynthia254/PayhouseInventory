using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.DTOs.RegisterVms
{
    public  class RegistrationOutputVm
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Salutation { get; set; }
        public string AdditionalInformation { get; set; }
        public string BusinessUnit { get; set; }
        public string UserStatus { get; set; }
        public string Site { get; set; }
        public string County { get; set; }
        public string DepartmentName { get; set; }
        public string Address { get; set; }
        public string ClientName { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Email { get; set; }
        public DateTime VerificationTime { get; set; }
        public string VerificationToken { get; set; }
        public string AnyMessage { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public int RoleId { get; set; }

        public string Position { get; set; }
        public string DepartmentDescription { get; set; }
        public string StatusReason { get; set; }
        public string UserId { get; set; }
        public string? StatusDescription { get; set; }

        public string? ReasonforStatus { get; set; }

        public string UserActiveMessage { get; set; }
        public string PositionName { get; set; }
        public string PositionDescription { get; set; }
        public bool Checker { get; set; }
        public bool Issuer { get; set; }


    }
}
