using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.TableConfigurations
{
    public class BudgetTableConfig : IEntityTypeConfiguration<Budget>
    {
        public void Configure(EntityTypeBuilder<Budget> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(50);
            builder.Property(p => p.Description).HasMaxLength(500);
            builder.Property(p => p.CreateAt).IsRequired();
            builder.Property(p => p.EstimatedPrice).IsRequired();

            builder.HasMany(b => b.Items)
                .WithOne(i => i.Budget)
                .HasForeignKey(i => i.BudgetId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.User)
                .WithMany(u => u.Budgets)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Room)
                    .WithMany(u => u.Budgets)
                    .HasForeignKey(p => p.RoomId)
                    .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(p => p.Project)
                    .WithMany(u => u.Budgets)
                    .HasForeignKey(p => p.ProjectId)
                    .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
