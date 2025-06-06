namespace Kantar.TechnicalAssessment.ApplicationService.ViewModels
{
    public class BasketResumeViewModel
    {
        public Guid Id { get; set; }
        public decimal Discounts { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; internal set; }
    }
}
