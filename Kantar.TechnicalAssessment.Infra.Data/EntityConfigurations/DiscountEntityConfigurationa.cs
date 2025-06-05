using Kantar.TechnicalAssessment.Domain.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kantar.TechnicalAssessment.Infra.Data.EntityConfigurations
{
    public class DiscountEntityConfigurationa : IEntityTypeConfiguration<Discount>
    {
        const string TABLE_NAME = "Discounts";

        public void Configure(EntityTypeBuilder<Discount> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.Property(e => e.CreatedAt).IsRequired();
            
            List<Discount> discounts = [
                new ()
                {
                    Id = Guid.NewGuid(),
                    ItemId = BasicValue.AppleId,
                    CreatedAt = DateTime.UtcNow,
                    Until = DateTime.UtcNow.AddDays(7),
                    Name = "Apple week discount",
                    Description = "Get 10% off on apples for one week.",
                    Value = 10.0m,
                    DiscountType = Domain.Enums.DiscountType.Percentage,
                },
                new ()
                {
                    Id = Guid.NewGuid(),
                    ItemId = BasicValue.SoupId,
                    CreatedAt = DateTime.UtcNow,
                    Until = DateTime.UtcNow.AddDays(30),
                    Name = "Bread discount",
                    Description = "Buy 2 tins of soup and get a loaf of bread for half price.",
                    Value = 2m,
                    DiscountType = Domain.Enums.DiscountType.ConditionalDivision,
                    ItemConditionalId = BasicValue.BreadId,
                },
            ];

            builder.HasData(discounts);
        }
    }

}
