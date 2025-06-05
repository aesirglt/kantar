using Kantar.TechnicalAssessment.ApplicationService.Features.Managements.Commands;
using Kantar.TechnicalAssessment.WebApi.Dtos;

namespace Kantar.TechnicalAssessment.WebApi.Mappers
{
    public static class CreateBasketMapper
    {
        public static CreateBasketCommand ToCommand(this CreateBasketDto createBasketDto, Guid id) => new()
        {
            BasketId = id,
            Items = [.. createBasketDto.Items.Select(item => new CreateBasketCommand.BasketItemCmd
            {
                ItemId = item.ItemId,
                Quantity = item.Quantity
            })]
        };
    }
}
