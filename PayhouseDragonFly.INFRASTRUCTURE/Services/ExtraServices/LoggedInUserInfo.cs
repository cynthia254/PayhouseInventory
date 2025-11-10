using PayhouseDragonFly.CORE.Models.Roles;
using PayhouseDragonFly.CORE.Models.UserRegistration;

namespace PayhouseDragonFly.INFRASTRUCTURE.Services.ExtraServices
{
    public class LoggedInUserInfo
    {
        public PayhouseDragonFlyUsers User { get; set; }
        public List<OtherRoles> OtherRoles { get; set; }
    }
}