/*
 * WebSiteAdvantage KeePass to Firefox
 *
 * Copyright (C) 2018 - 2019 Mark Kozlov
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

using System.Runtime.InteropServices;

namespace WebSiteAdvantage.KeePass.Firefox.Nss.Native
{
    internal static class NsprNativeMethods
    {
        static NsprNativeMethods()
        {
            NativeUtils.LoadLibrary("nspr4.dll");
        }

        /// <summary>
        /// Returns the current thread's last set error code.
        /// </summary>
        /// <returns>The current thread's last set error code.</returns>
        [DllImport("nspr4.dll")] //, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PR_GetError();

        /// <summary>
        /// Gets the macro name for an error code.
        /// </summary>
        /// <remarks>
        /// Does not work for error table 0, the system error codes.
        /// </remarks>
        /// <param name="code">The error code for which to get a name.</param>
        /// <returns>The macro name of the error, or <c>NULL</c> if the error code is unknown.</returns>
        [DllImport("nspr4.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern string PR_ErrorToName(int code);
    }
}
