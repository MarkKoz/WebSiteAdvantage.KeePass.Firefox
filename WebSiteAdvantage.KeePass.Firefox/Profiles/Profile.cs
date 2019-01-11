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

using NLog;

using WebSiteAdvantage.KeePass.Firefox.Nss;
using WebSiteAdvantage.KeePass.Firefox.Signons;

using static WebSiteAdvantage.KeePass.Firefox.Nss.Native.NsprNativeMethods;
using static WebSiteAdvantage.KeePass.Firefox.Nss.Native.NssNativeMethods;

namespace WebSiteAdvantage.KeePass.Firefox.Profiles
{
    /// <summary>
    /// Represents a Firefox user profile.
    /// </summary>
    public class Profile : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #region Constructors

        /// <summary>
        /// Constructs a profile from the given <see cref="ProfileInfo"/>.
        /// </summary>
        /// <param name="profileInfo">The <see cref="ProfileInfo"/> from which to construct a profile.</param>
        /// <param name="password">The profile's master password.</param>
        /// <exception cref="ArgumentException">Thrown when the profile cannot be initialised or the password is invalid.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="profileInfo"/> is <c>null</c>.</exception>
        /// <exception cref="NsprException">Thrown when NSS cannot be initialised or a key slot cannot be retrieved.</exception>
        internal Profile(ProfileInfo profileInfo, string password = "") : this(profileInfo?.AbsolutePath, password)
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
        public Profile(string profilePath, string password = "")
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
        public Profile(string password = "") : this(ProfileParser.GetPrimaryProfile(), password) { }

        #endregion

        /// <summary>
        /// Initialises NSS.
        /// </summary>
        /// <exception cref="NsprException">Thrown when the profile cannot be initialised.</exception>
        private void Init()
        {
            if (NSS_Init(Path) != SecStatus.Success)
                throw new NsprException($"Failed to initialise profile at {Path}.");
        }

        /// <summary>
        /// Initialise the profile and validates its password.
        /// </summary>
        /// <param name="password">The profile's master password.</param>
        /// <exception cref="ArgumentException">Thrown when the password cannot be validated.</exception>
        /// <exception cref="NsprException">Thrown when a key slot cannot be retrieved or authenticating it fails.</exception>
        private void Login(string password)
        {
            slot = PK11_GetInternalKeySlot(); // Gets a slot to work with.

            if (slot == IntPtr.Zero)
                throw new NsprException("Failed to get internal key slot.");

            if (PK11_CheckUserPassword(slot, password ?? "") != SecStatus.Success)
            {
                int error = PR_GetError();
                string errorName = PR_ErrorToName(error);

                throw new ArgumentException($"Failed to validate the profile's password: ({error}) {errorName}.", nameof(password));
            }

            if (PK11_Authenticate(slot, false, IntPtr.Zero) != SecStatus.Success)
                throw new NsprException("Failed to authenticate the slot.");
        }

        /// <summary>
        /// Reads and parses the profile's sign ons.
        /// </summary>
        /// <exception cref="FileNotFoundException">Thrown when no sign on file can be found for the profile.</exception>
        /// <returns>The parsed sign ons.</returns>
        public IEnumerable<Signon> GetSignons()
        {
            string pathJson = System.IO.Path.Combine(Path, "logins.json");
            string pathDb = System.IO.Path.Combine(Path, "signons.sqlite");

            if (File.Exists(pathJson))
                return SignonParser.ParseJson(pathJson);

            Logger.Info("logins.json could not be found. Falling back to signons.sqlite.");

            if (File.Exists(pathDb))
                return SignonParser.ParseDatabase(pathDb);

            throw new FileNotFoundException("No sign on file could be found.", "signons.sqlite");
        }

        #region Profile Data

        private IntPtr slot;

        /// <summary>
        /// The profile's information. <c>null</c> if <see cref="Profile(string,string)"/> is used.
        /// </summary>
        public ProfileInfo Info { get; }

        /// <summary>
        /// The profile's absolute path.
        /// </summary>
        public string Path { get; }

        #endregion

        public void Dispose()
        {
            PK11_FreeSlot(slot);

            if (NSS_Shutdown() != SecStatus.Success)
                throw new NsprException("Failed to shut down.");
        }
    }
}
