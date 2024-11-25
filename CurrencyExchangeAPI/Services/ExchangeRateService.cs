using System;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace CurrencyExchangeAPI.Services
{
    public class ExchangeRateService
{
    private readonly HttpClient _httpClient;
    private const string ApiUrl = "https://api.fxratesapi.com/latest";
    private Dictionary<string, decimal> _cachedRates;
    private DateTime _lastFetched;

    public ExchangeRateService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _lastFetched = DateTime.MinValue;
    }

    public async Task<Dictionary<string, decimal>> GetExchangeRatesAsync()
{
    const int maxRetries = 3;
    for (int retry = 0; retry < maxRetries; retry++)
    {
        try
        {
            var response = await _httpClient.GetAsync(ApiUrl);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var exchangeRateResponse = JsonConvert.DeserializeObject<ExchangeRateResponse>(content);

                _cachedRates = exchangeRateResponse?.Rates ?? new Dictionary<string, decimal>();
                _lastFetched = DateTime.UtcNow;

                return _cachedRates;
            }
            else if ((int)response.StatusCode == 429) // Rate limited
            {
                var retryAfter = response.Headers.RetryAfter?.Delta?.TotalSeconds ?? 10;
                await Task.Delay(TimeSpan.FromSeconds(retryAfter));
            }
        }
        catch (Exception ex)
        {
            if (retry == maxRetries - 1)
            {
                _cachedRates ??= new Dictionary<string, decimal> { { "USD", 0 }, { "EUR", 0 }, { "JPY", 0 } }; 
                throw new Exception("Rate limit exceeded and retries failed.", ex);
            }
            await Task.Delay(2000 * (int)Math.Pow(2, retry)); 
        }
    }
    return _cachedRates ?? new Dictionary<string, decimal>();
}
}


    public class ExchangeRateResponse
    {
        public string Base { get; set; }
        public Dictionary<string, decimal> Rates { get; set; }
    }
}
