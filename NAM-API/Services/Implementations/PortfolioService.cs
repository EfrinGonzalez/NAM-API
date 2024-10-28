namespace NAM_API.Services.Implementations
{
    using NAM_API.Models;
    using NAM_API.Services.Interfaces;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Caching.Memory;

    public class PortfolioService : IPortfolioService
    {
        private readonly IPythonApiService _pythonApiService;
        private readonly IMemoryCache _cache;

        public PortfolioService(IPythonApiService pythonApiService, IMemoryCache cache)
        {
            _pythonApiService = pythonApiService;
            _cache = cache;
        }

        public async Task<List<string>> GetPortfoliosByStockAsync(string stockId)
        {
            return await _cache.GetOrCreateAsync($"portfoliosByStock_{stockId}", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5); // Cache for 5 minutes

                try
                {
                    List<Portfolio> portfolios = await _pythonApiService.GetPortfoliosAsync();
                    ConcurrentBag<string> result = new ConcurrentBag<string>();

                    await Parallel.ForEachAsync(portfolios.Where(p => !p.IsDisabled), async (portfolio, _) =>
                    {
                        try
                        {
                            List<Holding> holdings = await _pythonApiService.GetHoldingsAsync(portfolio.Name);
                            if (holdings.Any(h => h.StockId == stockId))
                            {
                                result.Add(portfolio.Name);
                            }
                        }
                        catch (Exception ex)
                        {
                                throw new ApplicationException("An error occurred while fetching portfolios by stock.", ex);
                        }
                    });

                    return result.ToList();
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("An error occurred while fetching portfolios by stock.", ex);
                }
            });
        }

        public async Task<Dictionary<string, decimal>> GetCashFractionAsync()
        {
            return await _cache.GetOrCreateAsync("cashFraction", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5); // Cache for 5 minutes
                try
                {
                    List<Portfolio> portfolios = await _pythonApiService.GetPortfoliosAsync();
                    ConcurrentDictionary<string, decimal> result = new ConcurrentDictionary<string, decimal>();

                    await Parallel.ForEachAsync(portfolios.Where(p => !p.IsDisabled), async (portfolio, _) =>
                    {
                        try
                        {
                            List<Holding> holdings = await _pythonApiService.GetHoldingsAsync(portfolio.Name);
                            Cash cash = await _pythonApiService.GetCashAsync(portfolio.Name);

                            if (cash.Value.HasValue)
                            {
                                decimal totalValue = holdings.Sum(h => h.Value) + cash.Value.Value;
                                decimal cashFraction = cash.Value.Value / totalValue;
                                result.TryAdd(portfolio.Name, cashFraction);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new ApplicationException("An error occurred while fetching cash fractions.", ex);
                        }
                    });

                    return result.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("An error occurred while fetching cash fractions.", ex);
                }
            });
        }
    }
}
