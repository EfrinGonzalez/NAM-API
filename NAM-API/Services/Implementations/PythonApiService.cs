namespace NAM_API.Services.Implementations
{
    using NAM_API.Models;
    using NAM_API.Services.Interfaces;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.ObjectPool;

    /// <summary>
    /// Service dedicated to fetching data from an External Python API.
    /// </summary>
    public class PythonApiService : IPythonApiService
    {
        private readonly ObjectPool<HttpClient> _httpClientPool;
        private readonly IMemoryCache _cache;

        public PythonApiService(ObjectPool<HttpClient> httpClientPool, IMemoryCache cache)
        {
            _httpClientPool = httpClientPool;
            _cache = cache;
        }

        public async Task<List<Portfolio>> GetPortfoliosAsync()
        {
            HttpClient client = _httpClientPool.Get();
            try
            {
                return await _cache.GetOrCreateAsync("portfolios", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5); // Cache for 5 minutes
                    return await client.GetFromJsonAsync<List<Portfolio>>("portfolios");
                });
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while fetching portfolios.", ex);
            }
            finally
            {
                _httpClientPool.Return(client);
            }          
        }

        public async Task<List<Holding>> GetHoldingsAsync(string portfolioName)
        {
            HttpClient client = _httpClientPool.Get();
            try
            {
                return await _cache.GetOrCreateAsync($"holdings_{portfolioName}", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5); 
                    string holdingsResponse = await client.GetStringAsync($"{portfolioName}/holdings");
                    return JsonConvert.DeserializeObject<List<Holding>>(holdingsResponse);
                });
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while fetching holdings.", ex);
            }
            finally
            {
                _httpClientPool.Return(client);
            }           
        }

        public async Task<Cash> GetCashAsync(string portfolioName)
        {
            HttpClient client = _httpClientPool.Get();
            try
            {
                return await _cache.GetOrCreateAsync($"cash_{portfolioName}", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                    return await client.GetFromJsonAsync<Cash>($"{portfolioName}/cash");
                });
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while fetching cash.", ex);

            }
            finally
            {
                _httpClientPool.Return(client);
            }           
        }
    }
}
