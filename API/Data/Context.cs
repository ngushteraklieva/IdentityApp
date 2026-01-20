using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Context: IdentityDbContext<AppUser, AppRole, int, 
        IdentityUserClaim<int>, AppUserRoleBridge, IdentityUserLogin<int>, 
        IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>().HasMany(ur => ur.Roles).WithOne(u => u.User).HasForeignKey(u => u.UserId).IsRequired();
            builder.Entity<AppRole>().HasMany(ur => ur.Users).WithOne(u => u.Role).HasForeignKey(u => u.RoleId).IsRequired();

        }
    }
}
//I am building a database context called Context.
//It should handle all the login and security stuff automatically.
//However, please use numbers for IDs instead of long strings,
//use my custom User and Role classes so I can add my own fields,
//and make sure the link between Users and Roles is strong and clear.