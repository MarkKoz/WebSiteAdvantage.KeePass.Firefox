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

namespace WebSiteAdvantage.KeePass.Firefox.Gecko
{
    /// <summary>
    /// A Netscape Portable Runtime API wrapper class.
    /// </summary>
    public static class NSPR4
    {
        /// <summary>
        /// Returns the last set error code.
        /// </summary>
        /// <returns>The last set error code.</returns>
        public static int PR_GetError()
        {
            switch (Gecko.Version)
            {
                //case "NSS310":
                //    return NSS310.NSPR4.PR_GetError();
                case "NSS312":
                    return NSS312.NSPR4.PR_GetError();
                case "NSS64":
                    return NSS64.NSPR4.PR_GetError();
                default:
                    throw new InvalidOperationException("Unsupported Gecko version.");
            }
        }

        /// <summary>
        /// Returns the macro name for an error code, or NULL if the error code is not known.
        /// </summary>
        /// <param name="code">The error code for which to return a name.</param>
        /// <returns>The error code's name.</returns>
        public static string PR_ErrorToName(int code)
        {
            switch (Gecko.Version)
            {
                //case "NSS310":
                //    return NSS310.NSPR4.PR_ErrorToName(code);
                case "NSS312":
                    return NSS312.NSPR4.PR_ErrorToName(code);
                case "NSS64":
                    return NSS64.NSPR4.PR_ErrorToName(code);
                default:
                    throw new InvalidOperationException("Unsupported Gecko version.");
            }
        }
    }

    /// <summary>
    /// A Netscape Portable Runtime exception.
    /// </summary>
    public class NsprException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NsprException"/> class.
        /// </summary>
        public NsprException()
        {
            ErrorName = NSPR4.PR_ErrorToName(ErrorCode);
            // TODO: Use PR_GetErrorText for the message?
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NsprException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public NsprException(string message) : base(message)
        {
            ErrorName = NSPR4.PR_ErrorToName(ErrorCode);
        }

        /*/// <summary>
        /// Initializes a new instance of the <see cref="NsprException"/> class with a specified error message
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception.</param>
        public NsprException(string message, Exception inner) : base(message, inner)
        {
            ErrorName = NSPR4.PR_ErrorToName(ErrorCode);
        }*/

        // The code of the error which caused this exception.
        public int ErrorCode { get; } = NSPR4.PR_GetError();

        // The name of the error which caused this exception.
        public string ErrorName { get; }
    }
}
