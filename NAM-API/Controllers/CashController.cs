namespace NAM_API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using NAM_API.Services.Interfaces;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/[controller]")]
    public class CashController : ControllerBase
    {
        private readonly IPortfolioService _portfolioService;

        public CashController(IPortfolioService portfolioService)
        {
            _portfolioService = portfolioService;
        }

        [HttpGet("fraction")]
        public async Task<IActionResult> GetCashFraction()
        {
            var result = await _portfolioService.GetCashFractionAsync();
            return Ok(result);
        }
    }

}
