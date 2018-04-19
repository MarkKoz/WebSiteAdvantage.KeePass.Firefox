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
    public static class NSPR4
    {
        public static Int32 PR_GetError()
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
                    throw new Exception("Not Supported");
            }
        }

        public static string PR_ErrorToName(Int32 code)
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
                    throw new Exception("Not Supported");
            }
        }
    }
}
