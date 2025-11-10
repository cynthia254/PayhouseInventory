
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayhouseDragonFly.CORE.ConnectorClasses.Response.BseResponse;
using PayhouseDragonFly.CORE.DTOs;
using PayhouseDragonFly.CORE.DTOs.escalate;
using PayhouseDragonFly.CORE.DTOs.resolve;
using PayhouseDragonFly.CORE.DTOs.Ticketsvms;
using PayhouseDragonFly.INFRASTRUCTURE.Services.ExtraServices.RoleChecker;
using PayhouseDragonFly.INFRASTRUCTURE.Services.IServiceCoreInterfaces.IExtraServices;
using PayhouseDragonFly.INFRASTRUCTURE.Services.IServiceCoreInterfaces.IticketsCoreServices;
using PayhouseDragonFly.INFRASTRUCTURE.Services.RoleServices;

namespace PayhouseDragonFly.API.Controllers.Tickets
{

    [Route("api/[controller]", Name = "Tickets")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly IRoleChecker _rolechecker;
        public readonly IticketsCoreServices _ticketServices;
        private readonly ILoggeinUserServices _loggeinuser;

        private readonly IRoleServices _roleservices;
        public TicketsController(
            IticketsCoreServices ticketServices,
            IRoleChecker rolechecker,
            IRoleServices roleservices,
            ILoggeinUserServices loggeinuser
            )
        {
            _ticketServices = ticketServices;
            _rolechecker= rolechecker;
            _loggeinuser = loggeinuser;
            _roleservices = roleservices;

        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("RegisterTicket")]
        public async Task<BaseResponse> AddTicket(Ticketsvms vm)
        {
            var roleclaimname = "CanCreateTicket";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var roleclaimtrue = await _roleservices
                .CheckClaimInRole(roleclaimname, loggedinuser.RoleId);

            if (loggedinuser.RoleId >0)
            {

                if (roleclaimtrue)
                {
                    return await _ticketServices.AddTicket(vm);
                }
                else if(!roleclaimtrue) 
                {
                    return new BaseResponse("110", "You have no permission access this", null);

                }
            }
            else
            {

                return new BaseResponse("120", "You have no permission access this", null);
            }
            return new BaseResponse("", "", null);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        [Route("GetAllTickets")]
        public async Task<BaseResponse> GetAllTickets()
        {
            var roleclaimname = "CanViewAllTickets";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var roleclaimtrue = await _roleservices
                .CheckClaimInRole(roleclaimname, loggedinuser.RoleId);

            if (loggedinuser.RoleId >0)
            {

                if (roleclaimtrue)
                {
                    return await _ticketServices.GetAllTickets();
                }
                else if (!roleclaimtrue)
                {
                    return new BaseResponse("110", "You have no permission access this", null);

                }
            }
            else
            {

                return new BaseResponse("120", "You have no permission access this", null);
            }
            return new BaseResponse("", "", null);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("EditTicketsStatus")]
        public async Task<BaseResponse> EditTicketStatus(string status, int ticketid)
        {
            var roleclaimname = "CanChangeTicketStatus";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var roleclaimtrue = await _roleservices
                .CheckClaimInRole(roleclaimname, loggedinuser.RoleId);

            if (loggedinuser.RoleId > 0)
            {

                if (roleclaimtrue)
                {
                    return await _ticketServices.EditTicketStatus(status, ticketid);
                }
                else if (!roleclaimtrue)
                {
                    return new BaseResponse("110", "You have no permission access this", null);

                }
            }
            else
            {

                return new BaseResponse("120", "You have no permission access this", null);
            }
            return new BaseResponse("", "", null);
        }


        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("getticketbyid")]

        public async Task<BaseResponse> GetTicketById(int ticketid)
        {

            var roleclaimname = "CanGetTicketById";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var roleclaimtrue = await _roleservices
                .CheckClaimInRole(roleclaimname, loggedinuser.RoleId);

            if (loggedinuser.RoleId > 0)
            {

                if (roleclaimtrue)
                {
                    return await _ticketServices.GetTicketById(ticketid);
                }
                else if (!roleclaimtrue)
                {
                    return new BaseResponse("110", "You have no permission access this", null);

                }
            }
            else
            {

                return new BaseResponse("120", "You have no permission access this", null);
            }
            return new BaseResponse("", "", null);
        }

    

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("Assigntickettouser")]
        public async Task<BaseResponse> AsignedUserToTicket(asignuservm assignvm)
        {
            var roleclaimname = "CanAssignTicketToUser";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var roleclaimtrue = await _roleservices
                .CheckClaimInRole(roleclaimname, loggedinuser.RoleId);

            if (loggedinuser.RoleId > 0)
            {

                if (roleclaimtrue)
                {
                    return await _ticketServices.AsignedUserToTicket(assignvm);
                }
                else if (!roleclaimtrue)
                {
                    return new BaseResponse("110", "You have no permission access this", null);

                }
            }
            else
            {

                return new BaseResponse("120", "You have no permission access this", null);
            }
            return new BaseResponse("", "", null);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("resolveticket")]
        public async Task<BaseResponse> ResolveTicket(ResolveVm resolvevm)
        {
            var roleclaimname = "CanResolveTicket";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var roleclaimtrue = await _roleservices
                .CheckClaimInRole(roleclaimname, loggedinuser.RoleId);

            if (loggedinuser.RoleId > 0)
            {

                if (roleclaimtrue)
                {
                    return await _ticketServices.ResolveTicket(resolvevm);
                }
                else if (!roleclaimtrue)
                {
                    return new BaseResponse("110", "You have no permission access this", null);

                }
            }
            else
            {

                return new BaseResponse("120", "You have no permission access this", null);
            }
            return new BaseResponse("", "", null);

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("escalateticket")]
        public async Task<BaseResponse> EscalateTicket(Escalatevm escalatevm)
        {
            var roleclaimname = "CanEscalateTicket";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var roleclaimtrue = await _roleservices
                .CheckClaimInRole(roleclaimname, loggedinuser.RoleId);
            if (loggedinuser.RoleId >0)
            {

                if (roleclaimtrue)
                {
                    return await _ticketServices.EscalateTicket(escalatevm);

                }
                else if (!roleclaimtrue)
                {
                    return new BaseResponse("110", "You have no permission access this", null);

                }
            }
            else
            {

                return new BaseResponse("120", "You have no permission access this", null);
            }
            return new BaseResponse("", "", null);
        }


        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("closeticket")]
        public async Task<BaseResponse> CloseTicket(CloseTicketvm closeTicketvm)
        {
            var roleclaimname = "CanCloseTicket";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var roleclaimtrue = await _roleservices
                .CheckClaimInRole(roleclaimname, loggedinuser.RoleId);

            if (loggedinuser.RoleId >0)
            {

                if (roleclaimtrue)
                {
                    return await _ticketServices.CloseTicket(closeTicketvm);
                }
                else if (!roleclaimtrue)
                {
                    return new BaseResponse("110", "You have no permission access this", null);

                }
            }
            else
            {

                return new BaseResponse("120", "You have no permission access this", null);
            }
            return new BaseResponse("", "", null);

        }


    }
}
