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

using System.Collections.Generic;

namespace WebSiteAdvantage.KeePass.Firefox
{
    /// <summary>
    /// Contains signons which are for the same site/hostname.
    /// </summary>
    public class FirefoxSignonSite
    {
        /// <summary>
        /// The site's hostname.
        /// </summary>
        public string Site { get; set; }

        /// <summary>
        /// Collection of signons for the site.
        /// </summary>
        public List<FirefoxSignon> Signons { get; } = new List<FirefoxSignon>();
    }
}
