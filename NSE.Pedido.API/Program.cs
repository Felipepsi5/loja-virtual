using MediatR;
using NSE.Pedidos.API.Configuration;
using NSE.Web.Api.Core.Identidade;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddApiConfiguration(builder.Configuration);
builder.Services.AddJwtConfiguration(builder.Configuration);
builder.Services.AddMediatR(typeof(Program));
builder.Services.RegisterServices();

builder.Services.AddMessageBusConfiguration(builder.Configuration);
builder.Services.AddSwaggerConfiguration();

var app = builder.Build();

app.UseApiConfiguration(builder.Environment);

app.Run();

