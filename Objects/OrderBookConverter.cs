using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Bitstamp.DataPuller.Objects
{
    public class OrderBookConverter
    {
        public OrderBook Convert(JObject orderBookJson)
        {
            OrderBook orderBook = new OrderBook();
            orderBook.Json = orderBookJson;
            orderBook.Bids = ConvertArray(orderBookJson.Value<JArray>("bids"));
            orderBook.Asks = ConvertArray(orderBookJson.Value<JArray>("asks"));
            return orderBook;
        }
        List<OrderBookEntry> ConvertArray(JArray array)
        {
            List<OrderBookEntry> list = new List<OrderBookEntry>();
            foreach (JToken token in array)
            {
                JArray entryJson = token.Value<JArray>();
                OrderBookEntry entry = new OrderBookEntry();
                entry.Price = ValueConverter.ToDecimal(entryJson[0]);
                entry.Amount = ValueConverter.ToDecimal(entryJson[1]);
                list.Add(entry);
            }
            return list;
        }
    }
}
