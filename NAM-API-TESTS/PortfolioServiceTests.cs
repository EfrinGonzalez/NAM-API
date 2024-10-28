using Moq;
using NAM_API.Models;
using NAM_API.Services.Implementations;
using NAM_API.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.ObjectPool;
using Moq.Protected;
using System.Net;

namespace NAM_API.Tests
{
    [TestFixture]
    public class PortfolioServiceTests
    {        
        private Mock<IPythonApiService> _pythonApiServiceMock;
        private Mock<ObjectPool<HttpClient>> _httpClientPoolMock;
        private IMemoryCache _cache;
        private PortfolioService _portfolioService;

        [SetUp]
        public void SetUp()
        {
            _pythonApiServiceMock = new Mock<IPythonApiService>();
            _httpClientPoolMock = new Mock<ObjectPool<HttpClient>>();
            _cache = new MemoryCache(new MemoryCacheOptions());

            var httpClientHandlerMock = new Mock<HttpMessageHandler>();
            httpClientHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("[]"), // Mocked empty JSON array response
                });

            var httpClient = new HttpClient(httpClientHandlerMock.Object)
            {
                BaseAddress = new System.Uri("https://api.example.com")
            };
            _httpClientPoolMock.Setup(p => p.Get()).Returns(httpClient);

            _portfolioService = new PortfolioService(_pythonApiServiceMock.Object, _cache);
        }

        [TearDown]
        public void TearDown()
        {
            _cache.Dispose();
        }

        [Test]
        public async Task GetPortfoliosByStockAsync_ShouldReturnPortfolios_WhenStockExists()
        {
            // Arrange
            var stockId = "AAPL";
            var portfolios = new List<Portfolio>
            {
                new Portfolio { Name = "PORTFOLIO_A", IsDisabled = false },
                new Portfolio { Name = "PORTFOLIO_B", IsDisabled = false }
            };
            var holdings = new List<Holding>
            {
                new Holding { StockId = stockId, Value = 100 }
            };

            _pythonApiServiceMock.Setup(p => p.GetPortfoliosAsync()).ReturnsAsync(portfolios);
            _pythonApiServiceMock.Setup(p => p.GetHoldingsAsync(It.IsAny<string>())).ReturnsAsync(holdings);

            // Act
            var result = await _portfolioService.GetPortfoliosByStockAsync(stockId);

            // Assert
            Assert.That(result, Is.EquivalentTo(new List<string> { "PORTFOLIO_A", "PORTFOLIO_B" }));
        }

        [Test]
        public async Task GetCashFractionAsync_ShouldReturnCorrectCashFraction()
        {
            // Arrange
            var portfolios = new List<Portfolio>
            {
                new Portfolio { Name = "PortfolioA", IsDisabled = false }
            };
            var holdings = new List<Holding>
            {
                new Holding { StockId = "AAPL", Value = 100 }
            };
            var cash = new Cash { Value = 50 };

            _pythonApiServiceMock.Setup(p => p.GetPortfoliosAsync()).ReturnsAsync(portfolios);
            _pythonApiServiceMock.Setup(p => p.GetHoldingsAsync(It.IsAny<string>())).ReturnsAsync(holdings);
            _pythonApiServiceMock.Setup(p => p.GetCashAsync(It.IsAny<string>())).ReturnsAsync(cash);

            // Act
            var result = await _portfolioService.GetCashFractionAsync();

            // Assert
            Assert.That(result["PortfolioA"], Is.EqualTo(0.3333333333333333333333333333m)); // 50 / (100 + 50)
        }
    }
}
