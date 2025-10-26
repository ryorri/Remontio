using Application.Interfaces.DatabaseInterfaces;
using Application.Interfaces.SeederInterfaces;
using Application.Interfaces.ServiceInterfaces;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Extensions;
using Infrastructure.Seeders;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Infrastructure
{
    public static class InfrastructureInjection
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            #region Scopes
            services.AddTransient<DatabaseInitialiser>();
            services.AddTransient<UserExtension>();
            services.AddScoped<CalculatingAreaExtension>();
            services.AddScoped<IRemontioDbContext, RemontioDbContext>();

            services.AddScoped<IRoleSeeder, RoleSeeder>();
            services.AddScoped<IUserSeeder, UserSeeder>();

            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IRoomService, RoomService>();


            #endregion

            #region DB

            services.AddDbContext<RemontioDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));


            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
            })
            .AddEntityFrameworkStores<RemontioDbContext>()
            .AddDefaultTokenProviders();

            #endregion

            #region JWT

            services.AddAuthentication(op =>
            {
                op.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                op.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config["Jwt:Issuer"],
                    ValidAudience = config["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!))
                };

                options.RequireHttpsMetadata = false;
                options.SaveToken = true;

            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("UserOnly", policy => policy.RequireClaim(ClaimTypes.NameIdentifier));
            });

            services.AddControllers();


            #endregion 

        }
    }
}
