using PayhouseDragonFly.CORE.Models.UserRegistration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.INFRASTRUCTURE.Services.ExtraServices
{
    public  interface IEExtraServices
    {
        Task<PayhouseDragonFlyUsers> LoggedInUser();
        
    }
}
