using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PayhouseDragonFly.INFRASTRUCTURE.DataContext;
using PayhouseDragonFly.INFRASTRUCTURE.Services.IServiceCoreInterfaces.IExtraServices;
using PayhouseDragonFly.INFRASTRUCTURE.Services.RoleServices;

namespace PayhouseDragonFly.INFRASTRUCTURE.Services.ExtraServices.RoleChecker
{
    public class RoleChecker:IRoleChecker
    {
        private readonly IRoleServices _roleservice;
        private readonly DragonFlyContext _context;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILoggeinUserServices _loggedinuser;
        public RoleChecker(IRoleServices roleservice,
            ILoggeinUserServices loggedinuser,
            DragonFlyContext context,
            IServiceScopeFactory scopeFactory
            ) 
        {
            _roleservice = roleservice;
            _loggedinuser = loggedinuser;
            _context = context;
            _scopeFactory = scopeFactory;


        }

        public async Task<int> Returnedrole()
        {
            try
            {
                var rolereturn = _roleservice.Roleschecker();
                var cureentlyloggedinuser = await _loggedinuser.LoggedInUser();
                using (var scope = _scopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var userrole = await scopedcontext.RolesTable.Where(t => t.RolesID == cureentlyloggedinuser.RoleId).FirstOrDefaultAsync();

                    if(userrole == null) 
                    {
                        return 0;                   
                    }


                    else if (userrole.RolesID == 1)
                    {
                        return 1;
                    }

                    else if(userrole.RolesID == 2)
                    {
                        return 2;
                    }

                    else if (userrole.RolesID == 3)
                    {
                        return 3;
                    }

                    else if  (userrole.RolesID == 4)
                    {

                        return 4;
                    }
                    return 0000000000;
                }


            }
            catch (Exception ex)
            {
                return 0;
            }
        }


        
    }
}
