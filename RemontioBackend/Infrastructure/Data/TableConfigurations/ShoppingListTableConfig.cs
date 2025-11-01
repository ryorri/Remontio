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
    public class ShoppingListTableConfig : IEntityTypeConfiguration<ShoppingList>
    {
        public void Configure(EntityTypeBuilder<ShoppingList> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(50);
            builder.Property(p => p.Description).HasMaxLength(500);
            builder.Property(p => p.CreateAt).IsRequired();

            builder.HasOne(p => p.User)
                .WithMany(u => u.ShoppingLists)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(p => p.Room)
                    .WithMany(u => u.ShoppingLists)
                    .HasForeignKey(p => p.RoomId)
                    .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(p => p.Project)
                    .WithMany(u => u.ShoppingLists)
                    .HasForeignKey(p => p.ProjectId)
                    .OnDelete(DeleteBehavior.NoAction);

        }
    }
    
}
