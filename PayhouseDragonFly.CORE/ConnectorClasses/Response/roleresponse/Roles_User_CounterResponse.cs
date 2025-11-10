using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.ConnectorClasses.Response.roleresponse
{
    public class Roles_User_CounterResponse
    {

        public int UsersAssigned { get; set; }
        public string RoleName { get; set; }

  
        public object AllUsers { get; set; }
        public Roles_User_CounterResponse(int usersassigned, string rolename, object allusers)
        {
            UsersAssigned = usersassigned;
            RoleName = rolename;

            AllUsers = allusers;
        }
        
            
        
    }
}
