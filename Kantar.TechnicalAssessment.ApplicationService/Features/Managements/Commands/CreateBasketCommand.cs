namespace Kantar.TechnicalAssessment.ApplicationService.Features.Managements.Commands
{
    public class CreateBasketCommand
    {
        public Guid BasketId { get; set; }
        public List<BasketItemCmd> Items { get; set; } = [];

        public class BasketItemCmd
        {
            public Guid ItemId { get; set; }
            public long Quantity { get; set; }
        }
    }
}
