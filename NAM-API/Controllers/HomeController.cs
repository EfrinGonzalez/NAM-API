namespace NAM_API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {     
            [HttpGet]
            public IActionResult Get()
            {
            var links = new List<object>
            {
                new { Description = "Get the cash fraction", Href = Url.Action("GetCashFraction", "Cash", null, Request.Scheme) },
                new { Description = "Get the portfolio by stock (replace AMZN with the desired stock name)", Href = Url.Action("GetPortfoliosByStock", "Portfolio", new { stockId = "AMZN" }, Request.Scheme) }
            };

            var response = new
            {
                Message = "Welcome to the NAM API. Use the link below to interact with API.",
                Links = links
            };

            return Ok(response);
        }
        
    }
}
