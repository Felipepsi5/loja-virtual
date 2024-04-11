using NSE.Identidade.API.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.AddEnvirommentConfig();

builder.Services.AddApiConfiguration();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerConfiguration();

builder.Services.AddIdentityConfiguration(builder.Configuration);

builder.Services.AddMessageBusConfiguration(builder.Configuration);

var app = builder.Build();

app.UseApiConfiguration(builder.Environment);

app.Run();
