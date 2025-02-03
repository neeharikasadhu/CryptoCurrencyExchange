using Newtonsoft.Json;
using System;

public class CryptoCurrencyResponse
{
	public class CryptoResponse
	{
		[JsonProperty("data")]
		public required Dictionary<string, CryptoData> Data { get; set; }
	}

	public class CryptoData
	{
		[JsonProperty("quote")]
		public required CryptoQuote Quote { get; set; }
	}

	public class CryptoQuote
	{
		[JsonProperty("EUR")]
		public required CurrencyInfo EUR { get; set; }
	}

	public class CurrencyInfo
	{
		[JsonProperty("price")]
		public decimal Price { get; set; }
	}
}
