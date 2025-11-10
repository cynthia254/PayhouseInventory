using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayhouseDragonFly.CORE.ConnectorClasses.Response.BseResponse;
using PayhouseDragonFly.CORE.ConnectorClasses.Response.roleresponse;
using PayhouseDragonFly.CORE.ConnectorClasses.Response.RolesResponse;
using PayhouseDragonFly.CORE.ConnectorClasses.Response.StockResponse;
using PayhouseDragonFly.CORE.DTOs.loginvms;
using PayhouseDragonFly.CORE.DTOs.Roles;
using PayhouseDragonFly.CORE.DTOs.Stock;
using PayhouseDragonFly.INFRASTRUCTURE.Services.IServiceCoreInterfaces.IExtraServices;
using PayhouseDragonFly.INFRASTRUCTURE.Services.RoleServices;

namespace PayhouseDragonFly.API.Controllers.Roles
{

    [Route("api/[controller]", Name = "Role")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleServices _roleServices;
        //public readonly IticketsCoreServices _ticketServices;
        private readonly ILoggeinUserServices _loggeinuser;

        public RolesController(IRoleServices _roleService, ILoggeinUserServices loggeinuser)
        {
            _roleServices = _roleService;
            _loggeinuser = loggeinuser;


        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("CreateRole")]
        public async Task<RolesResponse> CreateRole(string Rolename)
        {
            var roleclaimname = "CreateRole";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var master_rolechecker_response = await _roleServices.MasterRoleChecker(loggedinuser.Id, roleclaimname);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {

                return await _roleServices.CreateRole(Rolename);

            }
            else
            {
                return new RolesResponse(false, "You have no permission to access this", null);
            }

        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        [Route("GetAllRoles")]
        public async Task<RolesResponse> GetAllRoles()
        {
            var roleclaimname = "ViewRoles";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var master_rolechecker_response = await _roleServices.MasterRoleChecker(loggedinuser.Id, roleclaimname);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {

                return await _roleServices.GetAllRoles();

            }
            else
            {
                return new RolesResponse(false, "You have no permission to access this", null);
            }
        }


        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        [Route("AssignUserToRole")]
        public async Task<RolesResponse> AssignUserToRole(string useremail, int roleid)
        {
            var roleclaimname = "AssignRole";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var master_rolechecker_response = await _roleServices.MasterRoleChecker(loggedinuser.Id, roleclaimname);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {

                return await _roleServices.AssignUserToRole(useremail, roleid);

            }
            else
            {
                return new RolesResponse(false, "You have no permission to access this", null);
            }

        }



        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("AddRoleClaim")]
        public async Task<Rolesresponse> AddRoleClaim(string roleclaimname)
        {
            var roleclaimnamename = "AddPermission";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var master_rolechecker_response = await _roleServices.MasterRoleChecker(loggedinuser.Id, roleclaimnamename);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {

                return await _roleServices.AddRoleClaim(roleclaimname);

            }
            else
            {
                return new Rolesresponse(false, "You have no permission to access this", null);
            }

        }


        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        [Route("AllClaims")]

        public async Task<Rolesresponse> GetAllRolecLaims()
        {
            var roleclaimname = "ViewPermissions";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var master_rolechecker_response = await _roleServices.MasterRoleChecker(loggedinuser.Id, roleclaimname);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {

                return await _roleServices.GetAllRolecLaims();

            }
            else
            {
                return new Rolesresponse(false, "You have no permission to access this", null);
            }
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("AddClaimstoRoles")]
        public async Task<Rolesresponse> AddClaimsToRole(int roleid, int claimid)
        {
            var roleclaimname = "GrantPermission";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var master_rolechecker_response = await _roleServices.MasterRoleChecker(loggedinuser.Id, roleclaimname);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {

                return await _roleServices.AddClaimsToRole(roleid,claimid);

            }
            else
            {
                return new Rolesresponse(false, "You have no permission to access this", null);
            }
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetAllroleClaims")]
        public async Task<RoleClaimsResponse> GetRoleClaims(int roleid)
        {
            var roleclaimname = "ViewPermissions";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var master_rolechecker_response = await _roleServices.MasterRoleChecker(loggedinuser.Id, roleclaimname);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {

                return await _roleServices.GetRoleClaims(roleid);

            }
            else
            {
               return new RoleClaimsResponse(false,"You have no permission to access this","",null);
            }
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("Getrolebyid")]
        public async Task<BaseResponse> GetRoleByID(int Roleid)
        {
            var roleclaimname = "ViewRoles";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var master_rolechecker_response = await _roleServices.MasterRoleChecker(loggedinuser.Id, roleclaimname);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {

                return await _roleServices.GetRoleByID(Roleid);

            }
            else
            {
                return new BaseResponse("190", "You have no permission to access this", null);
            }


        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("Deleteroleclaim")]
        public async Task<BaseResponse> DeleteRoleClaim(int ClaimId, int roleid)
        {
            var roleclaimname = "DeletePermission";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var master_rolechecker_response = await _roleServices.MasterRoleChecker(loggedinuser.Id, roleclaimname);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {
                return await _roleServices.DeleteRoleClaim(ClaimId, roleid);

            }
            else
            {
                return new BaseResponse("190", "You have no permission to access this", null);
            }
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("AssignUserOtherRoles")]
        public async Task<BaseResponse> AssignUserOtherRoles(string userId, List<int> roleIds)
        {
            return await _roleServices.AssignUserOtherRoles(userId,roleIds);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]

        [HttpPost]
        [Route("GetUserOtherRoles")]
        public async Task<BaseResponse> GetUserOtherRoles(string userid)
        {
            return await _roleServices .GetUserOtherRoles(userid);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("DeleteRole")]
        public async Task<BaseResponse> DeleteRole(string RoleName)
        {
            var roleclaimname = "DeleteRole";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var master_rolechecker_response = await _roleServices.MasterRoleChecker(loggedinuser.Id, roleclaimname);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {
                return await _roleServices.DeleteRole(RoleName);

            }
            else
            {
                return new BaseResponse("190", "You have no permission to access this", null);
            }
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("DeleteResponsibility")]
        public async Task<BaseResponse> DeleteResponsibility(int ClaimId)
        {
            var roleclaimname = "DeletePermission";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var master_rolechecker_response = await _roleServices.MasterRoleChecker(loggedinuser.Id, roleclaimname);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {
                return await _roleServices.DeleteResponsibility(ClaimId);

            }
            else
            {
                return new BaseResponse("190", "You have no permission to access this", null);
            }
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetUserRoles")]
        public async Task<BaseResponse> GetUserRoles(string userid)
        {
            return await _roleServices.GetUserRoles(userid);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetRoleByUserId")]
        public async Task<Rolesresponse> GetRoleByUserId(string userid)
        {
            return await _roleServices.GetRoleByUserId(userid);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("Total_Users_With_Role")]
        public async Task<Roles_User_CounterResponse> UsersWithRole(int roleid)
        {
            return await _roleServices.UsersWithRole(roleid);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("CheckAndGrantPermissions")]
        public async Task<BaseResponse> CheckAndGrantPermissions(string userId, List<string> requiredClaimNames)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return new BaseResponse("140", "User ID is required", null);
                }

                // Call the service method to update user claims based on roles
                var updateResponse = await _roleServices.CheckAndGrantPermissions(userId,requiredClaimNames);

                if (updateResponse.Code == "200")
                {
                    // Check if the user has the required claims
                    var userClaims = HttpContext.User.Claims
                        .Where(c => requiredClaimNames.Contains(c.Type))
                        .Select(c => c.Type)
                        .ToList();

                    if (userClaims.Count == requiredClaimNames.Count)
                    {
                        return new BaseResponse("200", "User permissions updated based on assigned roles and required claims", null);
                    }
                    else
                    {
                        return new BaseResponse("200", "User permissions updated based on assigned roles, but missing some required claims", null);
                    }
                }
                else
                {
                    return new BaseResponse("190", "Failed to update user permissions", null);
                }
            }
            catch (Exception ex)
            {
                return new BaseResponse("190", ex.Message, null);
            }
        }








    }
}

