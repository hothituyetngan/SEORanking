using SEORanking.Application.Interfaces;
using SEORanking.Application.Services;
using SEORanking.Domain.Interfaces;
using SEORanking.Infrastructure.SearchEngines;
using SEORanking.Infrastructure.Caching;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMemoryCache();

// Register Application Layer Services
builder.Services.AddScoped<ISearchService, SearchService>();

// Register Caching Service
builder.Services.AddSingleton<ICacheService, InMemoryCacheService>();

// Register Search Engine Implementations
builder.Services.AddScoped<ISearchEngine, GoogleSearchEngine>();
builder.Services.AddScoped<ISearchEngine, BingSearchEngine>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
