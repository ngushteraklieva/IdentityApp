using API.Models;
using API.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data
{
    public static class ContextInitializer
    {
        public static async Task InitializeAsync(Context context, UserManager<AppUser> userManager)
        {
            if (context.Database.GetPendingMigrations().Count() > 0)
            {
                await context.Database.MigrateAsync();
            }

            if (!userManager.Users.Any())
            {
                var john = new AppUser
                {
                    Name = "John",
                    UserName = "john",
                    Email = "john@example.com",
                    EmailConfirmed = true,
                    LockoutEnabled = true,
                };

                await userManager.CreateAsync(john, SD.DefaultPassword);

                var tom = new AppUser
                {
                    Name = "Tom",
                    UserName = "tom",
                    Email = "tom@example.com",
                    EmailConfirmed = true,
                    LockoutEnabled = true,
                };

                await userManager.CreateAsync(john, SD.DefaultPassword);

                var peter = new AppUser
                {
                    Name = "Peter",
                    UserName = "peter",
                    Email = "peter@example.com",
                    EmailConfirmed = true,
                    LockoutEnabled = true,
                };

                await userManager.CreateAsync(john, SD.DefaultPassword);

                var sam = new AppUser
                {
                    Name = "Sam",
                    UserName = "sam",
                    Email = "sam@example.com",
                    EmailConfirmed = true,
                    LockoutEnabled = true,
                };

                await userManager.CreateAsync(john, SD.DefaultPassword);

            }
        }
    }
}
