using Bitstamp.DataPuller.Objects;
using Newtonsoft.Json.Linq;
using System;

namespace Bitstamp.DataPuller.Services
{
    public class OrderBookService : IDisposable
    {
        const string CHANNEL_NAME = "order_book";
        SubscriptionToken _subscription;
        public event Action<OrderBook> OnNewOrderBook;
        public OrderBookService()
        {
            _subscription = Communicator.Instance.Subscribe(CHANNEL_NAME);
            _subscription.Listen("data", OnNewData);
        }
        private void OnNewData(dynamic unknownObj)
        {
            JObject orderBookJson = (JObject)unknownObj;
            OrderBook newOrderBook = new OrderBookConverter().Convert(orderBookJson);
            OnNewOrderBook?.Invoke(newOrderBook);
        }
        public void Dispose()
        {
            _subscription.Unsubscribe();
        }
    }
}
