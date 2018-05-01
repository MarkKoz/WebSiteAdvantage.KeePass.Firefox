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

namespace WebSiteAdvantage.KeePass.Firefox
{
    /// <summary>
    /// Some common methods to help doing keepass stuff
    /// </summary>
    public static class KeePassUtilities
    {
        /// <summary>
        /// Path to the log file.
        /// </summary>
        public static string LogPath { get; set; } = @"WebSiteAdvantage.KeePass.Firefox.log";

        /// <summary>
        /// This library's version.
        /// </summary>
        public static string Version { get; } = "2.28"; // [assembly: AssemblyFileVersion("2.21.0.0")]

        /// <summary>
        /// Maximum length of the string to assign to an <c>autotypewindow</c> parameter.
        /// </summary>
        /// <remarks>
        /// Defaults value is arbitrary. Titles change and it's better to get multiple options by default than none.
        /// </remarks>
        public static int AutoTypeTextSize { get; set; } = 15;

        /// <summary>
        /// Returns the text to use in an autotypewindow parameter
        /// </summary>
        /// <param name="title">title that would be on the matching window</param>
        /// <returns></returns>
        public static string AutoTypeWindow(string title)
        {
            string s = title;

            if (s.Length > AutoTypeTextSize)
                s = s.Substring(0, AutoTypeTextSize);

            return s + "*"; // starts with
        }

        /// <summary>
        /// Provides the standard value for the autotype sequence
        /// </summary>
        /// <returns></returns>
        public static string AutoTypeSequence()
        {
            return "{USERNAME}{TAB}{PASSWORD}{ENTER}";

            // suggested solution to some lost char issues...
            // return "{DELAY 50}1{DELAY 50}{BACKSPACE}{USERNAME}{TAB}{DELAY 50}1{DELAY 50}{BACKSPACE}{PASSWORD}{ENTER}";
        }
    }
}
