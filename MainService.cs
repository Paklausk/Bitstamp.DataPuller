using Bitstamp.DataPuller.Db;
using Bitstamp.DataPuller.Objects;
using Bitstamp.DataPuller.Services;
using System;
using System.ServiceProcess;
using System.Threading;

namespace Bitstamp.DataPuller
{
    [System.ComponentModel.DesignerCategory("")]
    public class MainService : ServiceBase
    {
        Database _db;
        DatabaseConnectionOpener _dbOpener;
        OrderBookService _orderBook;
        LiveTradesService _liveTrades;
        DbSaverService _saver;
        Thread _startThread;

        public MainService()
        {
            this.ServiceName = Settings.Instance.ServiceName;
        }

        protected override void OnStart(string[] args)
        {
            _startThread = new Thread(EntryPoint);
            _startThread.Start();
        }
        protected override void OnStop()
        {
            _dbOpener?.Dispose();
            _startThread?.Join();
            LeavePoint();
        }

        public void EntryPoint()
        {
            _db = new DatabaseFactory().CreateDatabase();
            _dbOpener = new DatabaseConnectionOpener(_db);
            if (_dbOpener.OpenWait())
            {
                _saver = new DbSaverService(_db);
                _orderBook = new OrderBookService();
                _orderBook.OnNewOrderBook += OnNewOrderBook;
                _liveTrades = new LiveTradesService();
                _liveTrades.OnNewLiveTrade += OnNewLiveTrade;
                Log.Instance.Info("Service started");
            }
        }
        public void LeavePoint()
        {
            if (_orderBook != null)
                _orderBook.OnNewOrderBook -= OnNewOrderBook;
            _orderBook?.Dispose();
            if (_liveTrades != null)
                _liveTrades.OnNewLiveTrade -= OnNewLiveTrade;
            _liveTrades?.Dispose();
            Communicator.Instance.Close();
            _db?.Close();
            Log.Instance.Info("Service stopped");
        }

        private void OnNewOrderBook(OrderBook book)
        {
            _saver.Save(book);
        }
        private void OnNewLiveTrade(LiveTrade trade)
        {
            _saver.Save(trade);
        }
    }
}