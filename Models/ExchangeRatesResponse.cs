using Newtonsoft.Json;
using System;

public class Class1
{
	public class ExchangeRatesResponse
	{
		[JsonProperty("rates")]
		public required Dictionary<string, decimal> Rates { get; set; }
	}
}
