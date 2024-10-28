using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.ObjectPool;
using NAM_API.Pools;
using NAM_API.Services.Implementations;
using NAM_API.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Read the base URL from the environment variable
//var baseUrl = builder.Configuration["PYTHON_API_BASE_URL"];
var baseUrl = builder.Configuration["PythonApi:BaseUrl"];

// Add services to the container.
builder.Services.AddHttpClient<IPythonApiService, PythonApiService>(client =>
{
    client.BaseAddress = new Uri(baseUrl);
});

// Add services to the container.
builder.Services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
builder.Services.AddSingleton<ObjectPool<HttpClient>>(provider =>
{
    var poolProvider = provider.GetRequiredService<ObjectPoolProvider>();
    return poolProvider.Create(new HttpClientPoolPolicy(baseUrl));
});

builder.Services.AddMemoryCache();

builder.Services.AddSingleton<IPythonApiService, PythonApiService>(provider =>
{
    var httpClientPool = provider.GetRequiredService<ObjectPool<HttpClient>>();
    var memoryCache = provider.GetRequiredService<IMemoryCache>();
    return new PythonApiService(httpClientPool, memoryCache);
});

builder.Services.AddScoped<IPortfolioService, PortfolioService>();

// Add memory cache services
builder.Services.AddMemoryCache();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
