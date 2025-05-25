using FluentValidation;
using MetaExchange.Application.Interfaces.UseCase;
using MetaExchange.Application.Services.UseCase;
using MetaExchange.SharedKernel;
using MetaExchange.WebAPI.Extensions;
using MetaExchange.WebAPI.Validator;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddMetaExchangeServices(builder.Configuration);
builder.Services.AddScoped<IOrderMatchingService, OrderMatchingService>();
builder.Services.AddValidatorsFromAssemblyContaining<OrderRequestValidator>();

builder.Services
    .AddProblemDetails()
    .ConfigureCustomModelStateResponses();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program
{ }