using NSE.Identidade.API.Configuration;
using NSE.WebApp.MVC.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.AddEnvirommentConfig();

builder.Services.AddIdentityConfiguration();

builder.Services.AddMvcConfiguration(builder.Configuration);

builder.Services.RegisterServices(builder.Configuration);

var app = builder.Build();

app.UseApiConfiguration(builder.Environment);

app.Run();
