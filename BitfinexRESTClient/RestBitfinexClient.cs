using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using BitfinexCore;
using RestSharp;

namespace BitfinexRESTClient;

public class RestBitfinexClient:IRestBitfinexClient
{
    private const string ApiUrl = "https://api-pub.bitfinex.com/v2/";
    public Task<IEnumerable<Trade>> GetNewTradesAsync(string pair, int maxCount)
    {
        string finalUrl = $"{ApiUrl}trades/{pair}/hist?limit={maxCount}";
        var option = new RestClientOptions(finalUrl);
        var client = new RestClient(option);
        var request = new RestRequest("");
        request.AddHeader("accept", "application/json");
        var response = client.GetAsync(request).Result;
        var jsonResult = JsonObject.Parse(response.Content);
        List<Trade> trades = new List<Trade>();
        foreach (var tradeItem in jsonResult.AsArray())
        {
            trades.Add(new Trade
                {
                    Pair = pair,
                    Amount = Math.Abs(tradeItem[2].GetValue<decimal>()),
                    Id = tradeItem[0].ToString(),
                    Price = tradeItem[3].GetValue<decimal>(),
                    Side = Convert.ToDecimal(tradeItem[2].GetValue<decimal>())>0?"buy":"sell",
                    Time = new DateTime(1970,1,1).AddMilliseconds(Convert.ToInt64(tradeItem[1].GetValue<Int64>()))
                }
            );
        }
        return Task.FromResult(trades.AsEnumerable());
    }

    public Task<IEnumerable<Candle>> GetCandleSeriesAsync(string pair, int periodInSec, DateTimeOffset? from, DateTimeOffset? to = null, long? count=0)
    {
        throw new NotImplementedException();
    }
}