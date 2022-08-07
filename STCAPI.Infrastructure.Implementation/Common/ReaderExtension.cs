using MySqlConnector;

namespace STCAPI.Infrastructure.Implementation.Common
{
    public static class ReaderExtension
    {
        public static T DefaultIfNull<T>(this MySqlDataReader reader, string columnName)
        {
            var colIndex = reader.GetOrdinal(columnName);
            if (!reader.IsDBNull(colIndex))
            {
                return (T)reader[reader.GetName(colIndex)];
            }

            return default;
        }
    }
}
