using PayhouseDragonFly.CORE.Models.UserRegistration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.INFRASTRUCTURE.Services.IServiceCoreInterfaces.IExtraServices
{
    public interface ILoggeinUserServices
    {
       Task<PayhouseDragonFlyUsers> LoggedInUser();
    }
}
