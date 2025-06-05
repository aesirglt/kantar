namespace Kantar.TechnicalAssessment.Domain.Features
{
    public record Basket : EntityBase<Basket>
    {
        public decimal Discounts { get; set; }
        public decimal Total => Subtotal - Discounts;
        public decimal Subtotal => BasketItems.Sum(i => i.UnitPrice);
        public List<BasketItem> BasketItems { get; set; } = [];
    }
}
