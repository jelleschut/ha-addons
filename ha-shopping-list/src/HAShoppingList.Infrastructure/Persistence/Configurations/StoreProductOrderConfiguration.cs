using HAShoppingList.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HAShoppingList.Infrastructure.Persistence.Configurations;

public class StoreProductOrderConfiguration : IEntityTypeConfiguration<StoreProductOrder>
{
    public void Configure(EntityTypeBuilder<StoreProductOrder> builder)
    {
        builder.HasKey(spo => new { spo.StoreId, spo.ProductId });
        
        builder.HasOne(spo => spo.Store)
            .WithMany(s => s.ProductOrders)
            .HasForeignKey(spo => spo.StoreId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(spo => spo.Product)
            .WithMany(p => p.StoreOrders)
            .HasForeignKey(spo => spo.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}