using App.Application.Contracts.Caching;
using App.Application.Extensions;
using App.Bus;
using App.Caching;
using App.Persistance.Extensions;
using CleanApp.API.ExceptionHandlers;
using CleanApp.API.Extensions;
using CleanApp.API.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithFiltersExt().AddSwaggerGenExt().AddExceptionHandlerExt().AddCachingExt();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddRepositories(builder.Configuration).AddServices(builder.Configuration)
    .AddBusExt(builder.Configuration);

var app = builder.Build();

app.UseConfigurePipelineExt();

app.MapControllers();

app.Run();
