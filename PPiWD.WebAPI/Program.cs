using Microsoft.EntityFrameworkCore;
using PPiWD.WebAPI.Database;
using PPiWD.WebAPI.Endpoints;
using PPiWD.WebAPI.Extensions;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddDbContext<DatabaseContext>(options => options.UseInMemoryDatabase("items")); //TODO: remove this line and Uninstall-Package Microsoft.EntityFrameworkCore.InMemory
builder.Services.AddSwaggerGen();
builder.Services.AddAppServices();

//builder.Services.AddAppServices();

builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultPolicy", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
//app.UseAuthorization();

// Endpoints
app.MapMeasurementsEndpoints();
app.Run();