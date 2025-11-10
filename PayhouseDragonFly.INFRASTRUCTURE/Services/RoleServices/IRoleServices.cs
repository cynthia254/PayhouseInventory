using PayhouseDragonFly.CORE.ConnectorClasses.Response.BseResponse;
using PayhouseDragonFly.CORE.ConnectorClasses.Response.roleresponse;
using PayhouseDragonFly.CORE.ConnectorClasses.Response.RolesResponse;
using PayhouseDragonFly.CORE.DTOs.Roles;

namespace PayhouseDragonFly.INFRASTRUCTURE.Services.RoleServices
{
    public interface IRoleServices
    {
        Task<RolesResponse> CreateRole(string Rolename);
        Task<RolesResponse> AssignUserToRole(string useremail, int roleid);
        Task<RolesResponse> GetAllRoles();
        Task<string> Roleschecker();
        Task<Rolesresponse> AddRoleClaim(string roleclaimname);
        Task<Rolesresponse> GetAllRolecLaims();
        Task<Rolesresponse> AddClaimsToRole(int roleid, int claimid);
        Task<RoleClaimsResponse> GetRoleClaims(int roleid);
        Task<BaseResponse> GetRoleByID(int Roleid);
        Task<BaseResponse> GetRoleClaimByname(string claimname);
        Task<bool> CheckClaimInRole(String claimname, int roleid);
        Task<BaseResponse> DeleteRoleClaim(int ClaimId, int roleid);
         Task<BaseResponse> GetUserOtherRoles(string userid);
         Task<BaseResponse> AssignUserOtherRoles(string userId, List<int> roleIds);
        Task<BaseResponse> DeleteRole(string RoleName);
        Task<BaseResponse> DeleteResponsibility(int ClaimId);
        Task<BaseResponse> GetUserRoles(string userid);
        Task<Rolesresponse> GetRoleByUserId(string userid);
        Task<Roles_User_CounterResponse> UsersWithRole(int roleid);
       Task<BaseResponse> CheckAndGrantPermissions(string userId, List<string> requiredClaimNames);
       Task<bool> MasterRoleChecker(string userid, string claimrole);

       }
        }

