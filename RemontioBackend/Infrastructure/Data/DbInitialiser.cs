using Application.Interfaces.DatabaseInterfaces.SeederInterfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Data
{

    public static class InitialiserExtensions
    {
        public static async Task InitialiseDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var initialiser = scope.ServiceProvider.GetRequiredService<DatabaseInitialiser>();

            await initialiser.InitializeDB();

            await initialiser.SeedAsync();
        }
    }


    public class DatabaseInitialiser
    {
        private readonly RemontioDbContext _dBcontext;
        private readonly IRoleSeeder _roleSeeder;
        private readonly IUserSeeder _userSeeder;

        public DatabaseInitialiser(RemontioDbContext dBcontext, IRoleSeeder roleSeeder, IUserSeeder userSeeder)
        {
            _dBcontext = dBcontext;
            _roleSeeder = roleSeeder;
            _userSeeder = userSeeder;
        }

        public async Task InitializeDB()
        {
            try
            {
                await _dBcontext.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error {ex}");
            }
        }
        public async Task SeedAsync()
        {
            try
            {

                await SeedRolesAsync();
                await SeedUsersAsync();
                await _dBcontext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw new Exception($"Error {ex}");
            }
        }

        public async Task SeedRolesAsync()
        {
            await _roleSeeder.SeedRolesAsync();
        }
        public async Task SeedUsersAsync()
        {
            await _userSeeder.SeedUsersAsync();
        }

    }
}
