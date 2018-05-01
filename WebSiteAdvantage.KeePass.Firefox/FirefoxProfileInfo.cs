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
    /// Information on a profile parsed from a <c>profiles.ini</c> file.
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
        public bool IsRelative { get; set; } = true;

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
        public string AbsolutePath => IsRelative ? System.IO.Path.Combine(BasePath, Path) : Path;

        #region Finding Profiles

        /// <summary>
        /// Possible paths to <c>profiles.ini</c> files.
        /// </summary>
        private static readonly string[] _IniPaths =
        {
            @"Mozilla\Firefox\profiles.ini",
            @"Thunderbird\profiles.ini",
            @"Mozilla\SeaMonkey\profiles.ini",
            @"Mozilla\profiles.ini"
        };

        /// <summary>
        /// Retrieves the first default profile if it exists. Otherwise,
        /// retrieves the first profile. Invalid profiles are excluded.
        /// </summary>
        /// <returns>The primary profile or <c>null</c> if none could be found.</returns>
        public static FirefoxProfileInfo GetPrimaryProfile()
        {
            try
            {
                IEnumerable<FirefoxProfileInfo> profiles = GetProfiles(GetProfilePaths()).SkipExceptions();

                // It's fine to enumerate again if there's no default because it's quite unlikely there won't be a default.
                // This way, for the far more common case of a default existing, it can take advantage of lazy evaluation.
                return profiles.FirstOrDefault(p => p.Default && !string.IsNullOrEmpty(p.Path)) ?? profiles.FirstOrDefault();
            }
            catch (Exception e)
            {
                _Logger.Error(e, "Error reading a profile INI."); // Catching may be redundant, but just in case...
            }

            return null;
        }

        /// <summary>
        /// Gets possible paths for <c>profiles.ini</c> files. This includes the application data folders.
        /// </summary>
        /// <returns>Possible paths for <c>profiles.ini</c> files.</returns>
        public static IEnumerable<string> GetProfilePaths()
        {
            string[] basePaths =
            {
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
            };

            foreach (string basePath in basePaths)
            {
                foreach (string path in _IniPaths)
                    yield return System.IO.Path.Combine(basePath, path);
            }
        }

        /// <summary>
        /// Parses <c>profiles.ini</c> files.
        /// </summary>
        /// <param name="paths">The paths of the files to parse.</param>
        /// <returns>The parsed profiles.</returns>
        public static IEnumerable<FirefoxProfileInfo> GetProfiles(IEnumerable<string> paths)
        {
            foreach (string path in paths)
            {
                if (!File.Exists(path))
                {
                    _Logger.Debug("File does not exist at " + path);
                    continue;
                }

                _Logger.Info("File exists at " + path);

                using (StreamReader reader = File.OpenText(path))
                {
                    FirefoxProfileInfo profile = null;
                    string line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        FirefoxProfileInfo previous = profile;

                        if (ParseLine(line, path, ref profile) && previous != null)
                            yield return previous;
                    }

                    // Finished reading the file. Yields the final profile if it exists.
                    if (profile != null)
                        yield return profile;
                }
            }
        }

        /// <summary>
        /// Parses a line of a <c>profiles.ini</c> file.
        /// </summary>
        /// <param name="line">The line to parse.</param>
        /// <param name="path">The path to the <c>profiles.ini</c> file.</param>
        /// <param name="profile">The profile currently being parsed.</param>
        /// <returns><c>true</c> if on a new profile; <c>false</c> otherwise.</returns>
        private static bool ParseLine(string line, string path, ref FirefoxProfileInfo profile)
        {
            if (line.StartsWith("[profile", StringComparison.OrdinalIgnoreCase))
            {
                profile = new FirefoxProfileInfo
                {
                    Code = line.Trim().TrimStart('[').TrimEnd(']'),
                    BasePath = System.IO.Path.GetDirectoryName(path)
                };

                return true;
            }

            if (profile != null)
            {
                if (line.StartsWith("name=", StringComparison.OrdinalIgnoreCase))
                    profile.Name = line.Substring(5);

                // TODO: Handle paths with spaces? Are they within quotes?
                if (line.StartsWith("path=", StringComparison.OrdinalIgnoreCase))
                    profile.Path = line.Substring(5);

                if (line.Equals("default=1", StringComparison.OrdinalIgnoreCase))
                    profile.Default = true;

                if (line.Equals("isrelative=0", StringComparison.OrdinalIgnoreCase))
                    profile.IsRelative = false;
            }

            return false;
        }

        #endregion

        public override string ToString()
        {
            return Name;
        }
    }
}
