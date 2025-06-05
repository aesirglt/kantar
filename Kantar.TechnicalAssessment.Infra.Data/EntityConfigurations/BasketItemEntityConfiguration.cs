using Kantar.TechnicalAssessment.Domain.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kantar.TechnicalAssessment.Infra.Data.EntityConfigurations
{
    public class BasketBasketItemEntityConfiguration : IEntityTypeConfiguration<BasketItem>
    {
        const string TABLE_NAME = "BasketItems";

        public void Configure(EntityTypeBuilder<BasketItem> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.Property(e => e.CreatedAt).IsRequired();
        }
    }

}
