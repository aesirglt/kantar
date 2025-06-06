namespace Kantar.TechnicalAssessment.WebApi.Dtos
{
    public class CreateBasketDto
    {
        public List<CreateBasketItemDto> Items { get; set; } = [];

        public class CreateBasketItemDto
        {
            public Guid ItemId { get; set; }
            public ulong Quantity { get; set; }
        }
    }
}
