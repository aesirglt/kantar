using Kantar.TechnicalAssessment.Domain.Features;

namespace Kantar.TechnicalAssessment.Domain.Interfaces.Services
{
    public interface IBasketService
    {
        Task<Basket?> GetBasketAsync(Guid id);
        Task<Basket> CreateBasketAsync(Basket basket);
        Task<Basket> UpdateBasketAsync(Basket basket);
        Task<bool> DeleteBasketAsync(Guid id);
        Task<Basket> AddItemToBasketAsync(Guid basketId, BasketItem item);
        Task<Basket> RemoveItemFromBasketAsync(Guid basketId, Guid itemId);
    }
}
