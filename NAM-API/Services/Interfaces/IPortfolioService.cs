namespace NAM_API.Services.Interfaces
{
    public interface IPortfolioService
    {
        Task<List<string>> GetPortfoliosByStockAsync(string stockId);
        Task<Dictionary<string, decimal>> GetCashFractionAsync();
    }
}
