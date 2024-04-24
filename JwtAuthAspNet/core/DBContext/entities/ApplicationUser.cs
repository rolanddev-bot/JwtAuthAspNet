using Microsoft.AspNetCore.Identity;

namespace JwtAuthAspNet.core.DBContext.entities
{
    public class ApplicationUser:IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
