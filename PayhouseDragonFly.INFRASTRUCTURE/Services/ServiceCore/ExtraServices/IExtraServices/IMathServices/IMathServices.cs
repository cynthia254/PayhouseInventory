using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.INFRASTRUCTURE.Services.IServiceCoreInterfaces.IExtraServices.IMathServices
{
    public interface IMathServices
    {
        Task<string> GenerateTokenString();
    }
}
