using Kantar.TechnicalAssessment.ApplicationService.Interfaces;
using Kantar.TechnicalAssessment.Domain.Interfaces.Services;
using Kantar.TechnicalAssessment.WebApi.Dtos;
using Kantar.TechnicalAssessment.WebApi.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace Kantar.TechnicalAssessment.WebApi.Endpoints
{
    public static class MapBasketEndpoint
    {
        public static WebApplication MapBasketEndpoints(this WebApplication app)
        {
            var grouped = app.MapGroup("/api/baskets")
                .WithName("Baskets")
                .WithOpenApi();

            grouped.MapGet("/{id:guid}",
                async (Guid id, [FromServices] IBasketService basketService, CancellationToken cancellationToken) =>
                {
                    var basket = await basketService.GetAsync(id, cancellationToken);
                    return basket.IsOk ? Results.Ok(basket) : Results.NotFound();
                });

            grouped.MapPost("/{id:guid}/items",
                async (Guid id,
                [FromBody] CreateBasketDto createBasketDto,
                [FromServices] IBasketManagmentService basketManagment,
                CancellationToken cancellationToken) =>
                {
                    var basket = await basketManagment.AddAsync(createBasketDto.ToCommand(id), cancellationToken);
                    return basket.IsOk ? Results.Ok(basket) : Results.NotFound();
                });

            return app;
        }
    }
}
