using Microsoft.AspNet.Identity.EntityFramework;

namespace Domain
{
    public class ApplicationUser: IdentityUser
    {
        public string FullName { get; set; }
    }
}