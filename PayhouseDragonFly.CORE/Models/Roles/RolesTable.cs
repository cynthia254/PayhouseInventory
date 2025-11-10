using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.Models.Roles
{
    public  class RolesTable
    {
        [Key]
        public int RolesID { get; set; }
        public string RoleName { get; set; }

    }
}
