using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.Models.Roles
{
    public  class RoleClaimsTable
    {
        [Key]
        public int RolesClaimsTableId { get; set; }
        public string ClaimName { get; set; }
    }
}
