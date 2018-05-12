using System.Data.SQLite;

namespace WebSiteAdvantage.KeePass.Firefox.Extensions
{
    /// <summary>
    /// Contains extension methods for <see cref="SQLiteDataReader"/>.
    /// </summary>
    public static class SQLiteDataReaderExtensions
    {
        public static string GetString(this SQLiteDataReader reader, string column)
        {
            int oridnal = reader.GetOrdinal(column);

            return reader.IsDBNull(oridnal) ? string.Empty : reader.GetString(oridnal);
        }

        public static ulong GetUInt64(this SQLiteDataReader reader, string column)
        {
            int oridnal = reader.GetOrdinal(column);

            return reader.IsDBNull(oridnal) ? default(ulong) : (ulong)reader.GetInt64(oridnal);
        }

        public static ulong? GetNullableUInt64(this SQLiteDataReader reader, string column)
        {
            int oridnal = reader.GetOrdinal(column);

            return reader.IsDBNull(oridnal) ? default(ulong?) : (ulong?)reader.GetInt64(oridnal);
        }
    }
}
