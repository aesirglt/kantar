using Kantar.TechnicalAssessment.Domain.Enums;

namespace Kantar.TechnicalAssessment.Domain.Features
{
    public record Discount : EntityBase<Discount>
    {
        public required string Name { get; set; }
        public DiscountType DiscountType { get; set; }
        public decimal Amount { get; set; }
        public Guid ItemId { get; set; }
        public Item Item { get; set; } = null!;
    }
}
