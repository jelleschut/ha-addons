using HAShoppingList.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HAShoppingList.Infrastructure.Persistence.Configurations;

public class ShoppingListItemConfiguration : IEntityTypeConfiguration<ShoppingListItem>
{
    public void Configure(EntityTypeBuilder<ShoppingListItem> builder)
    {
        builder.HasKey(sli => sli.Id);
        builder.Property(sli => sli.Quantity).HasColumnType("decimal(8,2)");
        builder.Property(sli => sli.Note).HasMaxLength(500);
        builder.Property(sli => sli.Unit).HasConversion<string>();
        
        builder.HasOne(sli => sli.Product)
            .WithMany()
            .HasForeignKey(sli => sli.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(sli => sli.Store)
            .WithMany(s => s.ShoppingListItems)
            .HasForeignKey(sli => sli.StoreId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}