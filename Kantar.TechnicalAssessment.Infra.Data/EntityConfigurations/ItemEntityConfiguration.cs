using Kantar.TechnicalAssessment.Domain.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kantar.TechnicalAssessment.Infra.Data.EntityConfigurations
{
    public class ItemEntityConfiguration : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.Property(e => e.CreatedAt).IsRequired();
            builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
            builder.Property(e => e.Price).IsRequired().HasPrecision(18, 2);

            builder.HasMany(e => e.Discounts)
                   .WithOne(e => e.Item)
                   .HasForeignKey(a => a.ItemId);

            builder.HasMany(e => e.BasketItems)
                   .WithOne(e => e.Item)
                   .HasForeignKey(a => a.ItemId);

            List<Item> items = [
                new () { Id = BasicValue.AppleId, Price = 0.30m,  CreatedAt = DateTime.UtcNow, Name = "Apples"  },
                new () { Id = BasicValue.SoupId, Price = 1.99m,  CreatedAt = DateTime.UtcNow, Name = "Soup"   },
                new () { Id = BasicValue.BreadId, Price = 1.20m,  CreatedAt = DateTime.UtcNow, Name = "Bread"   },
                new () { Id = Guid.NewGuid(), Price = 0.80m,  CreatedAt = DateTime.UtcNow, Name = "Milk"    },
                new () { Id = Guid.NewGuid(), Price = 0.40m,  CreatedAt = DateTime.UtcNow, Name = "Pears"   },
                new () { Id = Guid.NewGuid(), Price = 1.00m,  CreatedAt = DateTime.UtcNow, Name = "Eggplant"},
                new () { Id = Guid.NewGuid(), Price = 1.20m,  CreatedAt = DateTime.UtcNow, Name = "Cucumber"},
            ];

            builder.HasData(items);
        }
    }

}
