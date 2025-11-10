using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.Models.departments
{
    public class Departments
    {
        [Key]
        public int Departnmentid { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentDescription { get; set; }
       

    }
}
