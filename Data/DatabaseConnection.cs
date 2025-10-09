using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using MySql.Data.MySqlClient;

namespace Inmobiliaria.Data
{
    public class DatabaseConnection
    {
        private readonly string _connectionString;
        private readonly string _databaseType;

        public DatabaseConnection(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? "Data Source=inmobiliaria.db";
            
            // Detectar tipo de base de datos
            if (_connectionString.Contains("Server=") || _connectionString.Contains("Database="))
            {
                _databaseType = "MySQL";
            }
            else
            {
                _databaseType = "SQLite";
            }
        }

        public IDbConnection GetConnection()
        {
            return _databaseType switch
            {
                "MySQL" => new MySqlConnection(_connectionString),
                "SQLite" => new SQLiteConnection(_connectionString),
                _ => throw new NotSupportedException($"Tipo de base de datos no soportado: {_databaseType}")
            };
        }

        public string GetDatabaseType()
        {
            return _databaseType;
        }

        public string GetParameterPrefix()
        {
            return _databaseType switch
            {
                "MySQL" => "@",
                "SQLite" => "@",
                _ => "@"
            };
        }

        public string GetLastInsertIdQuery()
        {
            return _databaseType switch
            {
                "MySQL" => "SELECT LAST_INSERT_ID();",
                "SQLite" => "SELECT last_insert_rowid();",
                _ => "SELECT @@IDENTITY;"
            };
        }
    }
}

