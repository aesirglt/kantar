using Kantar.TechnicalAssessment.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kantar.TechnicalAssessment.WebApi.Endpoints
{
    public static class MapBasketEndpoint
    {
        public static WebApplication MapBasketEndpoints(this WebApplication app)
        {
            var grouped = app.MapGroup("/api/baskets");

            grouped.MapGet("/{id:guid}", async (Guid id, [FromServices] IBasketService basketService) =>
            {
                var basket = await basketService.GetBasketAsync(id);
                return basket is not null ? Results.Ok(basket) : Results.NotFound();
            });

            return app;
        }
    }
}
