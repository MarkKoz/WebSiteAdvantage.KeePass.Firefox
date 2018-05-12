/*	WebSiteAdvantage KeePass to Firefox
 *	Copyright (C) 2008 - 2012  Anthony James McCreath
 *
 *	This library is free software; you can redistribute it and/or
 *	modify it under the terms of the GNU Lesser General Public
 *	License as published by the Free Software Foundation; either
 *	version 2.1 of the License, or (at your option) any later version.
 *
 *	This library is distributed in the hope that it will be useful,
 *	but WITHOUT ANY WARRANTY; without even the implied warranty of
 *	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 *	Lesser General Public License for more details.
 *
 *	You should have received a copy of the GNU Lesser General Public
 *	License along with this library; if not, write to the Free Software
 *	Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
 */

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

using Newtonsoft.Json;

using WebSiteAdvantage.KeePass.Firefox.Extensions;
using WebSiteAdvantage.KeePass.Firefox.Gecko;
using WebSiteAdvantage.KeePass.Firefox.Signons.Converters;

namespace WebSiteAdvantage.KeePass.Firefox.Signons
{
    /// <summary>
    /// Parses Firefox sign on files. Supports <c>logins.json</c> and <c>signons.sqlite</c>.
    /// </summary>
    internal static class SignonParser
    {
        /// <summary>
        /// Reads and parses sign ons from the <c>logins.json</c> file.
        /// </summary>
        /// <param name="path">The absolute path to the <c>logins.json</c> file.</param>
        /// <returns>The parsed sign ons.</returns>
        public static IEnumerable<Signon> ParseJson(string path)
        {
            using (var reader = new StreamReader(path))
            using (var jsonReader = new JsonTextReader(reader) {SupportMultipleContent = true})
            {
                var settings = new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    DateParseHandling = DateParseHandling.None
                };

                JsonSerializer serialiser = JsonSerializer.Create(settings);

                int depth = -1;

                // Finds the first object in logins.
                while (jsonReader.Read())
                {
                    if (jsonReader.Path.Equals("logins[0]", StringComparison.OrdinalIgnoreCase))
                    {
                        depth = jsonReader.Depth;

                        break;
                    }

                    /*if (jsonReader.Depth > 1)
                        jsonReader.Skip();*/
                }

                // Couldn't find anything valid.
                if (depth == -1) yield break;

                // Only deserialises at the depth at which the first object was found.
                // If it's lower, it means the login's EndArray token was reached.
                while (jsonReader.Depth == depth)
                {
                    yield return serialiser.Deserialize<Signon>(jsonReader);

                    jsonReader.Read(); // Skips EndObject
                }
            }
        }

        /// <summary>
        /// Reads and parses sign ons from the <c>signons.sqlite</c> database.
        /// </summary>
        /// <param name="path">The absolute path to the <c>signons.sqlite</c> file.</param>
        /// <returns>The parsed sign ons.</returns>
        public static IEnumerable<Signon> ParseDatabase(string path)
        {
            var connectionBuilder = new SQLiteConnectionStringBuilder
            {
                DataSource = path,
                Version = 3,
                FailIfMissing = true,
                ReadOnly = true
            };

            const string commandText =
                @"SELECT
                        hostname,
                        httpRealm,
                        formSubmitURL,
                        usernameField,
                        passwordField,
                        encryptedUsername,
                        encryptedPassword,
                        timeCreated,
                        timeLastUsed,
                        timePasswordChanged,
                        timesUsed
                    FROM moz_logins
                    ORDER BY hostname";

            // Doesn't handle exceptions since they were just being rethrown anyway. Default messages are adequately descriptive.
            using (var connection = new SQLiteConnection(connectionBuilder.ConnectionString).OpenAndReturn())
            using (var command = new SQLiteCommand(commandText, connection))
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    yield return new Signon
                    {
                        Hostname = reader.GetString("hostname"),
                        HttpRealm = reader.GetString("httpRealm"),
                        Username = NSS3.DecodeAndDecrypt(reader.GetString("encryptedUsername")),
                        UserNameField = reader.GetString("usernameField"),
                        Password = NSS3.DecodeAndDecrypt(reader.GetString("encryptedPassword")),
                        PasswordField = reader.GetString("passwordField"),
                        FormSubmitUrl = reader.GetString("formSubmitURL"),
                        TimeCreated = UnixTimeConverter.Convert(reader.GetNullableUInt64("timeCreated")),
                        TimeLastUsed = UnixTimeConverter.Convert(reader.GetNullableUInt64("timeLastUsed")),
                        TimePasswordChanged = UnixTimeConverter.Convert(reader.GetNullableUInt64("timePasswordChanged")),
                        TimesUsed = reader.GetUInt64("timesUsed")
                    };
                }
            }
        }
    }
}
