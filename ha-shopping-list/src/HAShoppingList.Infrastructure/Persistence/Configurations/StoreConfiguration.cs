using HAShoppingList.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HAShoppingList.Infrastructure.Persistence.Configurations;

public class StoreConfiguration : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).IsRequired().HasMaxLength(100);
        builder.Property(s => s.DisplayOrder).IsRequired();

        builder.HasData(
            new Store { Id = 1, Name = "Albert Heijn", DisplayOrder = 1 },
            new Store { Id = 2, Name = "Lidl", DisplayOrder = 2 },
            new Store { Id = 3, Name = "Groenteboer", DisplayOrder = 3 },
            new Store { Id = 4, Name = "Kruidvat", DisplayOrder = 4 },
            new Store { Id = 5, Name = "Action", DisplayOrder = 5 },
            new Store { Id = 6, Name = "HEMA", DisplayOrder = 6 }
        );
    }
}