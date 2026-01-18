using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Entities.Items;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.TableConfigurations
{
    public class ShoppingListItemConfig : IEntityTypeConfiguration<ShoppingListItem>
    {
        public void Configure(EntityTypeBuilder<ShoppingListItem> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.ShoppingList)
                   .WithMany(l => l.Items)
                   .HasForeignKey(x => x.ShoppingListId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.Name)
                   .HasMaxLength(200)
                   .IsRequired();

            builder.Property(x => x.Quantity)
                   .IsRequired();

            builder.Property(x => x.Price)
                   .IsRequired();

            builder.Property(x => x.IsBought)
                   .IsRequired();
        }
    }
}
