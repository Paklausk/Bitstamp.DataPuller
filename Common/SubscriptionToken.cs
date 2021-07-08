using PusherClient;
using System;

namespace Bitstamp.DataPuller
{
    public class SubscriptionToken
    {
        Channel _channel;
        bool _subscribed = false;
        public SubscriptionToken(Channel channel)
        {
            _channel = channel;
            _subscribed = _channel != null;
        }
        public void Listen(string eventName, Action<dynamic> callback)
        {
            if (_subscribed)
                _channel.Bind(eventName, callback);
        }
        public void Unsubscribe()
        {
            _subscribed = false;
            _channel.UnbindAll();
            _channel.Unsubscribe();
        }
    }
}
