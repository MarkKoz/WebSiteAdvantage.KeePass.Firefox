/*
 * WebSiteAdvantage KeePass to Firefox
 *
 * Copyright (C) 2018 - 2019 Mark Kozlov
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
 */

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
