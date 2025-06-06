using Kantar.TechnicalAssessment.Domain.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kantar.TechnicalAssessment.Infra.Data.EntityConfigurations
{
    public class DiscountEntityConfigurationa : IEntityTypeConfiguration<Discount>
    {
        public void Configure(EntityTypeBuilder<Discount> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.Property(e => e.CreatedAt).IsRequired();
            builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
            builder.Property(e => e.Description).HasMaxLength(500);
            builder.Property(e => e.Value).IsRequired().HasPrecision(18, 2);
            builder.Property(e => e.DiscountType).IsRequired();
            builder.Property(e => e.ItemId).IsRequired();
            builder.Property(e => e.Until).IsRequired();
            builder.Property(e => e.ItemConditionalId).IsRequired(false);

            List<Discount> discounts = [
                new ()
                {
                    Id = Guid.NewGuid(),
                    ItemId = BasicValue.AppleId,
                    CreatedAt = DateTime.UtcNow,
                    Until = DateTime.UtcNow.AddDays(7),
                    Name = "Apple week discount",
                    Description = "Get 10% off on apples for one week.",
                    Value = 10m,
                    DiscountType = Domain.Enums.DiscountType.Percentage,
                },
                new ()
                {
                    Id = Guid.NewGuid(),
                    ItemId = BasicValue.BreadId,
                    CreatedAt = DateTime.UtcNow,
                    Until = DateTime.UtcNow.AddDays(30),
                    Name = "Bread discount",
                    Description = "Buy 2 tins of soup and get a loaf of bread for half price.",
                    Value = 2m,
                    DiscountType = Domain.Enums.DiscountType.ConditionalDivision,
                    ItemConditionalId = BasicValue.SoupId,
                    ConditionalQuantity = 2
                },
            ];

            builder.HasData(discounts);
        }
    }

}
