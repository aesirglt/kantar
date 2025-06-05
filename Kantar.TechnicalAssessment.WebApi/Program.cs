using Kantar.TechnicalAssessment.Infra.Data.Contexts;
using Kantar.TechnicalAssessment.WebApi.Dependencies;
using Kantar.TechnicalAssessment.WebApi.Endpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<GroceryShoppingContext>(opt => opt.UseInMemoryDatabase("InMemoryDatabase"));

builder.Services.AddInfraData()
    .AddDomainServices()
    .AddApplicationServices();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapBasketEndpoints();

app.UseHttpsRedirection();

app.Run();