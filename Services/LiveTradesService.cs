using Bitstamp.DataPuller.Objects;
using Newtonsoft.Json.Linq;
using System;

namespace Bitstamp.DataPuller.Services
{
    public class LiveTradesService : IDisposable
    {
        const string CHANNEL_NAME = "live_trades";
        SubscriptionToken _subscription;
        public event Action<LiveTrade> OnNewLiveTrade;
        public LiveTradesService()
        {
            _subscription = Communicator.Instance.Subscribe(CHANNEL_NAME);
            _subscription.Listen("trade", OnNewData);
        }
        private void OnNewData(dynamic unknownObj)
        {
            JObject liveTradeJson = (JObject)unknownObj;
            LiveTrade newLiveTrade = new LiveTradeConverter().Convert(liveTradeJson);
            OnNewLiveTrade?.Invoke(newLiveTrade);
        }
        public void Dispose()
        {
            _subscription.Unsubscribe();
        }
    }
}
