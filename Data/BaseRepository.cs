using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;
using System.Data.SQLite;

namespace Inmobiliaria.Data
{
    public abstract class BaseRepository
    {
        protected readonly DatabaseConnection _dbConnection;

        public BaseRepository(DatabaseConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        protected int ExecuteNonQuery(string query, Dictionary<string, object>? parameters = null)
        {
            using var connection = _dbConnection.GetConnection();
            connection.Open();
            
            using var command = CreateCommand(query, connection, parameters);
            return command.ExecuteNonQuery();
        }

        protected object? ExecuteScalar(string query, Dictionary<string, object>? parameters = null)
        {
            using var connection = _dbConnection.GetConnection();
            connection.Open();
            
            using var command = CreateCommand(query, connection, parameters);
            return command.ExecuteScalar();
        }

        protected IDataReader ExecuteReader(string query, Dictionary<string, object>? parameters = null)
        {
            var connection = _dbConnection.GetConnection();
            connection.Open();
            
            var command = CreateCommand(query, connection, parameters);
            return command.ExecuteReader();
        }

        protected IDbCommand CreateCommand(string query, IDbConnection connection, Dictionary<string, object>? parameters = null)
        {
            IDbCommand command = _dbConnection.GetDatabaseType() switch
            {
                "MySQL" => new MySqlCommand(query, (MySqlConnection)connection),
                "SQLite" => new SQLiteCommand(query, (SQLiteConnection)connection),
                _ => throw new InvalidOperationException("Unsupported database type.")
            };

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    var dbParameter = command.CreateParameter();
                    dbParameter.ParameterName = param.Key;
                    dbParameter.Value = param.Value ?? DBNull.Value;
                    command.Parameters.Add(dbParameter);
                }
            }

            return command;
        }

        protected int GetLastInsertId(IDbConnection connection)
        {
            var query = _dbConnection.GetLastInsertIdQuery();
            using var command = CreateCommand(query, connection);
            var result = command.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        protected string GetParameterName(string parameterName)
        {
            return _dbConnection.GetParameterPrefix() + parameterName;
        }
    }
}