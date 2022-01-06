using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
