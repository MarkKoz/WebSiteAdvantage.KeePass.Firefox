/* WebSiteAdvantage KeePass to Firefox
 * Copyright (C) 2008 - 2012  Anthony James McCreath
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

using WebSiteAdvantage.KeePass.Firefox.Signons.Converters;

namespace WebSiteAdvantage.KeePass.Firefox.Signons
{
    /// <summary>
    /// Saved sign on information from Firefox's signon file.
    /// </summary>
    public class Signon
    {
        /// <summary>
        /// The URL of the website the sign on is for.
        /// </summary>
        [JsonProperty("hostname")]
        public string Hostname { get; set; }

        /// <summary>
        /// The sign on's HTTP realm. Used by HTTP-authenticated sign ons.
        /// </summary>
        [JsonProperty("httpRealm")]
        public string HttpRealm { get; set; }

        /// <summary>
        /// The login form's domain.
        /// </summary>
        [JsonProperty("formSubmitURL")]
        public string FormSubmitUrl { get; set; }

        /// <summary>
        /// The name of HTML user name field or blank for HTTP authentication.
        /// </summary>
        [JsonProperty("usernameField")]
        public string UserNameField { get; set; }

        /// <summary>
        /// The name of HTML password field or blank for HTTP authentication.
        /// </summary>
        [JsonProperty("passwordField")]
        public string PasswordField { get; set; }

        /// <summary>
        /// The decrypted username.
        /// </summary>
        [JsonProperty("encryptedUsername")]
        [JsonConverter(typeof(EncryptedValueConverter))]
        public string Username { get; set; }

        /// <summary>
        /// The decrypted password.
        /// </summary>
        [JsonProperty("encryptedPassword")]
        [JsonConverter(typeof(EncryptedValueConverter))]
        public string Password { get; set; }

        /// <summary>
        /// When the sign on was created.
        /// </summary>
        [JsonProperty("timeCreated")]
        [JsonConverter(typeof(UnixTimeConverter))]
        public DateTime? TimeCreated { get; set; }

        /// <summary>
        /// When the sign on was last used.
        /// </summary>
        [JsonProperty("timeLastUsed")]
        [JsonConverter(typeof(UnixTimeConverter))]
        public DateTime? TimeLastUsed { get; set; }

        /// <summary>
        /// When the sign on was last modified.
        /// </summary>
        [JsonProperty("timePasswordChanged")]
        [JsonConverter(typeof(UnixTimeConverter))]
        public DateTime? TimePasswordChanged { get; set; }

        /// <summary>
        /// Amount of times sign on was used.
        /// </summary>
        [JsonProperty("timesUsed")]
        public ulong TimesUsed { get; set; }
    }
}
