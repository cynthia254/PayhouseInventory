using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.Models.Roles
{
    public class OtherRoles
    {
        public int otherRolesID { get; set; }
        public int RoleID { get; set; } = 0;
        public string userID { get; set; } = "Unknown";
    }
}
