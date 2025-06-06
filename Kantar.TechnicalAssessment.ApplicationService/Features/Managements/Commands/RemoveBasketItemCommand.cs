namespace Kantar.TechnicalAssessment.ApplicationService.Features.Managements.Commands
{
    public class RemoveBasketItemCommand
    {
        public Guid BasketId { get; set; }
        public Guid ItemId { get; set; }
    }
}
