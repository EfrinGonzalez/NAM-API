using NAM_API.Models;

namespace NAM_API.Services.Interfaces
{
    public interface IPythonApiService
    {
        Task<List<Portfolio>> GetPortfoliosAsync();
        Task<List<Holding>> GetHoldingsAsync(string portfolioName);
        Task<Cash> GetCashAsync(string portfolioName);
    }
}
