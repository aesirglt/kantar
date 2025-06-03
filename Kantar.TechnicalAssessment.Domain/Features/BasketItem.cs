namespace Kantar.TechnicalAssessment.Domain.Features
{
    public record BasketItem : EntityBase<BasketItem>
    {
        public Guid ItemId { get; set; }
        public Guid BasketId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public Item Item { get; set; } = null!;
        public Basket Basket { get; set; } = null!;
    }
}
