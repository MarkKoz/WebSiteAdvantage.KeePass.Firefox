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
using System.Diagnostics;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using WebSiteAdvantage.KeePass.Firefox.Gecko;

namespace WebSiteAdvantage.KeePass.Firefox
{
    /// <summary>
    /// represents the content of a firefox signon file
    /// </summary>
    public class FirefoxSignonsFile
    {
        #region Construction and Loading
        /// <summary>
        /// Quick way to load a new file
        /// </summary>
        /// <param name="profile">Profile with the signon file</param>
        public static FirefoxSignonsFile Create(FirefoxProfile profile, string password)
        {
            FirefoxSignonsFile file = new FirefoxSignonsFile();
            file.Load(profile, password);

            return file;
        }

        /// <summary>
        /// Loads this object with signon data from a <paramref name="profile"/>.
        /// </summary>
        /// <param name="profile">The profile from which to load signons.</param>
        /// <param name="password"></param>
        public void Load(FirefoxProfile profile, string password)
        {
            Profile = profile;

            if (Profile.ProfilePath == null)
                throw new FileNotFoundException("Failed to determine the location of the default Firefox Profile.");

            SECStatus result1;

            try
            {
                result1 = NSS3.NSS_Init(Profile.ProfilePath); // init for profile
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to Init Profile (" + Profile.ProfilePath + "): " + ex.Message, ex);
            }

            if (result1 != SECStatus.Success)
            {
                Int32 error = NSPR4.PR_GetError();
                string errorName = NSPR4.PR_ErrorToName(error);
                throw new Exception("Failed to initialise profile for load at " + Profile.ProfilePath + " reason " + errorName);
            }

            try
            {
                IntPtr slot = NSS3.PK11_GetInternalKeySlot(); // get a slot to work with

                if (slot == IntPtr.Zero)
                    throw new Exception("Failed to GetInternalKeySlot");

                try
                {
                    SECStatus result2 = NSS3.PK11_CheckUserPassword(slot, password);

                    if (result2 != SECStatus.Success)
                    {
                        Int32 error = NSPR4.PR_GetError();
                        string errorName = NSPR4.PR_ErrorToName(error);
                        throw new Exception("Failed to Validate Password: " + errorName);
                    }

                    if (!LoadJson())
                        if (!LoadDatabase())
                            LoadLegacy();
                }
                finally
                {
                    NSS3.PK11_FreeSlot(slot);
                }
            }
            finally
            {
                NSS3.NSS_Shutdown();
            }
        }

        /// <summary>
        /// Loads signons from the <c>logins.json</c> file.
        /// </summary>
        /// <returns><c>true</c> if the file was found and loaded; <c>false</c> if the file doesn't exist.</returns>
        private bool LoadJson()
        {
            string loginsJsonPath = Path.Combine(Profile.ProfilePath, "logins.json");

            if (!File.Exists(loginsJsonPath))
                return false;

            JObject responseJson = (JObject)JsonConvert.DeserializeObject(File.ReadAllText(loginsJsonPath));

            foreach (JObject login in responseJson["logins"])
            {
                FirefoxSignonSite signonSite = null;

                string hostname = login["hostname"].ToString();

                foreach (FirefoxSignonSite site in this.SignonSites)
                {
                    if (site.Site == hostname)
                    {
                        signonSite = site;
                        break;
                    }
                }

                if (signonSite == null)
                {
                    signonSite = new FirefoxSignonSite();
                    signonSite.Site = hostname;
                    this.SignonSites.Add(signonSite);
                }

                FirefoxSignon signon = new FirefoxSignon();
                signonSite.Signons.Add(signon);

                signon.UserName = NSS3.DecodeAndDecrypt(login["encryptedUsername"].ToString());
                signon.UserNameField = login["usernameField"].ToString();
                signon.Password = NSS3.DecodeAndDecrypt(login["encryptedPassword"].ToString());
                signon.PasswordField = login["passwordField"].ToString();
                signon.LoginFormDomain = login["formSubmitURL"].ToString();
                signon.TimeCreated = ConvertUnixTime((ulong)login["timeCreated"]);
                signon.TimeLastUsed = ConvertUnixTime((ulong)login["timeLastUsed"]);
                signon.TimePasswordChanged = ConvertUnixTime((ulong)login["timePasswordChanged"]);
                signon.TimesUsed = (ulong)login["timesUsed"];

                Debug.WriteLine(login["id"].ToString());
                Debug.WriteLine(login["httpRealm"].ToString()); // null?
                Debug.WriteLine(login["guid"].ToString());
                Debug.WriteLine(login["encType"].ToString());
            }

            return true;
        }

        /// <summary>
        /// Loads signons from the <c>signons.sqlite</c> database.
        /// </summary>
        /// <returns><c>true</c> if the database was found and loaded; <c>false</c> if the database doesn't exist.</returns>
        private bool LoadDatabase()
        {
            string dbPath = Path.Combine(Profile.ProfilePath, "signons.sqlite");

            // FailIfMissing throws SQLiteException if not found.
            // However, that exception is too general so file existsence is checked this way.
            if (!File.Exists(dbPath))
                return false;

            var connectionBuilder = new SQLiteConnectionStringBuilder
            {
                DataSource = dbPath,
                Version = 3,
                FailIfMissing = true,
                ReadOnly = true
            };

            const string commandText =
                @"SELECT
                        hostname,
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
                FirefoxSignonSite signonSite = null;

                while (reader.Read())
                {
                    string hostname = reader.GetString("hostname");

                    // This works because the query results are sorted by hostname.
                    if (signonSite == null || signonSite.Site != hostname)
                    {
                        signonSite = new FirefoxSignonSite { Site = hostname };
                        SignonSites.Add(signonSite);
                    }

                    var signon = new FirefoxSignon();
                    signonSite.Signons.Add(signon);

                    signon.UserName = NSS3.DecodeAndDecrypt(reader.GetString("encryptedUsername"));
                    Debug.WriteLine("U=" + signon.UserName);

                    signon.UserNameField = reader.GetString("usernameField");

                    signon.Password = NSS3.DecodeAndDecrypt(reader.GetString("encryptedPassword"));
                    Debug.WriteLine("P=" + signon.Password);

                    signon.PasswordField = reader.GetString("passwordField");
                    signon.LoginFormDomain = reader.GetString("formSubmitURL");
                    signon.TimeCreated = ConvertUnixTime(reader.GetNullableUInt64("timeCreated"));
                    signon.TimeLastUsed = ConvertUnixTime(reader.GetNullableUInt64("timeLastUsed"));
                    signon.TimePasswordChanged = ConvertUnixTime(reader.GetNullableUInt64("timePasswordChanged"));
                    signon.TimesUsed = reader.GetUInt64("timesUsed");
                }
            }

            return true;
        }

        /// <summary>
        /// Loads signons from the <c>signons.txt</c> file used by legacy versions of Firefox.
        /// </summary>
        private void LoadLegacy()
        {
            int version = 3;
            string header = null;
            string signonFile = null;

            while (signonFile == null && version > 0)
            {
                string signonPath = Path.Combine(Profile.ProfilePath, SignonFileNames[version]);
                if (File.Exists(signonPath))
                {
                    signonFile = signonPath;
                    header = SignonHeaderValues[version];
                }
                else
                    version--;
            }

            Version = version;

            if (version == 0)
                throw new Exception("Could not find a signon file to process in " + Profile.ProfilePath);

            StreamReader reader = File.OpenText(signonFile);

            try
            {
                // first line is header
                string line = reader.ReadLine();

                if (line == null)
                    throw new Exception("signon file is empty");

                if (line != header)
                    throw new Exception("signon file contains an invalid header");

                // lines till the first dot are host excludes

                while ((line = reader.ReadLine()) != null && line != ".")
                {
                    Debug.WriteLine("# " + line);

                    Debug.WriteLine("ExcludeHost: " + line);

                    this.ExcludeHosts.Add(line);
                }
                FirefoxSignonSite signonSite = null;
                // read each entry

                while (line != null)
                {
                    Debug.WriteLine("## " + line);

                    // here after any dot (.) therefore new site
                    // all new lines pass through if they contain a dot (.)

                    signonSite = null;


                    while ((line = reader.ReadLine()) != null && line != ".")
                    {
                        Debug.WriteLine("# " + line);
                        // first line is host
                        // subsequent lines are pairs of name value
                        // if name starts with * then its a password
                        // values are encrypted

                        if (signonSite == null) // site is reset to null after each dot (.)
                        {
                            signonSite = new FirefoxSignonSite();
                            signonSite.Site = line;

                            this.SignonSites.Add(signonSite);

                            Debug.WriteLine("Site: " + line);

                            line = reader.ReadLine(); // move to the next line
                            Debug.WriteLine("# " + line);
                        }
                        // else stick to the same line for the next parser, second site entries dont have a dot (.) nor site line



                        if (line != null && line != ".")
                        {
                            FirefoxSignon signon = new FirefoxSignon();
                            signonSite.Signons.Add(signon);
                            // User field
                            signon.UserNameField = line;
                            Debug.WriteLine("UserNameField: " + signon.UserNameField);

                            if ((line = reader.ReadLine()) != null && line != ".")
                            {
                                Debug.WriteLine("# " + line);
                                // User Value
                                string u = NSS3.DecodeAndDecrypt(line);
                                signon.UserName = u;
                                Debug.WriteLine("UserName: " + signon.UserName);

                                if ((line = reader.ReadLine()) != null && line != ".")
                                {
                                    Debug.WriteLine("# " + line);
                                    // Password field
                                    signon.PasswordField = line.TrimStart(new char[] { '*' });
                                    Debug.WriteLine("PasswordField: " + signon.PasswordField);

                                    if ((line = reader.ReadLine()) != null && line != ".")
                                    {
                                        Debug.WriteLine("# " + line);
                                        // Password Value
                                        string p = NSS3.DecodeAndDecrypt(line);
                                        signon.Password = p;
                                        Debug.WriteLine("Password: " + signon.Password);

                                        if ((line = reader.ReadLine()) != null && line != ".")
                                        {
                                            Debug.WriteLine("# " + line);
                                            // Domain
                                            signon.LoginFormDomain = line;
                                            Debug.WriteLine("LoginFormDomain: " + signon.LoginFormDomain);

                                            if ((line = reader.ReadLine()) != null && line != ".")
                                            {
                                                Debug.WriteLine("# " + line);
                                                // Filler
                                                Debug.WriteLine("Filler: " + line);
                                            }
                                            // note: if there is not a dot (.) after this then its a subsequent entry for the same site
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                reader.Close();
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

        #endregion

        #region Related Profile

        /// <summary>
        /// The profile associated with the signon file.
        /// </summary>
        public FirefoxProfile Profile { get; set; }

        #endregion

        #region File Data

        /// <summary>
        /// The version of the legacy signon file.
        /// </summary>
        public int Version { get; set; } = 0;

        /// <summary>
        /// Collection of singons that were stored in the file, grouped by website.
        /// </summary>
        public List<FirefoxSignonSite> SignonSites { get; } = new List<FirefoxSignonSite>();

        /// <summary>
        /// Hosts that have been excluded.
        /// </summary>
        public List<string> ExcludeHosts { get; } = new List<string>();

        #endregion

        #region Information on Versions

        /// <summary>
        /// The file names, indexed by Firefox version, of the legacy signon file.
        /// </summary>
        public static string[] SignonFileNames { get; } = { null, "signons.txt", "signons2.txt", "signons3.txt" };

        /// <summary>
        /// The headers, indexed by Firefox version, of the legacy signon file.
        /// </summary>
        public static string[] SignonHeaderValues { get; } = { null, "#2c", "#2d", "#2e" };

        #endregion
    }
}
