using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class InfrastructureInjection
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            #region Scopes


            #endregion

            services.AddDbContext<RemontioDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));


            services.AddIdentityCore<User>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
            }).AddEntityFrameworkStores<RemontioDbContext>(); 

        }
    }
}
