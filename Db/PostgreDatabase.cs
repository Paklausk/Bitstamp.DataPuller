using System;
using System.Data;
using System.Configuration;
using Npgsql;

namespace Bitstamp.DataPuller.Db
{
    public class PostgreDatabase : Database
    {
        public static string ToDTString(DateTime datetime)
        {
            string datetimeString = datetime.ToString("yyyy.MM.dd HH:mm:ss");
            return "to_timestamp('" + datetimeString + "', 'YYYY.MM.DD HH24:MI:SS')";
        }
        public static string ToDTStatisticString(DateTime datetime)
        {
            string datetimeString = datetime.ToString("yyyy.MM.dd");
            return "to_timestamp('" + datetimeString + "', 'YYYY.MM.DD')";
        }
        public string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["Postgre"].ConnectionString; }
            set { ConfigurationManager.ConnectionStrings["Postgre"].ConnectionString = value; }
        }
        object _mutex = new object();
        NpgsqlConnection _connection;
        public bool Open()
        {
            lock (_mutex)
            {
                try
                {
                    if (_connection == null)
                        _connection = new NpgsqlConnection(ConnectionString);
                    if (_connection.State == ConnectionState.Broken)
                        Close();
                    if (_connection.State == ConnectionState.Closed)
                        _connection.Open();
                    return true;
                }
                catch (Exception e) { Log.Instance.Error("ConStr '" + ConnectionString + "' " + e.Message); }
            }
            return false;
        }
        public bool Close()
        {
            lock (_mutex)
            {
                try
                {
                    if (_connection != null && _connection.State != ConnectionState.Closed)
                    {
                        _connection.Close();
                        return true;
                    }
                }
                catch (Exception e) { Log.Instance.Error(e.Message); }
            }
            return false;
        }
        public bool IsOpen
        {
            get { return _connection != null && _connection.State == ConnectionState.Open; }
        }
        public bool Execute(string SQLCommand, bool logErrors = true)
        {
            lock (_mutex)
            {
                NpgsqlCommand cmd = new NpgsqlCommand(SQLCommand, _connection);
                try
                {
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception e) { if (logErrors) Log.Instance.Error("SQL '" + SQLCommand + "'" + e.Message); }
            }
            return false;
        }
        public long? ExecuteAndReturnID(string SQLCommand, string idColumn)
        {
            lock (_mutex)
            {
                NpgsqlCommand cmd = new NpgsqlCommand(SQLCommand + " returning " + idColumn, _connection);
                try
                {
                    object id = cmd.ExecuteScalar();
                    return (long)id;
                }
                catch (Exception e) { Log.Instance.Error("SQL '" + cmd.CommandText + "'" + e.Message); }
            }
            return null;
        }
        public DataTable Select(string SQLCommand)
        {
            lock (_mutex)
            {
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(SQLCommand, _connection);
                DataTable dt = new DataTable();
                try
                {
                    adapter.Fill(dt);
                    return dt;
                }
                catch (NpgsqlException e) { Log.Instance.Error("SQL '" + SQLCommand + "'" + e.Message); }
            }
            return null;
        }
        public void Read(string SQLCommand, Action<DataReader> read)
        {
            lock (_mutex)
            {
                try
                {
                    NpgsqlCommand cmd = new NpgsqlCommand(SQLCommand, _connection);
                    using (DataReader reader = new DataReader(cmd.ExecuteReader()))
                    {
                        read(reader);
                    }
                }
                catch (NpgsqlException e) { Log.Instance.Error("SQL '" + SQLCommand + "'" + e.Message); }
            }
        }
        public object SelectOne(string SQLCommand)
        {
            lock (_mutex)
            {
                NpgsqlCommand cmd = new NpgsqlCommand(SQLCommand, _connection);
                try
                {
                    return cmd.ExecuteScalar();
                }
                catch (Exception e) { Log.Instance.Error("SQL '" + SQLCommand + "'" + e.Message); }
            }
            return null;
        }
        public bool Execute(string SQLCommand, Action<IDbCommand> prepare, bool logErrors = true)
        {
            lock (_mutex)
            {
                NpgsqlCommand cmd = new NpgsqlCommand(SQLCommand, _connection);
                prepare(cmd);
                try
                {
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception e) { if (logErrors) Log.Instance.Error("SQL '" + SQLCommand + "'" + e.Message); }
            }
            return false;
        }
        public long? ExecuteAndReturnID(string SQLCommand, string idColumn, Action<IDbCommand> prepare)
        {
            lock (_mutex)
            {
                NpgsqlCommand cmd = new NpgsqlCommand(SQLCommand + " returning " + idColumn, _connection);
                prepare(cmd);
                try
                {
                    object id = cmd.ExecuteScalar();
                    return (long)id;
                }
                catch (Exception e) { Log.Instance.Error("SQL '" + cmd.CommandText + "'" + e.Message); }
            }
            return null;
        }
        public DataTable Select(string SQLCommand, Action<IDbCommand> prepare)
        {
            lock (_mutex)
            {
                NpgsqlCommand cmd = new NpgsqlCommand(SQLCommand, _connection);
                prepare(cmd);
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                try
                {
                    adapter.Fill(dt);
                    return dt;
                }
                catch (NpgsqlException e) { Log.Instance.Error("SQL '" + SQLCommand + "'" + e.Message); }
            }
            return null;
        }
        public void Read(string SQLCommand, Action<IDbCommand> prepare, Action<DataReader> read)
        {
            lock (_mutex)
            {
                NpgsqlCommand cmd = new NpgsqlCommand(SQLCommand, _connection);
                prepare(cmd);
                try
                {
                    using (DataReader reader = new DataReader(cmd.ExecuteReader()))
                    {
                        read(reader);
                    }
                }
                catch (NpgsqlException e) { Log.Instance.Error("SQL '" + SQLCommand + "'" + e.Message); }
            }
        }
        public object SelectOne(string SQLCommand, Action<IDbCommand> prepare)
        {
            lock (_mutex)
            {
                NpgsqlCommand cmd = new NpgsqlCommand(SQLCommand, _connection);
                prepare(cmd);
                try
                {
                    return cmd.ExecuteScalar();
                }
                catch (Exception e) { Log.Instance.Error("SQL '" + SQLCommand + "'" + e.Message); }
            }
            return null;
        }
    }
}
