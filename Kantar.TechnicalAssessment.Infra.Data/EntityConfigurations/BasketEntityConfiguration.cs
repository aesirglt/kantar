using Kantar.TechnicalAssessment.Domain.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kantar.TechnicalAssessment.Infra.Data.EntityConfigurations
{
    public class BasketEntityConfiguration : IEntityTypeConfiguration<Basket>
    {
        public void Configure(EntityTypeBuilder<Basket> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.Property(e => e.CreatedAt).IsRequired();
            builder.Ignore(e => e.Discounts);
            builder.Ignore(e => e.Total);
            builder.Ignore(e => e.Subtotal);

            builder.HasMany(e => e.BasketItems)
                       .WithOne(e => e.Basket)
                       .HasForeignKey(a => a.BasketId);
        }
    }

}
