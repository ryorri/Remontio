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
    public class RoomTableConfig : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(50);
            builder.Property(p => p.Description).HasMaxLength(450);
            builder.Property(p => p.CreateAt).IsRequired();
            builder.Property(p => p.Status)
                 .IsRequired()
                 .HasConversion<string>();




            builder.HasOne(p => p.Project)
                   .WithMany(u => u.Rooms)
                   .HasForeignKey(p => p.ProjectId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
