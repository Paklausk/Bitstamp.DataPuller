using Newtonsoft.Json.Linq;
using System;

namespace Bitstamp.DataPuller.Objects
{
    public class LiveTrade
    {
        public JObject Json { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
    }
}
