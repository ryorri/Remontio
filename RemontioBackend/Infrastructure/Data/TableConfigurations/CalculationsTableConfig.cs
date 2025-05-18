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
    public class CalculationsTableConfig : IEntityTypeConfiguration<Calculations>
    {
        public void Configure(EntityTypeBuilder<Calculations> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Type)
                 .IsRequired()
                 .HasConversion<string>();


            builder.HasOne(p => p.User)
                    .WithMany(u => u.Calculations)
                    .HasForeignKey(p => p.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Room)
                    .WithMany(u => u.Calculations)
                    .HasForeignKey(p => p.RoomId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Project)
                    .WithMany(u => u.Calculations)
                    .HasForeignKey(p => p.ProjectId)
                    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
