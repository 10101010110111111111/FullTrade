using FullTrade;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;

public class BinanceDataService
{
    private readonly HttpClient _httpClient;

    public BinanceDataService()
    {
        _httpClient = new HttpClient();
    }

    public async Task<List<CandleData>> GetSpotCandleData(string symbol, string interval, int limit = 1000)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("Symbol nemůže být prázdný", nameof(symbol));

            if (string.IsNullOrWhiteSpace(interval))
                throw new ArgumentException("Interval nemůže být prázdný", nameof(interval));

            string url = $"https://api.binance.com/api/v3/klines?symbol={symbol.ToUpper()}&interval={interval}&limit={limit}";
            Console.WriteLine($"Requesting URL: {url}");

            var response = await _httpClient.GetAsync(url);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = JsonConvert.DeserializeObject<dynamic>(responseBody);
                throw new Exception($"Binance API chyba: {errorResponse?.msg ?? responseBody}");
            }

            if (string.IsNullOrWhiteSpace(responseBody))
                throw new Exception("API vrátilo prázdnou odpověď");

            var jsonData = JArray.Parse(responseBody);
            return ParseCandleData(jsonData);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Chyba při komunikaci s Binance API: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Neočekávaná chyba při získávání dat: {ex.Message}");
            throw;
        }
    }

    private List<CandleData> ParseCandleData(JArray data)
    {
        var candles = new List<CandleData>();

        foreach (var candle in data)
        {
            try
            {
                var candleData = new CandleData
                {
                    Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(candle[0].ToString())).LocalDateTime,
                    Open = Convert.ToDouble(candle[1].ToString(), CultureInfo.InvariantCulture),
                    High = Convert.ToDouble(candle[2].ToString(), CultureInfo.InvariantCulture),
                    Low = Convert.ToDouble(candle[3].ToString(), CultureInfo.InvariantCulture),
                    Close = Convert.ToDouble(candle[4].ToString(), CultureInfo.InvariantCulture),
                    Volume = Convert.ToDouble(candle[5].ToString(), CultureInfo.InvariantCulture)
                };

                candles.Add(candleData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Chyba při zpracování svíčky: {ex.Message}");
            }
        }

        return candles;
    }
}