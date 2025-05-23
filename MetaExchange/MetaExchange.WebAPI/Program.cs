using FluentValidation;
using MetaExchange.Application.Interfaces.UseCase;
using MetaExchange.Application.Services.UseCase;
using MetaExchange.SharedKernel;
using MetaExchange.WebAPI.Validator;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddMetaExchangeServices(builder.Configuration);
builder.Services.AddScoped<IOrderMatchingService, OrderMatchingService>();
builder.Services.AddValidatorsFromAssemblyContaining<OrderRequestValidator>();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();