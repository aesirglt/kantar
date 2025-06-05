using Kantar.TechnicalAssessment.Domain.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kantar.TechnicalAssessment.Infra.Data.EntityConfigurations
{
    public class BasketItemEntityConfiguration : IEntityTypeConfiguration<BasketItem>
    {
        public void Configure(EntityTypeBuilder<BasketItem> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.Property(e => e.CreatedAt).IsRequired();

            builder.Property(e => e.ItemId).IsRequired();
            builder.Property(e => e.BasketId).IsRequired();
            builder.Property(e => e.Quantity).IsRequired();
            builder.Property(e => e.UnitPrice).IsRequired().HasPrecision(18, 2);
            builder.Property(e => e.Discounts).IsRequired().HasPrecision(18, 2);
            builder.Ignore(e => e.SubTotal);
            builder.Ignore(e => e.Total);
        }
    }

}
