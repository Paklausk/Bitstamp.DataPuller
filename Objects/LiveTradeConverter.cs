using System;
using Newtonsoft.Json.Linq;

namespace Bitstamp.DataPuller.Objects
{
    public class LiveTradeConverter
    {
        public LiveTrade Convert(JObject liveTradeJson)
        {
            LiveTrade liveTrade = new LiveTrade();
            liveTrade.Json = liveTradeJson;
            liveTrade.Timestamp = UnixTimeStampToDateTime(liveTradeJson.Value<long>("timestamp"));
            liveTrade.Amount = ValueConverter.ToDecimal(liveTradeJson.GetValue("amount"));
            liveTrade.Price = ValueConverter.ToDecimal(liveTradeJson.GetValue("price"));
            return liveTrade;
        }
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}