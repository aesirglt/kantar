namespace Kantar.TechnicalAssessment.Domain.Features
{
    public record Item : EntityBase<Item>
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public virtual List<Discount> Discounts { get; set; } = [];
        public virtual List<BasketItem> BasketItems { get; set; } = [];
    }
}
