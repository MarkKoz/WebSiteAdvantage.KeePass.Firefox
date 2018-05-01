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
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NLog;

namespace WebSiteAdvantage.KeePass.Firefox
{
    /// <summary>
    /// data gathered from the profileIni file
    /// </summary>
    public class FirefoxProfileInfo
    {
        private static readonly Logger _Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// String from the header part of a profile definition. In []
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// The profile's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Denotes if the profile's path is relative.
        /// </summary>
        public bool IsRelative { get; set; } = true; // Not sure what the default should be.

        /// <summary>
        /// The path to the profile.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Denotes if the profile is the default profile.
        /// </summary>
        public bool Default { get; set; } = false;

        public string BasePath { get; set; }

        /// <summary>
        /// The full path which should be used as the ProfilePath when accessing the profile file.
        /// </summary>
        public string AbsolutePath => IsRelative ? BasePath + "\\" + Path.Replace("/", "\\") : Path.Replace("/", "\\");

        #region Finding Profiles

        /// <summary>
        /// Retrieves the first default profile if it exists. Otherwise,
        /// retrieves the first profile. Invalid profiles are excluded.
        /// </summary>
        /// <returns>The primary profile or <c>null</c> if none could be found.</returns>
        public static FirefoxProfileInfo FindPrimaryProfile()
        {
            List<FirefoxProfileInfo> profiles = FindFirefoxProfileInfos();

            return profiles.FirstOrDefault(p => p.Default && !string.IsNullOrEmpty(p.Path)) ??
                   profiles.FirstOrDefault(p => !string.IsNullOrEmpty(p.Path));
        }

        /// <summary>
        /// try and find the current users default profile
        /// searches for firefox application data in:
        /// LocalApplicationData
        /// ApplicationData
        /// CommonApplicationData
        /// </summary>
        /// <returns>full path to a profile</returns>
        public static List<FirefoxProfileInfo> FindFirefoxProfileInfos()
        {
            List<FirefoxProfileInfo> profiles = new List<FirefoxProfileInfo>();

            FindFirefoxProfileInfos(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), profiles);
            FindFirefoxProfileInfos(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), profiles);
            FindFirefoxProfileInfos(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), profiles);

            return profiles;
        }
        public static void FindFirefoxProfileInfos(string applicationsPath, List<FirefoxProfileInfo> profiles)
        {
            FindFirefoxProfileInfos(applicationsPath, "\\Mozilla\\Firefox", profiles);
            FindFirefoxProfileInfos(applicationsPath, "\\Thunderbird", profiles);
            FindFirefoxProfileInfos(applicationsPath, "\\Mozilla\\SeaMonkey", profiles);
            FindFirefoxProfileInfos(applicationsPath, "\\Mozilla", profiles);
        }
        /// <summary>
        /// look for a default firefox profile in the supplied application data path
        /// </summary>
        /// <param name="applicationPath">a path to application data</param>
        /// <returns>full path to a profile</returns>
        public static void FindFirefoxProfileInfos(string applicationsPath, string applicationPath, List<FirefoxProfileInfo> profiles)
        {
            string basePath = applicationsPath + applicationPath; // firefoxes relative path
            string profilesIni = basePath + "\\profiles.ini"; // file that contains data on the default profile

            FindFirefoxProfileInfosFromIniFile(profilesIni, profiles);
        }
        public static void FindFirefoxProfileInfosFromIniFile(string profilesIni, List<FirefoxProfileInfo> profiles)
        {

            // searcvh the profile for a profile entry that contains "Default=1"
            if (File.Exists(profilesIni))
            {
                _Logger.Info("File exists at " + profilesIni);

                StreamReader reader = File.OpenText(profilesIni);

                try
                {

                    FirefoxProfileInfo profile = null;

                    string line = reader.ReadLine();
                    while (line != null)
                    {
                        string lowerLine = line.ToLower().Replace(" ", "");
                        if (lowerLine.StartsWith("[")) // new section
                        {
                            if (lowerLine.StartsWith("[profile")) // new section
                            {
                                profile = new FirefoxProfileInfo();

                                profile.Code = line.Trim().TrimStart('[').TrimEnd(']');

                                profile.BasePath = profilesIni.Substring(0,profilesIni.LastIndexOf("\\"));

                                profiles.Add(profile);
                            }
                            else
                                profile = null;
                        }

                        if (profile != null)
                        {
                            if (lowerLine.StartsWith("name=")) // this is the default profile
                                profile.Name = line.Substring(5);

                            if (lowerLine.StartsWith("path=")) // this is the default profile
                                profile.Path = line.Substring(5);

                            if (lowerLine == "default=1") // this is the default profile
                                profile.Default = true;

                            if (lowerLine == "isrelative=1") // this is the default profile
                                profile.IsRelative = true;

                            if (lowerLine == "isrelative=0") // this is the default profile
                                profile.IsRelative = false;
                        }

                        line = reader.ReadLine();
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            else
                _Logger.Debug("File does not exist at " + profilesIni);
        }
        #endregion

        public override string ToString()
        {
            return this.Name;
        }

    }
}
