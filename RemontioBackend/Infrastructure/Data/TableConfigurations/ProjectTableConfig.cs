using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.TableConfigurations
{
    public class ProjectTableConfig : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(50);
            builder.Property(p => p.Description).HasMaxLength(500);
            builder.Property(p => p.CreateAt).IsRequired();
            builder.Property(p => p.Status)
                  .IsRequired()
                  .HasConversion<string>();



            builder.HasOne(p => p.User)
                    .WithMany(u => u.Projects)
                    .HasForeignKey(p => p.UserId);
        }
    
    
    }
}
