using Application.Interfaces.DatabaseInterfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class RemontioDbContext : IdentityDbContext<User>, IRemontioDbContext
    {
        public RemontioDbContext(DbContextOptions<RemontioDbContext> options) : base(options) { }
        public override DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get ; set; }
    }
}
