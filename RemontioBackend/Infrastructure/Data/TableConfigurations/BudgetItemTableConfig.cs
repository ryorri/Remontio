using Domain.Entities.Items;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.TableConfigurations
{
    public class BudgetItemTableConfig : IEntityTypeConfiguration<BudgetItem>
    {
        public void Configure(EntityTypeBuilder<BudgetItem> builder)
        {
            builder.ToTable("BudgetItem");
            
            builder.HasKey(i => i.Id);
            
            builder.Property(i => i.Name).IsRequired();
            builder.Property(i => i.Description).IsRequired();
            builder.Property(i => i.Category).IsRequired();
            builder.Property(i => i.Price).IsRequired();
            builder.Property(i => i.Total).IsRequired();
            builder.Property(i => i.EstimatedPrice).IsRequired();
            builder.Property(i => i.IsCompleted).IsRequired();
            builder.Property(i => i.BudgetId).IsRequired();
        }
    }
}
