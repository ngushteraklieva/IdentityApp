using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class AppUser: IdentityUser<int>
    {
        [Required]
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Boolean isActive { get; set; } = true;

        // NAvigations
        public ICollection<AppUserRoleBridge> Roles { get; set; }
    }
}
