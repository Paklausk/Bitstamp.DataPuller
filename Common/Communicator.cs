using PusherClient;
using System;
using System.Configuration;

namespace Bitstamp.DataPuller
{
    public class Communicator
    {
        static Communicator _instance;
        public static Communicator Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Communicator();
                return _instance;
            }
        }
        private Communicator()
        {
            _pusher = new Pusher(ConfigurationManager.AppSettings["bitstampKey"]);
            _pusher.ConnectionStateChanged += ConnectionStateChanged;
            _pusher.Error += OnError;
            _pusher.Connect();
        }

        Pusher _pusher;

        public SubscriptionToken Subscribe(string channelName)
        {
            return new SubscriptionToken(_pusher.Subscribe(channelName));
        }
        public void Close()
        {
            _pusher.Disconnect();
        }

        private void OnError(object sender, PusherException error)
        {

        }
        private void ConnectionStateChanged(object sender, ConnectionState state)
        {

        }
    }
}
