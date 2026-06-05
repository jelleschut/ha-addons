using HAShoppingList.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HAShoppingList.Infrastructure.Persistence.Configurations;

public class RecipeItemConfiguration : IEntityTypeConfiguration<RecipeItem>
{
    public void Configure(EntityTypeBuilder<RecipeItem> builder)
    {
        builder.HasKey(ri => ri.Id);
        builder.Property(ri => ri.Quantity).HasColumnType("decimal(8,2)");
        builder.Property(ri => ri.Unit).HasConversion<string>();

        builder.HasOne(ri => ri.Recipe)
            .WithMany(r => r.Items)
            .HasForeignKey(ri => ri.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ri => ri.Product)
            .WithMany()
            .HasForeignKey(ri => ri.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
