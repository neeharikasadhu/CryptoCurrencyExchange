using CryptoCurrencyExchange.Interfaces;
using Newtonsoft.Json;
using static Class1;
using static CryptoCurrencyResponse;

namespace CryptoCurrencyExchange.Services
{
	public class CryptoExchangeService : ICryptoExchangeService
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly string _coinMarketCapApiKey;
		private readonly string _exchangeRatesApiKey;

		public CryptoExchangeService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
		{
			_httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
			_coinMarketCapApiKey = configuration["ApiKeys:CoinMarketCap"] ?? throw new ArgumentNullException("CoinMarketCap API key is missing");
			_exchangeRatesApiKey = configuration["ApiKeys:ExchangeRates"] ?? throw new ArgumentNullException("ExchangeRates API key is missing");
		}

		public async Task<Dictionary<string, decimal>> GetCryptoRatesAsync(string crypto)
		{
			var client = _httpClientFactory.CreateClient("CryptoClient");
			var cryptoPriceInEUR = await GetCryptoPriceInEUR(crypto, client);
			var exchangeRates = await GetExchangeRates(client);
			
			return new Dictionary<string, decimal>
			{
				{ "EUR", cryptoPriceInEUR },
				{ "USD", cryptoPriceInEUR * exchangeRates["USD"] },
				{ "BRL", cryptoPriceInEUR * exchangeRates["BRL"] },
				{ "GBP", cryptoPriceInEUR * exchangeRates["GBP"] },
				{ "AUD", cryptoPriceInEUR * exchangeRates["AUD"] }
			};
		}

		private async Task<decimal> GetCryptoPriceInEUR(string crypto, HttpClient client)
		{
			var url = $"https://pro-api.coinmarketcap.com/v1/cryptocurrency/quotes/latest?symbol={crypto}&convert=EUR";

			using var request = new HttpRequestMessage(HttpMethod.Get, url);
			request.Headers.Add("X-CMC_PRO_API_KEY", _coinMarketCapApiKey);

			var response = await client.SendAsync(request);
			response.EnsureSuccessStatusCode();

			var json = await response.Content.ReadAsStringAsync();
			var cryptoResponse = JsonConvert.DeserializeObject<CryptoResponse>(json);

			if (cryptoResponse?.Data == null || !cryptoResponse.Data.ContainsKey(crypto))
				throw new Exception("Invalid cryptocurrency code");

			return cryptoResponse.Data[crypto].Quote.EUR.Price;
		}

		private async Task<Dictionary<string, decimal>> GetExchangeRates(HttpClient client)
		{
			var url = $"https://api.exchangeratesapi.io/latest?base=EUR&symbols=USD,BRL,GBP,AUD&access_key={_exchangeRatesApiKey}";

			var response = await client.GetAsync(url);
			response.EnsureSuccessStatusCode();

			var json = await response.Content.ReadAsStringAsync();
			var ratesResponse = JsonConvert.DeserializeObject<ExchangeRatesResponse>(json);

			return ratesResponse?.Rates ?? throw new Exception("Failed to fetch exchange rates");
		}
	}
}
