
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace API.Models
{
    public class AppUserRoleBridge: IdentityUserRole<int> 
    {
        public AppUser User { get; set; }
        public AppRole Role { get; set; }

        // Navigations
        public ICollection<AppUserRoleBridge> Roles { get; set; }

    }
}
