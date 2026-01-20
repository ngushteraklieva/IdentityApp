using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace API.Models
{
    public class AppUser: IdentityUser<int>
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Boolean isActive { get; set; } = true;

        // NAvigations
        public ICollection<AppUserRoleBridge> Roles { get; set; }
    }
}
