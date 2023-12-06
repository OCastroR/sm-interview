using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace SmartVault.SQLiteRepository
{
    public class SQLiteContext : IDisposable
    {
        public SQLiteConnection _dbConn;
        private string _connectionString;
        public SQLiteContext(string connectionString)
        {
            _connectionString = connectionString;
            _dbConn = new SQLiteConnection(connectionString);

            DefaultTypeMap.MatchNamesWithUnderscores = true;
        }

        public T Select<T>(string sql, object parameters = null) where T : new()
        {
            using (_dbConn)
            {
                Open();
                var o = _dbConn.Query<T>(sql, parameters).FirstOrDefault();
                if (o != null)
                    return o;

                return new T();
            }
        }

        public List<T> SelectList<T>(string sql, object parameters = null)
        {
            using (_dbConn)
            {
                Open();
                return _dbConn.Query<T>(sql, parameters).ToList();
            }
        }

        public void Dispose()
        {
            _dbConn.Close();
            _dbConn.Dispose();
        }
        public void Open()
        {
            if (_dbConn.State == ConnectionState.Closed)
                _dbConn.Open();
        }

        public void ExecuteNonQuery(string sql)
        {
            using (_dbConn)
            {
                Open();
                using (SQLiteCommand command = new SQLiteCommand(sql, _dbConn))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}