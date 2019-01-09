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

using WebSiteAdvantage.KeePass.Firefox.Nss;

namespace WebSiteAdvantage.KeePass.Firefox.Signons.Converters
{
    /// <summary>
    /// Decrypts a value using NSS.
    /// </summary>
    internal class EncryptedValueConverter : JsonConverter
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
            return NssUtils.DecodeAndDecrypt((string)reader.Value);
        }

        /// <inheritdoc />
        public override bool CanWrite => false;

        /// <inheritdoc />
        /// <remarks>
        /// Using attribute to specify converter; CanConvert is unused.
        /// </remarks>
        public override bool CanConvert(Type objectType) => false;
    }
}
