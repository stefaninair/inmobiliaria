using System.Data;

namespace Inmobiliaria.Data
{
    public static class DataReaderExtensions
    {
        public static int GetInt32(this IDataReader reader, string columnName)
        {
            return reader.GetInt32(reader.GetOrdinal(columnName));
        }

        public static string GetString(this IDataReader reader, string columnName)
        {
            return reader.GetString(reader.GetOrdinal(columnName));
        }

        public static decimal GetDecimal(this IDataReader reader, string columnName)
        {
            return reader.GetDecimal(reader.GetOrdinal(columnName));
        }

        public static DateTime GetDateTime(this IDataReader reader, string columnName)
        {
            return reader.GetDateTime(reader.GetOrdinal(columnName));
        }

        public static bool GetBoolean(this IDataReader reader, string columnName)
        {
            return reader.GetBoolean(reader.GetOrdinal(columnName));
        }

        public static bool IsDBNull(this IDataReader reader, string columnName)
        {
            return reader.IsDBNull(reader.GetOrdinal(columnName));
        }

        public static double GetDouble(this IDataReader reader, string columnName)
        {
            return reader.GetDouble(reader.GetOrdinal(columnName));
        }

        public static float GetFloat(this IDataReader reader, string columnName)
        {
            return reader.GetFloat(reader.GetOrdinal(columnName));
        }

        public static long GetInt64(this IDataReader reader, string columnName)
        {
            return reader.GetInt64(reader.GetOrdinal(columnName));
        }

        public static short GetInt16(this IDataReader reader, string columnName)
        {
            return reader.GetInt16(reader.GetOrdinal(columnName));
        }

        public static byte GetByte(this IDataReader reader, string columnName)
        {
            return reader.GetByte(reader.GetOrdinal(columnName));
        }

        public static Guid GetGuid(this IDataReader reader, string columnName)
        {
            return reader.GetGuid(reader.GetOrdinal(columnName));
        }
    }
}

