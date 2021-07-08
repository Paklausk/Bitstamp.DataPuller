using Npgsql;
using NpgsqlTypes;
using System;
using System.Data;

namespace Bitstamp.DataPuller.Db
{
    public interface Database
    {
        string ConnectionString { get; set; }
        bool Open();
        bool Close();
        bool IsOpen { get; }
        DataTable Select(string SQLCommand);
        void Read(string SQLCommand, Action<DataReader> read);
        object SelectOne(string SQLCommand);
        bool Execute(string SQLCommand, bool logErrors = true);
        long? ExecuteAndReturnID(string SQLCommand, string idColumn);
        bool Execute(string SQLCommand, Action<IDbCommand> prepare, bool logErrors = true);
        long? ExecuteAndReturnID(string SQLCommand, string idColumn, Action<IDbCommand> prepare);
        DataTable Select(string SQLCommand, Action<IDbCommand> prepare);
        object SelectOne(string SQLCommand, Action<IDbCommand> prepare);
        void Read(string SQLCommand, Action<IDbCommand> prepare, Action<DataReader> read);
    }
    public static class IDbCommandExtensions {
        public static IDbCommand Set(this IDbCommand command, string name, object value) {
            var parameter = command.CreateParameter();
            parameter.Value = value != null ? value : DBNull.Value;
            parameter.ParameterName = name;
            command.Parameters.Add(parameter);
            return command;
        }
        public static IDbCommand Set(this IDbCommand command, string name, object value, NpgsqlDbType type)
        {
            NpgsqlParameter parameter = (command as NpgsqlCommand).CreateParameter();
            parameter.Value = value != null ? value : DBNull.Value;
            parameter.NpgsqlDbType = type;
            parameter.ParameterName = name;
            command.Parameters.Add(parameter);
            return command;
        }
    }
}
