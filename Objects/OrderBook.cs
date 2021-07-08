using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Bitstamp.DataPuller.Objects
{
    public class OrderBook
    {
        public JObject Json { get; set; }
        public List<OrderBookEntry> Bids = new List<OrderBookEntry>();
        public List<OrderBookEntry> Asks = new List<OrderBookEntry>();
    }
}
