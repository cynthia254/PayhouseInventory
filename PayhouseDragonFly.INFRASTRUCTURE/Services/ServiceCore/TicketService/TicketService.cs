using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PayhouseDragonFly.CORE.ConnectorClasses.Response.BseResponse;
using PayhouseDragonFly.CORE.DTOs;
using PayhouseDragonFly.CORE.DTOs.escalate;
using PayhouseDragonFly.CORE.DTOs.resolve;
using PayhouseDragonFly.CORE.DTOs.Ticketsvms;
using PayhouseDragonFly.CORE.Models.TicketRegistration;
using PayhouseDragonFly.CORE.Models.UserRegistration;
using PayhouseDragonFly.INFRASTRUCTURE.DataContext;
using PayhouseDragonFly.INFRASTRUCTURE.Services.ExtraServices;
using PayhouseDragonFly.INFRASTRUCTURE.Services.IServiceCoreInterfaces.IticketsCoreServices;

namespace PayhouseDragonFly.INFRASTRUCTURE.Services.ServiceCore.TicketService
{
    public class TicketService : IticketsCoreServices
    {

        //private readonly JwtBearerTokenSettings _jwtBearerTokenSettings;
        private readonly UserManager<PayhouseDragonFlyUsers> _userManager;
        private readonly SignInManager<PayhouseDragonFlyUsers> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<TicketService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DragonFlyContext _authDbContext;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IEExtraServices _extraServices;

        public TicketService
            (
                 UserManager<PayhouseDragonFlyUsers> userManager,
                 SignInManager<PayhouseDragonFlyUsers> signInManager,
                 RoleManager<IdentityRole> roleManager,
                 ILogger<TicketService> logger,
                 IHttpContextAccessor httpContextAccessor,
                 DragonFlyContext authDbContext,
                 IServiceScopeFactory scopeFactory,
                 IEExtraServices extraServices
               )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
            _signInManager = signInManager;
            _authDbContext = authDbContext;
            _scopeFactory = scopeFactory;
            _extraServices = extraServices;

        }

        public async Task<BaseResponse> AddTicket(Ticketsvms vm)
        {

            try
            {
                if (vm.ClientName == "")
                {
                    return new BaseResponse("150", "Client Name cannot be empty", null);
                }
                if (vm.clientLocation == "")
                {
                    return new BaseResponse("150", "Client Location cannot be empty", null);
                }
                if (vm.subject == "")
                {
                    return new BaseResponse("150", "Subject cannot be empty", null);

                }
                if (vm.ServiceIssue == "")
                {
                    return new BaseResponse("150", "Service Issue cannot be empty", null);

                }


                if (vm.itemName == "")
                {
                    return new BaseResponse("150", "item Name cannot be empty", null);
                }

                if (vm.description == "")
                {

                    return new BaseResponse("150", "Description cannot be empty", null);
                }
                if (vm.ServiceName == "")
                {
                    return new BaseResponse("150", "Service Name cannot be empty", null);
                }
                if (vm.priorityName == "")
                {
                    return new BaseResponse("150", "priority Name cannot be empty", null);
                }
                if (vm.siteArea == "")
                {
                    return new BaseResponse("150", "site Area cannot be empty", null);
                }
               



                using (var scope = _scopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var loggedinuserobject = await _extraServices.LoggedInUser();

                    var userEmail = loggedinuserobject.Email;

                    if (loggedinuserobject == null)
                    {

                        return new BaseResponse("190", "user not logged in. login again", null);

                    }


                    var newticket = new Tickets
                    {
                        TicketTitle = "New Ticket",
                        ServiceName = vm.ServiceName,
                        siteArea = vm.siteArea,
                        clientLocation = vm.clientLocation,
                        description = vm.description,
                        subject = vm.subject,
                        priorityName = vm.priorityName,
                        ServiceIssue = vm.ServiceIssue,
                        Status = "New",
                        itemName = vm.itemName,
                        CreatorEmail = userEmail,
                        ClientName = vm.ClientName,
                        assignedtoNames = "Unassigned",
                        Resolution="?",
                        Escalation="?",
                       


                        


                    };

                    await scopedcontext.Tickets.AddAsync(newticket);
                    await scopedcontext.SaveChangesAsync();

                    return new BaseResponse("200", "Ticket created successfully, Go to tickets table to see status  ", null);
                }



            }
            catch (Exception ex)
            {

                return new BaseResponse("120", ex.Message, null);
            }

        }
        public async Task<BaseResponse> GetAllTickets()
        {

            var alltickets = await _authDbContext.Tickets.ToListAsync();
            if (alltickets == null)
            {
                return new BaseResponse("130", "there are no tickets available", null);
            }

            return new BaseResponse("200", "Queried successfully", alltickets);
        }


        public async Task<BaseResponse> EditTicketStatus(string status, int ticketid)
        {

            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    //check if ticket exists
                    var ticketexists = await scopedcontext.Tickets.Where(t => t.Id == ticketid).FirstOrDefaultAsync();
                    if (ticketexists == null)
                    {
                        return new BaseResponse("149", "ticket not found", null);
                    }

                    ticketexists.Status = status;


                    scopedcontext.Update(ticketexists);
                    await scopedcontext.SaveChangesAsync();

                    return new BaseResponse("200", "Quried successfully", null);
                }


            }
            catch (Exception ex)
            {

                return new BaseResponse("160", ex.Message, null);
            }
        }

        public async Task<BaseResponse> GetTicketById(int ticketid)
        {

            var ticket = await _authDbContext.Tickets.Where(x => x.Id == ticketid).FirstOrDefaultAsync();


            if (ticket == null)
            {

                return new BaseResponse("120", "Ticketnot available", null);
            }

            return new BaseResponse("200", "Queried succesfully", ticket);
        }

        public async Task<BaseResponse> AsignedUserToTicket(asignuservm assignvm)
        {

            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var userexists = await scopedcontext.PayhouseDragonFlyUsers.Where(u => u.Id == Convert.ToString(assignvm.userId)).FirstOrDefaultAsync();

                    if (userexists == null)
                    {
                        return new BaseResponse("150", "User does not exist", null);
                    }
                    var ticketexists = await scopedcontext.Tickets.Where(t => t.Id == assignvm.ticketid).FirstOrDefaultAsync();

                    if (ticketexists == null)
                    {
                        return new BaseResponse("150", "ticket does not exist", null);
                    }
                    //login begins

                    ticketexists.assignedtoNames = userexists.FirstName + " " + userexists.LastName;
                    ticketexists.Status = assignvm.status;
                    ticketexists.DateAsigned = DateTime.Now;

                    scopedcontext.Update(ticketexists);
                    await scopedcontext.SaveChangesAsync();
                    return new BaseResponse("200", $"Tickets assigned to {userexists.FirstName} {userexists.LastName} successfully", null);

                }


            }
            catch (Exception ex)
            {

                return new BaseResponse("190", ex.Message, null);
            }

        }
        public async Task<BaseResponse> ResolveTicket(ResolveVm resolvevm)
        {

            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    //var userexists = await scopedcontext.PayhouseDragonFlyUsers.Where(u => u.Id == Convert.ToString(resolvevm.userId)).FirstOrDefaultAsync();

                    //if (userexists == null)
                    //{
                    //    return new BaseResponse("150", "User does not exist", null);
                    //}
                    var ticketexists = await scopedcontext.Tickets.Where(t => t.Id == resolvevm.ticketid).FirstOrDefaultAsync();

                    if (ticketexists == null)
                    {
                        return new BaseResponse("150", "ticket does not exist", null);
                    }
                    //login begins


                    ticketexists.Status = resolvevm.status;
                    ticketexists.DateAsigned = DateTime.Now;
                    ticketexists.Resolution = resolvevm.Resolution;
                    scopedcontext.Update(ticketexists);
                    await scopedcontext.SaveChangesAsync();
                    return new BaseResponse("200", $"Ticket resolved successfully", null);

                }


            }
            catch (Exception ex)
            {

                return new BaseResponse("190", ex.Message, null);
            }


        }
        public async Task<BaseResponse> EscalateTicket(Escalatevm escalatevm)
        {

            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    //var userexists = await scopedcontext.PayhouseDragonFlyUsers.Where(u => u.Id == Convert.ToString(resolvevm.userId)).FirstOrDefaultAsync();

                    //if (userexists == null)
                    //{
                    //    return new BaseResponse("150", "User does not exist", null);
                    //}
                    var ticketexists = await scopedcontext.Tickets.Where(t => t.Id == escalatevm.ticketid).FirstOrDefaultAsync();

                    if (ticketexists == null)
                    {
                        return new BaseResponse("150", "ticket does not exist", null);
                    }
                    //logic begins


                    ticketexists.Status = escalatevm.status;
                    ticketexists.DateAsigned = DateTime.Now;
                    ticketexists.Escalation = escalatevm.Escalation;

                    scopedcontext.Update(ticketexists);
                    await scopedcontext.SaveChangesAsync();
                    return new BaseResponse("200", $"Ticket escalated successfully", null);

                }


            }
            catch (Exception ex)
            {

                return new BaseResponse("190", ex.Message, null);
            }


        }
        public async Task<BaseResponse> CloseTicket(CloseTicketvm closeTicketvm)
        {

            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    //var userexists = await scopedcontext.PayhouseDragonFlyUsers.Where(u => u.Id == Convert.ToString(resolvevm.userId)).FirstOrDefaultAsync();

                    //if (userexists == null)
                    //{
                    //    return new BaseResponse("150", "User does not exist", null);
                    //}
                    var ticketexists = await scopedcontext.Tickets.Where(t => t.Id == closeTicketvm.ticketid).FirstOrDefaultAsync();

                    if (ticketexists == null)
                    {
                        return new BaseResponse("150", "ticket does not exist", null);
                    }
                    //logic begins


                    ticketexists.Status = closeTicketvm.status;
                    ticketexists.DateAsigned = DateTime.Now;
                    ticketexists.TicketTitle = closeTicketvm.TicketTitle;
            

                    scopedcontext.Update(ticketexists);
                    await scopedcontext.SaveChangesAsync();
                    return new BaseResponse("200", $"Ticket closed successfully...Client will be notified shortly", null);

                }


            }
            catch (Exception ex)
            {

                return new BaseResponse("190", ex.Message, null);
            }


        }
    }
}
