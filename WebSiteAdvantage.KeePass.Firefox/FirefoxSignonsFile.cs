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
using Newtonsoft.Json.Linq;

using NLog;

using WebSiteAdvantage.KeePass.Firefox.Gecko;

namespace WebSiteAdvantage.KeePass.Firefox
{
    /// <summary>
    /// Parses Firefox sign on files. Supports <c>logins.json</c> and <c>signons.sqlite</c>.
    /// </summary>
    internal static class FirefoxSignonsFile
    {
        /// <summary>
        /// Reads and parses sign ons from the <c>logins.json</c> file.
        /// </summary>
        /// <param name="path">The absolute path to the <c>logins.json</c> file.</param>
        /// <returns>The parsed sign ons.</returns>
        public static IEnumerable<FirefoxSignon> ParseJson(string path)
        {
            JObject responseJson = (JObject)JsonConvert.DeserializeObject(File.ReadAllText(path));

            foreach (JObject login in responseJson["logins"])
            {
                yield return new FirefoxSignon
                {
                    Hostname = login["hostname"].ToString(),
                    HttpRealm = login["httpRealm"].ToString(),
                    UserName = NSS3.DecodeAndDecrypt(login["encryptedUsername"].ToString()),
                    UserNameField = login["usernameField"].ToString(),
                    Password = NSS3.DecodeAndDecrypt(login["encryptedPassword"].ToString()),
                    PasswordField = login["passwordField"].ToString(),
                    FormSubmitUrl = login["formSubmitURL"].ToString(),
                    TimeCreated = ConvertUnixTime((ulong)login["timeCreated"]),
                    TimeLastUsed = ConvertUnixTime((ulong)login["timeLastUsed"]),
                    TimePasswordChanged = ConvertUnixTime((ulong)login["timePasswordChanged"]),
                    TimesUsed = (ulong)login["timesUsed"]
                };
            }
        }

        /// <summary>
        /// Reads and parses sign ons from the <c>signons.sqlite</c> database.
        /// </summary>
        /// <param name="path">The absolute path to the <c>signons.sqlite</c> file.</param>
        /// <returns>The parsed sign ons.</returns>
        public static IEnumerable<FirefoxSignon> ParseDatabase(string path)
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
                    yield return new FirefoxSignon
                    {
                        Hostname = reader.GetString("hostname"),
                        HttpRealm = reader.GetString("httpRealm"),
                        UserName = NSS3.DecodeAndDecrypt(reader.GetString("encryptedUsername")),
                        UserNameField = reader.GetString("usernameField"),
                        Password = NSS3.DecodeAndDecrypt(reader.GetString("encryptedPassword")),
                        PasswordField = reader.GetString("passwordField"),
                        FormSubmitUrl = reader.GetString("formSubmitURL"),
                        TimeCreated = ConvertUnixTime(reader.GetNullableUInt64("timeCreated")),
                        TimeLastUsed = ConvertUnixTime(reader.GetNullableUInt64("timeLastUsed")),
                        TimePasswordChanged = ConvertUnixTime(reader.GetNullableUInt64("timePasswordChanged")),
                        TimesUsed = reader.GetUInt64("timesUsed")
                    };
                }
            }
        }

        /// <summary>
        /// Converts a unix time, in miliseconds, to a <see cref="Nullable{DateTime}"/> in UTC.
        /// </summary>
        /// <param name="unixTimeMs">The unix timestamp, in miliseconds, to convert.</param>
        /// <returns>The converted time.</returns>
        private static DateTime? ConvertUnixTime(ulong? unixTimeMs)
        {
            return unixTimeMs.HasValue
                ? new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(unixTimeMs.Value)
                : default(DateTime?);
        }
    }
}
