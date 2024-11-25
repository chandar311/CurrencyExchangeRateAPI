using Microsoft.AspNetCore.Mvc;
using CurrencyExchangeAPI.Services;

namespace CurrencyExchangeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExchangeRateController : ControllerBase
    {
        private readonly ExchangeRateService _rateService;

        public ExchangeRateController(ExchangeRateService rateService)
        {
            _rateService = rateService;
        }

        [HttpGet]
        public async Task<IActionResult> GetExchangeRates()
        {
            var rates = await _rateService.GetExchangeRatesAsync();
            return Ok(rates);
        }
    }
}
