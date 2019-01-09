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

namespace WebSiteAdvantage.KeePass.Firefox.Profiles
{
    /// <summary>
    /// Information on a profile parsed from a <c>profiles.ini</c> file.
    /// </summary>
    public class ProfileInfo
    {
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

        /// <summary>
        /// The folder in which the profile's <c>profiles.ini</c> file is located.
        /// </summary>
        public string BasePath { get; set; }

        /// <summary>
        /// The full path which should be used as the ProfilePath when accessing the profile file.
        /// </summary>
        public string AbsolutePath => IsRelative ? System.IO.Path.Combine(BasePath, Path) : Path;

        /// <summary>
        /// Represents this profile instance as a string using its name.
        /// </summary>
        /// <returns>The profile's name.</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
