using Microsoft.Extensions.Configuration;
using SantanderTest.API.Services;
using SantanderTest.API.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IBestStoriesCacheService, BestStoriesCacheService>();
builder.Services.AddHttpClient("BestStories");
builder.Services.AddControllers();
builder.Services.Configure<HackerNewsApiConfigurationSettings>(builder.Configuration.GetSection("HackerNewsApiConfiguration"));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
