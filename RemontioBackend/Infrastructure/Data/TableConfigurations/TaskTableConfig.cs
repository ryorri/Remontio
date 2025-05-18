using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.TableConfigurations
{
    public class TaskTableConfig : IEntityTypeConfiguration<Domain.Entities.Task>
    {

        public void Configure(EntityTypeBuilder<Domain.Entities.Task> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(50);
            builder.Property(p => p.Description).HasMaxLength(450);
            builder.Property(p => p.CreateAt).IsRequired();
            builder.Property(p => p.Status)
                 .IsRequired()
                 .HasConversion<string>();
            builder.Property(p => p.Priority)
                 .IsRequired()
                 .HasConversion<string>();

            builder.HasOne(p => p.Project)
                  .WithMany(u => u.Tasks)
                  .HasForeignKey(p => p.ProjectId)
                  .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Room)
                  .WithMany(u => u.Tasks)
                  .HasForeignKey(p => p.RoomId)
                  .OnDelete(DeleteBehavior.Restrict);
        }
    }
}