using FullTrade;

public interface IBinanceDataService
{
    Task<List<CandleData>> GetSpotCandleData(string symbol, string interval, int limit = 1000);
}