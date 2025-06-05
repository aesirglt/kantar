namespace Kantar.TechnicalAssessment.Domain.Features
{
    public record BasketItem : EntityBase<BasketItem>
    {
        public Guid ItemId { get; set; }
        public Guid BasketId { get; set; }
        public long Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discounts { get; set; }
        public decimal SubTotal => UnitPrice * Quantity;
        public decimal Total => SubTotal - Discounts;

        public Item Item { get; set; } = null!;
        public Basket Basket { get; set; } = null!;
    }
}
