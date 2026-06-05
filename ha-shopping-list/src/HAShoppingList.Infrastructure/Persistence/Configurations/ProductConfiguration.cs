using HAShoppingList.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HAShoppingList.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
        builder.Property(p => p.DefaultQuantity).HasColumnType("decimal(8,2)");
        
        builder.HasOne(p => p.DefaultStore)
            .WithMany()
            .HasForeignKey(p => p.DefaultStoreId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}