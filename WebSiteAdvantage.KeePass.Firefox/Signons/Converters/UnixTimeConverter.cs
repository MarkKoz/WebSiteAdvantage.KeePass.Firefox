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

using System;
using Newtonsoft.Json;

namespace WebSiteAdvantage.KeePass.Firefox.Signons.Converters
{
    /// <summary>
    /// Converts unix time, in milliseconds, to a <see cref="DateTime"/> in UTC.
    /// </summary>
    internal class UnixTimeConverter : JsonConverter
    {
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {

            return reader.TokenType == JsonToken.Null ? default(DateTime?) : Convert(System.Convert.ToUInt64(reader.Value));
        }

        /// <inheritdoc />
        public override bool CanWrite => false;

        /// <inheritdoc />
        /// <remarks>
        /// Using attribute to specify converter; CanConvert is unused.
        /// </remarks>
        public override bool CanConvert(Type objectType) => false;

        /// <summary>
        /// Converts a unix time, in milliseconds, to a <see cref="Nullable{DateTime}"/> in UTC.
        /// </summary>
        /// <param name="unixTimeMs">The unix timestamp, in milliseconds, to convert.</param>
        /// <returns>The converted time.</returns>
        public static DateTime? Convert(ulong? unixTimeMs)
        {
            return unixTimeMs.HasValue
                ? new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(unixTimeMs.Value)
                : default(DateTime?);
        }
    }
}
