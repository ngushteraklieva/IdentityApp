using API.Data;
using API.Models;
using API.Services;
using API.Services.IServices;
using API.Utility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<Context>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddScoped<ITokenService, TokenService>();

            return builder;
        }

        public static WebApplicationBuilder AddAuthenticationServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.Password.RequiredLength = SD.RequiredPasswordLength;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedAccount = true;
                options.Lockout.AllowedForNewUsers = false;
                options.Lockout.MaxFailedAccessAttempts = SD.MaxFailedAccessAttempts;
                options.Lockout.DefaultLockoutTimeSpan = System.TimeSpan.FromDays(SD.DefaultLockoutTimeSpanInDays);
            }).AddEntityFrameworkStores<Context>()
             .AddDefaultTokenProviders();

            return builder;
        }
    }
}
