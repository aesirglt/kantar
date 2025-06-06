namespace Kantar.TechnicalAssessment.ApplicationService.Features.Managements.Commands
{
    public class UpdateBasketItemQuantityCommand
    {
        public Guid ItemId { get; set; }
        public Guid BasketId { get; set; }
        public long Quantity { get; set; }
    }
}
