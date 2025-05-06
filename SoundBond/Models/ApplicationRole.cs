using Microsoft.AspNetCore.Identity;

namespace SoundBond.Models
{
    public class ApplicationRole : IdentityRole
    {
        public ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}
