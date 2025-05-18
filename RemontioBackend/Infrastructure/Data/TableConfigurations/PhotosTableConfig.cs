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
    public class PhotosTableConfig : IEntityTypeConfiguration<Photos>
    {
        public void Configure(EntityTypeBuilder<Photos> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Url).IsRequired().HasMaxLength(200);
            builder.Property(p => p.Description).HasMaxLength(500);
            builder.Property(p => p.CreateAt).IsRequired();

            builder.HasOne(p => p.Project)
                .WithMany(u => u.Photos)
                .HasForeignKey(p => p.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Room)
                    .WithMany(u => u.Photos)
                    .HasForeignKey(p => p.RoomId)
                    .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
