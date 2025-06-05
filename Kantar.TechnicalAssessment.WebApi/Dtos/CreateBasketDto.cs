namespace Kantar.TechnicalAssessment.WebApi.Dtos
{
    public class CreateBasketDto
    {
        public List<CreateBasketItemDto> Items { get; set; } = [];

        public class CreateBasketItemDto
        {
            public Guid ItemId { get; set; }
            public long Quantity { get; set; }
        }
    }
}
