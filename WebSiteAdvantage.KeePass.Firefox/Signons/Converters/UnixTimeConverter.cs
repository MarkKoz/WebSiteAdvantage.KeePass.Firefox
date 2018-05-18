using System;
using Newtonsoft.Json;

namespace WebSiteAdvantage.KeePass.Firefox.Signons.Converters
{
    /// <summary>
    /// Converts unix time, in miliseconds, to a <see cref="DateTime"/> in UTC.
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
        /// Converts a unix time, in miliseconds, to a <see cref="Nullable{DateTime}"/> in UTC.
        /// </summary>
        /// <param name="unixTimeMs">The unix timestamp, in miliseconds, to convert.</param>
        /// <returns>The converted time.</returns>
        public static DateTime? Convert(ulong? unixTimeMs)
        {
            return unixTimeMs.HasValue
                ? new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(unixTimeMs.Value)
                : default(DateTime?);
        }
    }
}
