namespace NAM_API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using NAM_API.Services.Interfaces;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/[controller]")]
    public class PortfolioController : ControllerBase
    {
        private readonly IPortfolioService _portfolioService;

        public PortfolioController(IPortfolioService portfolioService)
        {
            _portfolioService = portfolioService;
        }

        [HttpGet("stock/{stockId}")]
        public async Task<IActionResult> GetPortfoliosByStock(string stockId)
        {
            var result = await _portfolioService.GetPortfoliosByStockAsync(stockId);
            return Ok(result);
        }
    }

}
