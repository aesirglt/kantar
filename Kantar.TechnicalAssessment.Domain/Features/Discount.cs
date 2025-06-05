using Kantar.TechnicalAssessment.Domain.Enums;

namespace Kantar.TechnicalAssessment.Domain.Features
{
    public record Discount : EntityBase<Discount>
    {
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public DiscountType DiscountType { get; set; }
        public decimal Value { get; set; }
        public Guid ItemId { get; set; }
        public virtual Item Item { get; set; } = null!;
        public DateTime Until { get; set; }
        public Guid? ItemConditionalId { get; set; }
        public long ConditionalQuantity { get; set; }
    }
}
