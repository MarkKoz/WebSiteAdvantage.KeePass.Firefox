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
using System.Runtime.InteropServices;

namespace WebSiteAdvantage.KeePass.Firefox.Nss
{
    internal enum SecItemType
    {
        Buffer = 0,
        ClearDataBuffer = 1,
        CipherDataBuffer = 2,
        DerCertBuffer = 3,
        EncodedCertBuffer = 4,
        DerNameBuffer = 5,
        EncodedNameBuffer = 6,
        AsciiNameString = 7,
        AsciiString = 8,
        DerOId = 9,
        UnsignedInteger = 10,
        UtcTime = 11,
        GeneralizedTime = 12,
        VisibleString = 13,
        Utf8String = 14,
        BmpString = 15
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SecItem
    {
        public SecItemType Type;
        public IntPtr Data;
        public int Length; // Should be uint.
    }

    /// <summary>
    /// A status code. Used by procedures that return status values.
    /// </summary>
    internal enum SecStatus
    {
        WouldBlock = -2,
        Failure = -1,
        Success = 0
    }
}
