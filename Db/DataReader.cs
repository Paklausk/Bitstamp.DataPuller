using System;
using System.Data;
using System.Collections.Generic;

namespace Bitstamp.DataPuller.Db
{
    public class DataReader : IDisposable
    {
        struct ColumnInfo
        {
            private int _index;
            public ColumnInfo(int index)
            {
                _index = index;
            }
            public int Index
            {
                get { return _index; }
            }
        }
        IDataReader _reader;
        SortedDictionary<string, ColumnInfo> _columnMap = new SortedDictionary<string, ColumnInfo>();
        public DataReader(IDataReader reader)
        {
            _reader = reader;
            LoadColumnMap(reader); 
        }
        protected void LoadColumnMap(IDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; i++)
                _columnMap[reader.GetName(i)] = new ColumnInfo(i);
        }
        protected bool GetOrdinal(string columnName, out int ordinal)
        {
            ordinal = -1;
			ColumnInfo columnInfo;
			if (_columnMap.TryGetValue(columnName, out columnInfo)) {
				ordinal = columnInfo.Index;
				return true;
			}
			return false;
        }
        protected int GetOrdinal(string columnName)
        {
            return _columnMap[columnName].Index;
        }
        protected bool Exists(string columnName)
        {
			int ordinal;
            return GetOrdinal(columnName, out ordinal);
        }
        protected bool Exists(int columnIndex)
        {
            return columnIndex < _reader.FieldCount;
        }
        public List<string> GetColumnNames()
        {
            List<string> columnNames = new List<string>();
            for (int i = 0; i < _reader.FieldCount; i++)
                columnNames.Add(_reader.GetName(i));
            return columnNames;
        }
        public int GetColumnsCount()
        {
            return _reader.FieldCount;
        }
        public string Get(string columnName)
        {
            return Get(columnName, null);
        }
        public string Get(string columnName, string defaultValue)
        {
            try
            {
                if (Exists(columnName))
                {
                    int ordinal = _reader.GetOrdinal(columnName);
                    if (!_reader.IsDBNull(ordinal))
                        return _reader.GetString(ordinal);
                }
            }
            catch { }
            return defaultValue;
        }
        public T? Get<T>(string columnName) where T : struct
        {
            return Get<T>(columnName, null);
        }
        public T? Get<T>(string columnName, T? defaultValue) where T : struct
        {
            try
            {
                if (Exists(columnName))
                {
                    int ordinal = _reader.GetOrdinal(columnName);
                    if (!_reader.IsDBNull(ordinal))
                        return _reader.GetValue(ordinal) as T?;
                }
            }
            catch { }
            return defaultValue;
        }
        public object Get(int columnIndex)
        {
            return Get(columnIndex, null);
        }
        public object Get(int columnIndex, object defaultValue)
        {
            try
            {
                if (Exists(columnIndex))
                {
                    if (!_reader.IsDBNull(columnIndex))
                        return _reader.GetValue(columnIndex);
                }
            }
            catch { }
            return defaultValue;
        }
        public T? Get<T>(int columnIndex) where T : struct
        {
            return Get<T>(columnIndex, null);
        }
        public T? Get<T>(int columnIndex, T? defaultValue) where T : struct
        {
            try
            {
                if (Exists(columnIndex))
                {
                    if (!_reader.IsDBNull(columnIndex))
                        return _reader.GetValue(columnIndex) as T?;
                }
            }
            catch { }
            return defaultValue;
        }
        public bool Read()
        {
            return _reader.Read();
        }
        public void Dispose()
        {
            _reader.Close();
            _reader.Dispose();
        }
    }
}
