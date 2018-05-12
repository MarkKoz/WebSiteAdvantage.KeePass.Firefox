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
