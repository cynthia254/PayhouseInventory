using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PayhouseDragonFly.CORE.ConnectorClasses.Response.BseResponse;
using PayhouseDragonFly.CORE.ConnectorClasses.Response.roleresponse;
using PayhouseDragonFly.CORE.ConnectorClasses.Response.RolesResponse;
using PayhouseDragonFly.CORE.DTOs.Roles;
using PayhouseDragonFly.CORE.Models.Roles;
using PayhouseDragonFly.CORE.Models.UserRegistration;
using PayhouseDragonFly.INFRASTRUCTURE.DataContext;
using PayhouseDragonFly.INFRASTRUCTURE.Services.IServiceCoreInterfaces.IExtraServices;
using System.Data;

namespace PayhouseDragonFly.INFRASTRUCTURE.Services.RoleServices
{
    public class RoleServices : IRoleServices
    {
        private UserManager<PayhouseDragonFlyUsers> _userManager;
        private readonly UserManager<PayhouseDragonFlyUsers> _signinmanager;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly DragonFlyContext _dragonflyContext;
        private readonly DragonFlyContext _context;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILoggeinUserServices _loggedinuser;
        private readonly ILogger<RoleServices> _logger;
        public RoleServices(DragonFlyContext context,
            IServiceScopeFactory scopeFactory,
            UserManager<PayhouseDragonFlyUsers> userManager,
            UserManager<PayhouseDragonFlyUsers> signinmanager,
            ILoggeinUserServices loggedinuser,
            DragonFlyContext dragonflyContext,
            IServiceScopeFactory serviceScopeFactory,
            ILogger<RoleServices> logger

            )
        {
            _context = context;
            _serviceScopeFactory = serviceScopeFactory;
            _scopeFactory = scopeFactory;
            _userManager = userManager;
            _signinmanager = signinmanager;
            _loggedinuser = loggedinuser;
            _dragonflyContext = dragonflyContext;
            _logger = logger;
        }
        public async Task<RolesResponse> CreateRole(string Rolename)
        {

            try
            {
                if (Rolename == "")
                {

                    return new RolesResponse(false, "Kindly provide a role name to add role", null);
                }
                using (var scope = _scopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    //check if role exists 

                    var roleexists = await scopedcontext.RolesTable.Where(x => x.RoleName == Rolename).FirstOrDefaultAsync();

                    if (roleexists != null)
                    {
                        return new RolesResponse(false, $" Role  '{Rolename}' already exist, if  must add a similar role kindly change the " +
                             $"latter cases from lower to upper and vice versa depending on the existing  role . The existsing role is '{roleexists}' with role id {roleexists.RolesID} ", null);
                    }
                    var rolesclass = new RolesTable
                    {
                        RoleName = Rolename
                    };
                    await scopedcontext.AddAsync(rolesclass);
                    await scopedcontext.SaveChangesAsync();
                    return new RolesResponse(true, $"Role '{Rolename}'  created successfully", null);

                }

            }
            catch (Exception ex)
            {
                return new RolesResponse(false, ex.Message, null);

            }

        }
        public async Task<BaseResponse> DeleteRole(string RoleName)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var scopecontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var roleexists = await scopecontext.RolesTable.Where(x => x.RoleName == RoleName).FirstOrDefaultAsync();

                    if (roleexists == null)
                    {
                        return new BaseResponse("190", "Role does not exist ", null);
                    }
                    scopecontext.Remove(roleexists);
                    await scopecontext.SaveChangesAsync();

                    return new BaseResponse("200", "Role deleted successfully", null);
                }
            }
            catch (Exception ex)
            {
                return new BaseResponse("130", ex.Message, null);
            }


        }
        public async Task<RolesResponse> AssignUserToRole(string useremail, int roleid)
        {
            try
            {
                if (useremail == "")
                {
                    return new RolesResponse(false, "User email cannot be empty", null);
                }
                using (var scope = _scopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    //check user exists
                    var userexists = await _userManager.FindByEmailAsync(useremail);

                    if (userexists == null)
                    {
                        return new RolesResponse(false, "User does not exist", null);

                    }

                    //check roleexists


                    var roleexists = await scopedcontext.RolesTable.Where(x => x.RolesID == roleid).FirstOrDefaultAsync();

                    if (roleexists == null)
                    {

                        return new RolesResponse(false, "Role does not exist", null);
                    }


                    //check user already has role

                    userexists.RoleId = roleexists.RolesID;
                    scopedcontext.Update(userexists);
                    await scopedcontext.SaveChangesAsync();
                    return new RolesResponse(true, "Role Assigned successfully", null);

                }


            }
            catch (Exception ex)
            {
                return new RolesResponse(false, ex.Message, null);

            }
        }


        public async Task<RolesResponse> GetAllRoles()
        {

            try
            {

                using (var scope = _scopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var allroles = await scopedcontext.RolesTable.ToListAsync();

                    if (allroles == null)
                    {
                        return new RolesResponse(false, "Roles don't exist", null);
                    }
                    return new RolesResponse(true, "Successfully queried", allroles);

                }
            }
            catch (Exception ex)
            {
                return new RolesResponse(false, ex.Message, null);
            }
        }

        //role checker based on logged in user

        public async Task<RolesResponse> RoleChecker()
        {
            try
            {

                using (var scope = _scopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    //roles chacker

                    return new RolesResponse(false, "not yet implemented , to bve done later", null);
                }
            }
            catch (Exception ex)
            {
                return new RolesResponse(false, ex.Message, null);



            }
        }
        public async Task<string> Roleschecker()
        {

            var user = await _loggedinuser.LoggedInUser();
            var roleexists = await _context.RolesTable.Where(x => x.RolesID == user.RoleId).FirstOrDefaultAsync();
            if (roleexists.RoleName == "SuperAdmin")
            {
                return "SuperAdmin";
            }

            else if (roleexists.RoleName == "Admin")
            {
                return "Admin";
            }
            else if (roleexists.RoleName == "Partner")
            {
                return "Partner";
            }

            else if (roleexists.RoleName == "User")
            {
                return "User";
            }
            return "";
        }

        public async Task<Rolesresponse> AddRoleClaim(string roleclaimname)
        {



            if (roleclaimname == "")
            {

                return new Rolesresponse(false, "Claim name cannot be null", null);
                //return new RolesResponse(false, "Claim name cannot be null", null)
            }


            var roleclainexists = await _dragonflyContext.RoleClaimsTable.Where(x => x.ClaimName == roleclaimname).FirstOrDefaultAsync();

            if (roleclainexists != null)
            {
                return new Rolesresponse(false, "Claim already exists", null);

            }
            var roleclaimadded = new RoleClaimsTable
            {

                ClaimName = roleclaimname
            };

            await _dragonflyContext.AddAsync(roleclaimadded);
            await _dragonflyContext.SaveChangesAsync();
            return new Rolesresponse(true, "Successfully added role Claim type", null);
            // return new RolesResponse(true, "Role claim addedd successfully", null);


        }

        public async Task<Rolesresponse> GetAllRolecLaims()
        {
            var allclaims = await _dragonflyContext.RoleClaimsTable.ToListAsync();

            return new Rolesresponse(true, "role claims queried successfully", allclaims);
        }



        public async Task<Rolesresponse> AddClaimsToRole(int roleid, int claimid)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var checkifclaimexists = await scopedcontext.Claim_Role_Map.Where(x => x.ClaimId == claimid && x.RoleId == roleid).FirstOrDefaultAsync();


                    if (checkifclaimexists != null)
                    {
                        return new Rolesresponse(false, "Claim already exists", null);

                    }

                    var new_claim_role_map = new Claim_Role_Map
                    {
                        RoleId = roleid,
                        ClaimId = claimid
                    };
                    var claimnameexists = await scopedcontext.RoleClaimsTable.Where(c => c.RolesClaimsTableId == claimid).FirstOrDefaultAsync();
                    var roleeixists = await scopedcontext.RolesTable.Where(r => r.RolesID == roleid).FirstOrDefaultAsync();
                    await scopedcontext.AddAsync(new_claim_role_map);
                    await scopedcontext.SaveChangesAsync();

                    return new Rolesresponse(true, $"Successfully added claim  {claimnameexists.ClaimName} to role   {roleeixists.RoleName}", null);

                }

            }
            catch (Exception ex)
            {

                return new Rolesresponse(false, ex.Message, null);
            }

        }
        public async Task<RoleClaimsResponse> GetRoleClaims(int roleid)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();


                    //get associated role
                    var roleexists = await scopedcontext.RolesTable.Where(r => r.RolesID == roleid).FirstOrDefaultAsync();

                    if (roleexists == null)
                    {

                        return new RoleClaimsResponse(false, "Role does not exist", "No role", null);

                    }

                    //get all related  claims to athe role found role mapper(only shows ids)

                    var roleclaimsexists = await scopedcontext.Claim_Role_Map.Where(rc => rc.RoleId == roleid).ToListAsync();


                    if (roleclaimsexists == null)
                    {
                        return new RoleClaimsResponse(false, "Role claims does not exist", "No role claims", null);
                    }
                    //all claims on claim table(specific name)


                    List<RoleClaimsTable> claimstable = new List<RoleClaimsTable>();

                    foreach (var role in roleclaimsexists)
                    {
                        var claimqueried = await scopedcontext.RoleClaimsTable.Where(x => x.RolesClaimsTableId == role.ClaimId).FirstOrDefaultAsync();

                        if (claimqueried == null)
                        {

                            _logger.LogInformation("____________________no role claim found_______________________");
                        }
                        var newclaimfound = new RoleClaimsTable
                        {
                            RolesClaimsTableId = claimqueried.RolesClaimsTableId,
                            ClaimName = claimqueried.ClaimName,

                        };

                        claimstable.Add(newclaimfound);


                    }



                    //return values found

                    return new RoleClaimsResponse(true, "Queries sueccessfully", roleexists.RoleName, claimstable);

                }


            }
            catch (Exception ex)
            {

                return new RoleClaimsResponse(false, ex.Message, "No role", null);
            }

        }
        public async Task<BaseResponse> GetRoleByID(int Roleid)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var roleexists = await scopedcontext.RolesTable.Where(x => x.RolesID == Roleid).FirstOrDefaultAsync();
                    if (roleexists == null)
                    {
                        return new BaseResponse("130", "Role does not exist", null);

                    }

                    return new BaseResponse("200", "Queried successfully", roleexists);
                }

            }
            catch (Exception ex)
            {
                return new BaseResponse("120", ex.Message, null);
            }
        }

        public async Task<BaseResponse> GetRoleClaimByname(string claimname)
        {

            var claimexists = await _context.RoleClaimsTable.Where(c => c.ClaimName == claimname).FirstOrDefaultAsync();

            if (claimexists == null)
            {

                return new BaseResponse("120", "You have no permission to access this page", null);
            }
            return new BaseResponse("200", "successfully queried", claimexists);

        }


        public async Task<bool> CheckClaimInRole(String claimname, int roleid)
        {

            //check role claim name exists 
            var roleclaim = await _context.RoleClaimsTable.Where(c => c.ClaimName == claimname).FirstOrDefaultAsync();
            //check role exists
            if (roleclaim == null)
            {
                return false;
            }
            //exists in mapper
            var roleexists = await _context.Claim_Role_Map
                     .Where(x => x.RoleId == roleid && x.ClaimId == roleclaim.RolesClaimsTableId).FirstOrDefaultAsync();

            if (roleexists == null)
            {

                return false;
            }

            return true;
        }
        public async Task<BaseResponse> DeleteRoleClaim(int ClaimId, int roleid)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var scopecontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    //check role claim exists in map table
                    var roleclaiminmaptable = await scopecontext.Claim_Role_Map.Where(x => x.ClaimId == ClaimId && x.RoleId == roleid).FirstOrDefaultAsync();

                    if (roleclaiminmaptable == null)
                    {
                        return new BaseResponse("190", "Role responsiblity not assigned to role group ", null);
                    }
                    scopecontext.Remove(roleclaiminmaptable);
                    await scopecontext.SaveChangesAsync();

                    return new BaseResponse("200", "Responsibility deleted successfully", null);
                }
            }
            catch (Exception ex)
            {
                return new BaseResponse("130", ex.Message, null);
            }


        }
        public async Task<BaseResponse> DeleteResponsibility(int ClaimId)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var scopecontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    //check role claim exists in map table
                    var roleclaiminmaptable = await scopecontext.RoleClaimsTable.Where(x => x.RolesClaimsTableId == ClaimId ).FirstOrDefaultAsync();

                    if (roleclaiminmaptable == null)
                    {
                        return new BaseResponse("190", "Role responsiblity does not exist ", null);
                    }
                    scopecontext.Remove(roleclaiminmaptable);
                    await scopecontext.SaveChangesAsync();

                    return new BaseResponse("200", "Responsibility deleted successfully", null);
                }
            }
            catch (Exception ex)
            {
                return new BaseResponse("130", ex.Message, null);
            }


        }

        //add user other roles
        public async Task<BaseResponse> AssignUserOtherRoles(string userId, List<int> roleIds)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    // Check if user exists
                    var userexists = await scopedcontext.PayhouseDragonFlyUsers.FirstOrDefaultAsync(p => p.Id == userId);

                    if (userexists == null)
                    {
                        return new BaseResponse("140", $"User with ID {userId} does not exist", null);
                    }

                    var addedRolesCount = 0;


                    foreach (var roleId in roleIds)
                    {
                        if (roleId <= 0)
                        {
                            continue;
                        }

                        // Check if role exists
                        var roleexists = await scopedcontext.RolesTable.FirstOrDefaultAsync(p => p.RolesID == roleId);

                        if (roleexists == null)
                        {
                            continue;
                        }

                        // Check if role is already assigned to the user
                        var roleAlreadyAdded = await scopedcontext.OtherRoles
                            .FirstOrDefaultAsync(y => y.RoleID == roleId && y.userID == userId);

                        if (roleAlreadyAdded != null)
                        {
                            continue;
                        }

                        // Add role to the user
                        var otherRoleAdded = new OtherRoles
                        {
                            userID = userId,
                            RoleID = roleId,
                        };
                        await scopedcontext.AddAsync(otherRoleAdded);
                        await scopedcontext.SaveChangesAsync();

                        addedRolesCount++;
                    }

                        return new BaseResponse("200", "Roles Added Successfully", null);
                    }
                
            }
            catch (Exception ex)
            {
                return new BaseResponse("190", ex.Message, null);
            }
        }

     

        //get user other roles 

        public async Task<BaseResponse> GetUserOtherRoles(string userid)
        {

            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    if (userid == null)
                    {

                        return new BaseResponse("180", "user id cannot be null", null);
                    }
                    //user exists

                    var userexists = await scopedcontext.PayhouseDragonFlyUsers.Where(p => p.Id == userid).FirstOrDefaultAsync();

                    if (userexists == null)
                    {
                        return new BaseResponse("140", "user does not exist", null);
                    }


                    //get all roles assciated by user

                    var alluserroles = await scopedcontext.OtherRoles.Where(x => x.userID == userid).ToListAsync();
                    if (alluserroles.Count == 0)
                    {
                        return new BaseResponse("120", "user has no other roles", null);

                    }

                    List<RolesTable> roles = new List<RolesTable>();

                    foreach (var userrole in alluserroles)
                    {

                        var rolefound = await scopedcontext.RolesTable.Where(r => r.RolesID == userrole.RoleID).FirstOrDefaultAsync();
                        if (rolefound == null)
                        {
                            _logger.LogInformation("no role found________");

                        }
                        var newroledisplay = new RolesTable
                        {
                          
                           RolesID=rolefound.RolesID,
                           RoleName=rolefound.RoleName,







                        };
                       

                        roles.Add(newroledisplay);
                       
                    }

                    return new BaseResponse("200", $"Other roles for {userexists.FirstName} {userexists.LastName} include :", roles);
                }

                }
            catch (Exception ex)
            {
                return new BaseResponse("180", ex.Message, null);

            }
        
        }
        public async Task<BaseResponse> GetUserRoles(string userid)
        {

            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    if (userid == null)
                    {

                        return new BaseResponse("180", "user id cannot be null", null);
                    }
                    //user exists

                    var userexists = await scopedcontext.PayhouseDragonFlyUsers.Where(p => p.Id == userid ).FirstOrDefaultAsync();

                    if (userexists == null)
                    {
                        return new BaseResponse("140", "user does not exist", null);
                    }
                    return new BaseResponse("200", "Queried successfully", userexists);
                }

            }
            catch (Exception ex)
            {
                return new BaseResponse("120", ex.Message, null);
            }
        }

        public async Task<Rolesresponse> GetRoleByUserId( string userid)
        {

            try
            {

                using (var scope = _scopeFactory.CreateScope())
                {

                    var scopedcontext =  scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    //user exists

                    var userexists= await _userManager.FindByIdAsync(userid);

                    if (userexists == null)
                    {
                        return new Rolesresponse(false, "User does not exists", null);
                    }
                    //role exists
                    var roleexists= await scopedcontext.RolesTable.Where(r=>r.RolesID== userexists.RoleId).FirstOrDefaultAsync();
                    if(roleexists == null)
                    { 
                        return new Rolesresponse(false, "No role", null);

                    }
                    else { 

                    return new Rolesresponse(true, $"{roleexists.RoleName}", roleexists);
                    }



                }

            }
            catch (Exception ex)
            {

                return new Rolesresponse(false, ex.Message, null);
            }
        }
        public async Task<Roles_User_CounterResponse> UsersWithRole( int roleid)
        {

            try
            {
                  using(var scope = _scopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();


                    //check role exists

                    var roleexists = await scopedcontext.RolesTable.Where(r => r.RolesID == roleid).FirstOrDefaultAsync();

                    if (roleexists == null)
                        return new Roles_User_CounterResponse(0, "No role", null);

                    //users associated with role

                    var users_with_role = await scopedcontext.PayhouseDragonFlyUsers
                        .Where(u => u.RoleId == roleexists.RolesID).ToListAsync();

                    if (users_with_role == null)
                        return new Roles_User_CounterResponse(0, "No users associated with the role", null);

                    //count users associated with users

                    var count_User_with_role = await scopedcontext.PayhouseDragonFlyUsers
                        .Where(u => u.RoleId == roleexists.RolesID).CountAsync();

                    if (count_User_with_role <=0)
                        return new Roles_User_CounterResponse(0, "No users associated with the role", null);



                    return new Roles_User_CounterResponse(count_User_with_role, roleexists.RoleName, users_with_role);



                }
            }
            catch (Exception ex) {
                return new Roles_User_CounterResponse(0, ex.Message, null);
            
            }
        }
        public async Task<BaseResponse> CheckAndGrantPermissions(string userId, List<string> requiredClaimNames)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    // Check if the user exists
                    var userExists = await scopedcontext.PayhouseDragonFlyUsers
                        .AnyAsync(u => u.Id == userId);

                    if (!userExists)
                    {
                        return new BaseResponse("140", $"User with ID {userId} does not exist", null);
                    }

                    // Get the role IDs assigned to the user in the "OtherRoles" table
                    var assignedRoleIds = await scopedcontext.OtherRoles
                        .Where(or => or.userID == userId)
                        .Select(or => or.RoleID)
                        .ToListAsync();

                    // Check if the user's assigned roles grant any of the required claims
                    var permissionsGranted = false;
                    foreach (var roleId in assignedRoleIds)
                    {
                        var hasPermission = await scopedcontext.Claim_Role_Map
              .AnyAsync(crm => crm.RoleId == roleId && requiredClaimNames.Contains(crm.ClaimId.ToString()));


                        if (hasPermission)
                        {
                            permissionsGranted = true;
                            break; // If any role grants permission, no need to continue checking
                        }
                    }

                    if (permissionsGranted)
                    {
                        return new BaseResponse("200", $"User with ID {userId} is granted permission for the specified claims", null);
                    }
                    else
                    {
                        return new BaseResponse("140", $"User with ID {userId} is not granted permission for the specified claims", null);
                    }
                }
            }
            catch (Exception ex)
            {
                return new BaseResponse("190", ex.Message, null);
            }

        }


        public async Task<bool> MasterRoleChecker(string userid, string claimrole)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();



                    // All user role lists
                    var all_user_roles = await scopedcontext.OtherRoles.Where(y => y.userID == userid).ToListAsync();
                    if (all_user_roles == null)
                    {
                        return false;
                    }

                    var claimexists = await scopedcontext.RoleClaimsTable.Where(y => y.ClaimName == claimrole).FirstOrDefaultAsync();
                    if (claimexists == null)
                    {
                        return false;
                    }

                    // Claim role exists 
                    foreach (var eachrole in all_user_roles)
                    {
                        var claimmapped = await scopedcontext
                            .Claim_Role_Map.Where(y => y.RoleId == eachrole.RoleID && y.ClaimId == claimexists.RolesClaimsTableId).FirstOrDefaultAsync();

                        if (claimmapped == null)
                        {
                            continue;
                        }
                        else
                        {
                            return true;
                        }
                    }

                    // If we reach this point, no matching claim mappings were found
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"method error :{ex.Message}");
                return false;
            }
        }



    }

}
