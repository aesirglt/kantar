using Kantar.TechnicalAssessment.Domain.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kantar.TechnicalAssessment.Infra.Data.EntityConfigurations
{
    public class BasketEntityConfiguration : IEntityTypeConfiguration<Basket>
    {
        const string TABLE_NAME = "Baskets";

        public void Configure(EntityTypeBuilder<Basket> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.Property(e => e.CreatedAt).IsRequired();

            builder.HasMany(e => e.BasketItems)
                   .WithOne(e => e.Basket)
                   .HasForeignKey(a => a.BasketId);
        }
    }

}
