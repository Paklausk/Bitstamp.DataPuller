using Bitstamp.DataPuller.Db;
using Bitstamp.DataPuller.Objects;
using System;

namespace Bitstamp.DataPuller.Services
{
    public class DbSaverService
    {
        Database _db;
        public DbSaverService(Database db)
        {
            _db = db;
        }
        public void Save(OrderBook book)
        {
            DateTime utcNow = DateTime.UtcNow;
            long timelineId = CreateAndGetTimelineId(utcNow);
            foreach (var ask in book.Asks)
            {
                _db.Execute("INSERT INTO asks (timeline_id, price, amount) VALUES (@timeline_id, @price, @amount)", (cmd) =>
                {
                    cmd
                    .Set("timeline_id", timelineId)
                    .Set("price", ask.Price)
                    .Set("amount", ask.Amount);
                });
            }
            foreach (var bid in book.Bids)
            {
                _db.Execute("INSERT INTO bids (timeline_id, price, amount) VALUES (@timeline_id, @price, @amount)", (cmd) =>
                {
                    cmd
                    .Set("timeline_id", timelineId)
                    .Set("price", bid.Price)
                    .Set("amount", bid.Amount);
                });
            }
        }
        public void Save(LiveTrade trade)
        {
            long timelineId = CreateAndGetTimelineId(trade.Timestamp);
            _db.Execute("INSERT INTO trades (timeline_id, price, amount) VALUES (@timeline_id, @price, @amount)", (cmd) =>
            {
                cmd
                .Set("timeline_id", timelineId)
                .Set("price", trade.Price)
                .Set("amount", trade.Amount);
            });
        }
        long CreateAndGetTimelineId(DateTime timeUtc)
        {
            _db.Execute("INSERT INTO timeline (time_utc) VALUES (@time)", (cmd) =>
            {
                cmd.Set("time", timeUtc);
            }, false);
            return (long)_db.SelectOne("SELECT id FROM timeline WHERE time_utc = @time", (cmd) =>
            {
                cmd.Set("time", timeUtc);
            });
        }
    }
}
