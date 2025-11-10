using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PayhouseDragonFly.CORE.Models.UserRegistration;
using PayhouseDragonFly.INFRASTRUCTURE.DataContext;
using PayhouseDragonFly.INFRASTRUCTURE.Services.IServiceCoreInterfaces.IExtraServices;

namespace PayhouseDragonFly.INFRASTRUCTURE.Services.ServiceCore.ExtraServices.LoggedIuserServices
{
    public  class LoggeinUserServices: ILoggeinUserServices

    {
        private readonly IServiceScopeFactory _scopeFactory;
        private  readonly IHttpContextAccessor _httpcontextaccessor;
        private readonly DragonFlyContext _context;
     
        public LoggeinUserServices(IHttpContextAccessor httpcontextaccessor,
            DragonFlyContext context,
             IServiceScopeFactory scopeFactory)
        {
            _httpcontextaccessor= httpcontextaccessor;
            _context= context;
            _scopeFactory= scopeFactory;
        }

        public async Task<PayhouseDragonFlyUsers> LoggedInUser()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                var currentuserid =
                    _httpcontextaccessor
                        .HttpContext
                        .User
                        .Claims
                        .Where(x => x.Type == "Id")
                        .Select(p => p.Value)
                        .FirstOrDefault();

                var loggedinuser =
                    await scopedcontext
                        .PayhouseDragonFlyUsers
                        .Where(x => x.Id == currentuserid)
                        .FirstOrDefaultAsync();

                return loggedinuser;
            }
        }
    }
}
