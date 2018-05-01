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
using System.Linq;

using WebSiteAdvantage.KeePass.Firefox.Gecko;

namespace WebSiteAdvantage.KeePass.Firefox
{
    /// <summary>
    /// Represents a Firefox user profile.
    /// </summary>
    public class FirefoxProfile : IDisposable
    {
        #region Constructors

        /// <summary>
        /// Constructs a profile from the given <see cref="FirefoxProfileInfo"/>.
        /// </summary>
        /// <param name="profileInfo">The <see cref="FirefoxProfileInfo"/> from which to construct a profile.</param>
        /// <param name="password">The profile's master password.</param>
        /// <exception cref="ArgumentException">Thrown when the profile cannot be initialised or the password is invalid.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="profileInfo"/> is <c>null</c>.</exception>
        /// <exception cref="NsprException">Thrown when NSS cannot be initialised or a key slot cannot be retrieved.</exception>
        public FirefoxProfile(FirefoxProfileInfo profileInfo, string password) : this(profileInfo?.AbsolutePath, password)
        {
            Info = profileInfo;
        }

        /// <summary>
        /// Constructs a profile for the profile at the given path.
        /// </summary>
        /// <param name="profilePath">The absolute path to the profile.</param>
        /// <param name="password">The profile's master password.</param>
        /// <exception cref="ArgumentException">Thrown when the profile cannot be initialised or the password is invalid.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="profilePath"/> is <c>null</c>.</exception>
        /// <exception cref="NsprException">Thrown when NSS cannot be initialised or a key slot cannot be retrieved.</exception>
        public FirefoxProfile(string profilePath, string password)
        {
            Path = profilePath ?? throw new ArgumentNullException(nameof(profilePath), "Could not find a profile.");

            Init();
            Login(password);
        }

        /// <summary>
        /// Constructs a profile for the default profile or the first profile found.
        /// </summary>
        /// <param name="password">The profile's master password.</param>
        /// <exception cref="ArgumentException">Thrown when the password is invalid.</exception>
        /// <exception cref="ArgumentNullException">Thrown when no profile can be found.</exception>
        /// <exception cref="NsprException">Thrown when NSS cannot be initialised or a key slot cannot be retrieved.</exception>
        public FirefoxProfile(string password) : this(FirefoxProfileParser.GetPrimaryProfile(), password) { }

        #endregion

        /// <summary>
        /// Initialises NSS.
        /// </summary>
        /// <exception cref="NsprException">Thrown when the profile cannot be initialised.</exception>
        private void Init()
        {
            if (NSS3.NSS_Init(Path) != SECStatus.Success)
                throw new NsprException($"Failed to initialise profile at {Path}.");
        }

        /// <summary>
        /// Initialise the profile and validates its password.
        /// </summary>
        /// <param name="password">The profile's master password.</param>
        /// <exception cref="ArgumentException">Thrown when the password cannot be validated.</exception>
        /// <exception cref="NsprException">Thrown when a key slot cannot be retrieved.</exception>
        private void Login(string password)
        {
            _Slot = NSS3.PK11_GetInternalKeySlot(); // Gets a slot to work with.

            if (_Slot == IntPtr.Zero)
                throw new NsprException("Failed to get internal key slot.");

            SECStatus result = NSS3.PK11_CheckUserPassword(_Slot, password);

            if (result != SECStatus.Success)
            {
                int error = NSPR4.PR_GetError();
                string errorName = NSPR4.PR_ErrorToName(error);

                throw new ArgumentException($"Failed to validate the profile's password: ({error}) {errorName}.", nameof(password));
            }
        }

        #region Profile Data

        private IntPtr _Slot;
        private FirefoxSignonsFile _SignonsFile;
        private FirefoxProfileInfo _Info;

        /// <summary>
        /// The profile's information.
        /// </summary>
        public FirefoxProfileInfo Info
        {
            // Parses the profiles.ini. Needed when the path constructor is used.
            get => _Info ?? (_Info = FirefoxProfileParser
                       .GetProfiles(new[] { System.IO.Path.Combine(Path, "profiles.ini") }).FirstOrDefault());

            private set => _Info = value;
        }

        /// <summary>
        /// Retrieves the profile's signon file.
        /// </summary>
        /// <returns>The profile's signon file.</returns>
        public FirefoxSignonsFile SignonsFile => _SignonsFile ?? (_SignonsFile = new FirefoxSignonsFile(Path));

        /// <summary>
        /// The profile's absolute path.
        /// </summary>
        public string Path { get; }

        #endregion

        public void Dispose()
        {
            NSS3.PK11_FreeSlot(_Slot);

            if (NSS3.NSS_Shutdown() != SECStatus.Success)
                throw new NsprException("Failed to shut down.");
        }
    }
}
