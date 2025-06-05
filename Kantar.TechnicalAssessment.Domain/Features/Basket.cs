namespace Kantar.TechnicalAssessment.Domain.Features
{
    public record Basket : EntityBase<Basket>
    {
        public decimal Discounts => BasketItems.Sum(x => x.Discounts);
        public decimal Subtotal => BasketItems.Sum(i => i.SubTotal);
        public decimal Total => BasketItems.Sum(i => i.Total);
        public virtual List<BasketItem> BasketItems { get; set; } = [];
    }
}
