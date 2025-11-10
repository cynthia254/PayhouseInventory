using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PayhouseDragonFly.CORE.Models.UserRegistration;
using PayhouseDragonFly.INFRASTRUCTURE.DataContext;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.INFRASTRUCTURE.Services.ExtraServices
{
    public class ExtraServices: IEExtraServices
    {
        private readonly IHttpContextAccessor _httpcontextaccessor;
        private readonly DragonFlyContext _context;
        public ExtraServices(
            IHttpContextAccessor httpcontextaccessor,
            DragonFlyContext context

            )
        {
            _httpcontextaccessor = httpcontextaccessor;
            _context = context;
        }

        public async Task<PayhouseDragonFlyUsers> LoggedInUser()
        {
            try
            {
                var currentuserid = _httpcontextaccessor.HttpContext.User.Claims.Where(x => x.Type == "Id").Select(p => p.Value).FirstOrDefault();

                var loggedinuser = await _context.PayhouseDragonFlyUsers.Where(x => x.Id == currentuserid).FirstOrDefaultAsync();

                return loggedinuser;
            }
            catch (Exception ex)
            {

                foreach (var error in ex.Message)
                {

                    return new PayhouseDragonFlyUsers { AnyMessage = error.ToString() };
                }
            }

            return new PayhouseDragonFlyUsers { AnyMessage = "Nothing to show here" };

        }
    }
}

