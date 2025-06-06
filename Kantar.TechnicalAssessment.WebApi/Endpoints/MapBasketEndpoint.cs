using Kantar.TechnicalAssessment.ApplicationService.Interfaces;
using Kantar.TechnicalAssessment.WebApi.Dtos;
using Kantar.TechnicalAssessment.WebApi.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace Kantar.TechnicalAssessment.WebApi.Endpoints
{
    public static class MapBasketEndpoint
    {
        public static WebApplication MapBasketEndpoints(this WebApplication app)
        {
            var grouped = app.MapGroup("/api/baskets");

            grouped.MapGet("/",
                async ([FromServices] IBasketManagmentService basketManagment,
                [FromQuery] int? skip,
                [FromQuery] int? take,
                CancellationToken cancellationToken) =>
                {
                    var basket = await basketManagment.GetAll(new()
                    {
                        Skip = skip ?? 0,
                        Take = take ?? 10
                    }, cancellationToken);
                    return EndpointBaseExt.AsResult(basket);
                })
            .WithName("GetBaskets")
            .WithOpenApi();

            grouped.MapGet("/{id:guid}",
                async (Guid id, [FromServices] IBasketManagmentService basketManagment, CancellationToken cancellationToken) =>
                {
                    var basket = await basketManagment.GetById(new() { BasketId = id }, cancellationToken);
                    return EndpointBaseExt.AsResult(basket);
                })
            .WithName("GetBasketById")
            .WithOpenApi();

            grouped.MapDelete("/{id:guid}",
                async (Guid id, [FromServices] IBasketManagmentService basketManagment, CancellationToken cancellationToken) =>
                {
                    var basket = await basketManagment.RemoveAsync(new() { BasketId = id }, cancellationToken);
                    return EndpointBaseExt.AsResult(basket);
                })
            .WithName("DeleteBasketById")
            .WithOpenApi();

            grouped.MapPost("/{id:guid}/items",
                async (Guid id,
                [FromBody] CreateBasketDto createBasketDto,
                [FromServices] IBasketManagmentService basketManagment,
                CancellationToken cancellationToken) =>
                {
                    var basket = await basketManagment.AddAsync(createBasketDto.ToCommand(id), cancellationToken);
                    return EndpointBaseExt.AsResult(basket);
                })
            .WithName("PostBasketItems")
            .WithOpenApi();

            grouped.MapDelete("/{id:guid}/items/{itemId:guid}",
                async ([FromRoute] Guid id, [FromRoute] Guid itemId,
                    [FromServices] IBasketManagmentService basketManagment,
                CancellationToken cancellationToken) =>
                {
                    var basket = await basketManagment.RemoveBasketItemAsync(new()
                    {
                        BasketId = id,
                        ItemId = itemId
                    }, cancellationToken);

                    return EndpointBaseExt.AsResult(basket);
                })
            .WithName("DeleteBasketItem")
            .WithOpenApi();

            grouped.MapPatch("/{id:guid}/items/{itemId:guid}",
                async ([FromRoute] Guid id, [FromRoute] Guid itemId,
                [FromBody] UpdateBasketItemQuantityDto updateDto,
                [FromServices] IBasketManagmentService basketManagment,
                CancellationToken cancellationToken) =>
                {
                    return EndpointBaseExt.AsResult(await basketManagment.UpdateItemQuantityAsync(new()
                    {
                        BasketId = id,
                        ItemId = itemId,
                        Quantity = (long)updateDto.Quantity
                    }, cancellationToken));
                })
            .WithName("UpdateBasketItemQuantity")
            .WithOpenApi();

            return app;
        }
    }
}
