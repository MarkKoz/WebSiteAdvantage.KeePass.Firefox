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
