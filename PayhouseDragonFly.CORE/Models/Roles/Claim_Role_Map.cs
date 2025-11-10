using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.Models.Roles
{
    public  class Claim_Role_Map
    {
        [Key]
        public int ClaimRoleId { get; set; }
        public int RoleId { get; set; }
        public int ClaimId { get; set; }
    }
}
