/*
 * WebSiteAdvantage KeePass to Firefox
 *
 * Copyright (C) 2018 - 2019 Mark Kozlov
 * Copyright (C) 2008 - 2012 Anthony James McCreath
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

using WebSiteAdvantage.KeePass.Firefox.Extensions;

namespace WebSiteAdvantage.KeePass.Firefox.Profiles
{
    /// <summary>
    /// Parses <c>profiles.ini</c> files.
    /// </summary>
    public static class ProfileParser
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Possible paths to <c>profiles.ini</c> files.
        /// </summary>
        private static readonly string[] IniPaths =
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
        /// <param name="profiles">The profiles for which to search.</param>
        /// <returns>The primary profile or <c>null</c> if none could be found.</returns>
        public static ProfileInfo GetPrimaryProfile(IEnumerable<ProfileInfo> profiles)
        {
            try
            {
                // It's fine to enumerate again if there's no default because it's quite unlikely there won't be a default.
                // This way, for the far more common case of a default existing, it can take advantage of lazy evaluation.
                return profiles.FirstOrDefault(p => p.Default && !string.IsNullOrEmpty(p.Path)) ?? profiles.FirstOrDefault();
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error reading a profile INI."); // Catching may be redundant, but just in case...
            }

            return null;
        }

        /// <summary>
        /// Retrieves the first default profile if it exists. Otherwise,
        /// retrieves the first profile. Invalid profiles are excluded.
        /// </summary>
        /// <remarks>
        /// Profiles are retrieved using <see cref="GetProfilePaths"/> and <see cref="GetProfiles"/>.
        /// </remarks>
        /// <returns>The primary profile or <c>null</c> if none could be found.</returns>
        public static ProfileInfo GetPrimaryProfile()
        {
            IEnumerable<ProfileInfo> profiles =  GetProfiles(GetProfilePaths()).SkipExceptions();

            return GetPrimaryProfile(profiles);
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
                foreach (string path in IniPaths)
                    yield return Path.Combine(basePath, path);
            }
        }

        /// <summary>
        /// Parses <c>profiles.ini</c> files.
        /// </summary>
        /// <param name="paths">The paths of the files to parse.</param>
        /// <returns>The parsed profiles.</returns>
        public static IEnumerable<ProfileInfo> GetProfiles(IEnumerable<string> paths)
        {
            foreach (string path in paths)
            {
                if (!File.Exists(path))
                {
                    Logger.Debug("File does not exist at " + path);
                    continue;
                }

                Logger.Info("File exists at " + path);

                using (StreamReader reader = File.OpenText(path))
                {
                    ProfileInfo profile = null;
                    string line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        ProfileInfo previous = profile;

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
        private static bool ParseLine(string line, string path, ref ProfileInfo profile)
        {
            if (line.StartsWith("[profile", StringComparison.OrdinalIgnoreCase))
            {
                profile = new ProfileInfo
                {
                    Code = line.Trim().TrimStart('[').TrimEnd(']'),
                    BasePath = Path.GetDirectoryName(path)
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
    }
}
