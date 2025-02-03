using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;
using CryptoCurrencyExchange.Services;

namespace CryptoCurrencyExchange.CryptoCurrencyExchange.Tests
{
	public class CryptoExchangeServiceTests
	{
		private readonly CryptoExchangeService _cryptoService;
		private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
		private readonly Mock<HttpMessageHandler> _handlerMock;
		private readonly IConfiguration _configuration;

		public CryptoExchangeServiceTests()
		{
			var settings = new Dictionary<string, string>
			{
				{ "ApiKeys:CoinMarketCap", "FAKE_COINMARKETCAP_KEY" },
				{ "ApiKeys:ExchangeRates", "FAKE_EXCHANGERATES_KEY" }
			};

			_configuration = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
			_httpClientFactoryMock = new Mock<IHttpClientFactory>();
			_handlerMock = new Mock<HttpMessageHandler>();

			var client = new HttpClient(_handlerMock.Object);
			_httpClientFactoryMock.Setup(m => m.CreateClient(It.IsAny<string>())).Returns(client);

			_cryptoService = new CryptoExchangeService(_httpClientFactoryMock.Object, _configuration);
		}

		[Fact]
		public async Task GetCryptoRatesAsync_Returns_ValidData()
		{
			var cryptoApiResponse = new { data = new { BTC = new { quote = new { USD = new { price = 50000.00M } } } } };
			var exchangeApiResponse = new { rates = new Dictionary<string, decimal> { { "EUR", 0.85M }, { "BRL", 5.2M }, { "GBP", 0.75M }, { "AUD", 1.4M } } };

			SetupMockHttpResponse(JsonConvert.SerializeObject(cryptoApiResponse));
			SetupMockHttpResponse(JsonConvert.SerializeObject(exchangeApiResponse));

			var rates = await _cryptoService.GetCryptoRatesAsync("BTC");

			Assert.NotNull(rates);
			Assert.Equal(50000.00M, rates["USD"]);
			Assert.Equal(50000.00M * 0.85M, rates["EUR"]);
		}

		private void SetupMockHttpResponse(string response)
		{
			_handlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(response) });
		}
	}
}
