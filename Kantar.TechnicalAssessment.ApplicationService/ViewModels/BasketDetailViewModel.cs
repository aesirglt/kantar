namespace Kantar.TechnicalAssessment.ApplicationService.ViewModels
{
    public class BasketItemViewModel
    {
        public Guid ItemId { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public long Quantity { get; set; }
        public decimal Total { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Discounts { get; set; }
    }

    public class Totals
    {
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public decimal Discount { get; set; }
    }

    public class BasketDetailViewModel
    {
        public Guid Id { get; set; }
        public BasketItemViewModel[] Items { get; set; } = [];
        public Totals Totals { get; set; } = null!;
    }
}
