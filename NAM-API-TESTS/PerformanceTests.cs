using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.ObjectPool;
using Moq;
using Moq.Protected;
using NAM_API.Models;
using NAM_API.Services.Implementations;
using NAM_API.Services.Interfaces;
using System.Diagnostics;
using System.Net;

namespace NAM_API_TESTS
{
    [TestFixture]
    public class PortfolioServicePerformanceTests
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

            var pythonApiService = new PythonApiService(_httpClientPoolMock.Object, _cache);
            _portfolioService = new PortfolioService(_pythonApiServiceMock.Object, _cache);
        }

        [TearDown]
        public void TearDown()
        {
            _cache.Dispose();
        }

        [Test]
        public async Task GetPortfoliosByStockAsync_PerformanceTest()
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

            var stopwatch = new Stopwatch();
            int numberOfCalls = 1000000;

            // Act
            stopwatch.Start();
            var tasks = new List<Task>();
            for (int i = 0; i < numberOfCalls; i++)
            {
                tasks.Add(_portfolioService.GetPortfoliosByStockAsync(stockId));
            }
            await Task.WhenAll(tasks);
            stopwatch.Stop();

            // Assert
            Assert.Pass($"Performance test completed in {stopwatch.ElapsedMilliseconds} ms for {numberOfCalls} calls.");
        }

        [Test]
        public async Task GetCashFractionAsync_PerformanceTest()
        {
            // Arrange
            var portfolios = new List<Portfolio>
            {
                new Portfolio { Name = "PORTFOLIO_A", IsDisabled = false }
            };
            var holdings = new List<Holding>
            {
                new Holding { StockId = "AAPL", Value = 100 }
            };
            var cash = new Cash { Value = 50 };

            _pythonApiServiceMock.Setup(p => p.GetPortfoliosAsync()).ReturnsAsync(portfolios);
            _pythonApiServiceMock.Setup(p => p.GetHoldingsAsync(It.IsAny<string>())).ReturnsAsync(holdings);
            _pythonApiServiceMock.Setup(p => p.GetCashAsync(It.IsAny<string>())).ReturnsAsync(cash);

            var stopwatch = new Stopwatch();
            int numberOfCalls = 1000000;

            // Act
            stopwatch.Start();
            var tasks = new List<Task>();
            for (int i = 0; i < numberOfCalls; i++)
            {
                tasks.Add(_portfolioService.GetCashFractionAsync());
            }
            await Task.WhenAll(tasks);
            stopwatch.Stop();

            // Assert
            Assert.Pass($"Performance test completed in {stopwatch.ElapsedMilliseconds} ms for {numberOfCalls} calls.");
        }
    }
}
