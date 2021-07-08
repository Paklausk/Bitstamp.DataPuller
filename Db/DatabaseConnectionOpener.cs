using System;

namespace Bitstamp.DataPuller.Db
{
    public class DatabaseConnectionOpener
    {
        Database _db;
        bool _run;
        public DatabaseConnectionOpener(Database db)
        {
            _db = db;
            _run = true;
        }
        public bool OpenWait()
        {
            while (_run)
            {
                if (_db.Open() && _db.IsOpen)
                    return true;
                else _db.Close();
            }
            return false;
        }
        public void Dispose()
        {
            _run = false;
        }
    }
}
