namespace CryptoCurrencyExchange.Interfaces
{
	public interface ICryptoExchangeService
	{
		Task<Dictionary<string, decimal>> GetCryptoRatesAsync(string crypto);
	}
}
