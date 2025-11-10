using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.INFRASTRUCTURE.Services.ExtraServices.RoleChecker
{
    public interface IRoleChecker
    {
        Task<int> Returnedrole();
    }
}
