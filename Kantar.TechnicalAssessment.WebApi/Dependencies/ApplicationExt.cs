using Kantar.TechnicalAssessment.ApplicationService.Features.Baskets;
using Kantar.TechnicalAssessment.ApplicationService.Features.Managements;
using Kantar.TechnicalAssessment.ApplicationService.Interfaces;
using Kantar.TechnicalAssessment.Domain.Features;
using Kantar.TechnicalAssessment.Domain.Interfaces.DomainServices;
using Kantar.TechnicalAssessment.Domain.Interfaces.Repositories;
using Kantar.TechnicalAssessment.Domain.Interfaces.Services;
using Kantar.TechnicalAssessment.DomainServices;
using Kantar.TechnicalAssessment.Infra.Data.Repositories;

namespace Kantar.TechnicalAssessment.WebApi.Dependencies
{
    public static class ApplicationExt
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
            => services.AddSingleton<IApplyDiscountDomainService, ApplyDiscountDomainService>();

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
            => services.AddScoped<IBasketService, BasketService>()
            .AddScoped<IBasketItemService, BasketItemService>()
            .AddScoped<IBasketManagmentService, BasketManagmentService>();

        public static IServiceCollection AddInfraData(this IServiceCollection services)
            => services.AddSingleton<IBaseRepository<Basket>, BaseRepository<Basket>>()
            .AddSingleton<IBaseRepository<BasketItem>, BaseRepository<BasketItem>>()
            .AddSingleton<IBaseRepository<Discount>, BaseRepository<Discount>>()
            .AddSingleton<IBaseRepository<Item>, BaseRepository<Item>>();
    }
}
