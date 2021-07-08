using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bitstamp.DataPuller.Db
{
    public class DatabaseFactory
    {
        public static object DatabaseWriteLock = new object();
        protected static Type _databaseType = null;

        static DatabaseFactory()
        {
            SetDatabaseType("postgre");
        }
        public static void SetDatabaseType(string databaseName)
        {
            switch (databaseName)
            {
                case "postgre":
                    _databaseType = typeof(PostgreDatabase);
                    break;
                default:
                    _databaseType = null;
                    break;
            }
        }
        public static bool IsDatabaseType(string databaseName)
        {
            switch (databaseName)
            {
                case "postgre":
                    if (_databaseType == typeof(PostgreDatabase))
                        return true;
                    break;
                default:
                    return false;
            }
            return false;
        }
        public virtual Database CreateDatabase()
        {
            return (Database)_databaseType.GetConstructor(new Type[] { }).Invoke(new object[] { });
        }
    }
}
