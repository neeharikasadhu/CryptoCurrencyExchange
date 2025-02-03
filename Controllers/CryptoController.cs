using CryptoCurrencyExchange.Interfaces;
using CryptoCurrencyExchange.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CryptoCurrencyExchange.Controllers
{
	[Route("api/crypto")]
	[ApiController]
	public class CryptoExchangeController : ControllerBase
	{
		private readonly ICryptoExchangeService _cryptoService;

		public CryptoExchangeController(ICryptoExchangeService cryptoService)
		{
			_cryptoService = cryptoService;
		}

		[HttpGet("{crypto}")]
		public async Task<IActionResult> GetCryptoRates(string crypto)
		{
			try
			{
				var rates = await _cryptoService.GetCryptoRatesAsync(crypto.ToUpper());
				return Ok(rates);
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}
	}
}
