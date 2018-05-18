using System;
using System.Runtime.InteropServices;

namespace WebSiteAdvantage.KeePass.Firefox.Nss.Native
{
    internal static class NssNativeMethods
    {
        static NssNativeMethods()
        {
            NativeUtils.LoadLibrary("mozcrt19.dll");
            NativeUtils.LoadLibrary("sqlite3.dll");
            NativeUtils.LoadLibrary("nss3.dll");
        }

        /// <summary>
        /// Opens the Cert, Key, and Security Module databases as read-only.
        /// Initialises the Random Number Generator.
        /// Does not initialise the cipher policies or enables.
        /// Default policy settings disallow all ciphers.
        /// </summary>
        /// <param name="configdir">The base directory where all the cert, key, and module datbases live.</param>
        /// <returns>The status of the function's execution.</returns>
        [DllImport("nss3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern SecStatus NSS_Init([MarshalAs(UnmanagedType.LPStr)] string configdir);

        /// <summary>
        /// Closes the Cert and Key databases.
        /// </summary>
        /// <returns>The status of the function's execution.</returns>
        [DllImport("nss3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern SecStatus NSS_Shutdown();

        /// <summary>
        /// Gets the internal key slot. FIPS has only one slot for both key slots and default slots.
        /// </summary>
        /// <returns>The retrieved key slot.</returns>
        [DllImport("nss3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr PK11_GetInternalKeySlot();

        /// <summary>
        /// Frees a key slot.
        /// </summary>
        /// <param name="slot">The key slot to free.</param>
        [DllImport("nss3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void PK11_FreeSlot(IntPtr slot);

        /// <summary>
        /// Check the user's password. Logs out before hand to make sure that the password is really being checked.
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="password"></param>
        /// <returns>The status of the function's execution.</returns>
        [DllImport("nss3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern SecStatus PK11_CheckUserPassword(IntPtr slot, string password);

        /// <summary>
        /// Ensures a slot is authenticated. Only authenticates if needed.
        /// </summary>
        /// <param name="slot">The slot to authenticate.</param>
        /// <param name="loadCerts"></param>
        /// <param name="wincx"></param>
        /// <returns>The status of the function's execution.</returns>
        [DllImport("nss3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern SecStatus PK11_Authenticate(IntPtr slot, bool loadCerts, IntPtr wincx);

        /// <summary>
        /// Decrypt a block of data produced by <c>PK11SDR_Encrypt</c>. The key used is identified by the <c>keyid</c> field
        /// within the input.
        /// </summary>
        /// <param name="data">The data to decrypt.</param>
        /// <param name="result"></param>
        /// <param name="cx"></param>
        /// <returns>The status of the function's execution.</returns>
        [DllImport("nss3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern SecStatus PK11SDR_Decrypt(IntPtr data, ref SecItem result, IntPtr cx);

        /// <summary>
        /// Perform base64 decoding from an ASCII string <paramref name="inStr"/> to a <see cref="SecItem"/>. The length of the
        /// input must be provided as <paramref name="inLen"/>. The <see cref="SecItem"/> may be provided (as
        /// <paramref name="outItemOpt"/>); you can also pass in <c>NULL</c> and the <see cref="SecItem"/> will be allocated for
        /// you.
        /// <para>
        /// In any case, the data within the <see cref="SecItem"/> will be allocated for you. All allocation will happen out of
        /// the passed-in <paramref name="arenaOpt"/>, if non-<c>NULL</c>. If <paramref name="arenaOpt"/> is <c>NULL</c>,
        /// standard allocation (heap) will be used and you will want to free the result via <see cref="SECITEM_FreeItem"/>.
        /// </para>
        /// </summary>
        /// <param name="arenaOpt"></param>
        /// <param name="outItemOpt">The <see cref="SecItem"/> into which to decode.</param>
        /// <param name="inStr">The string to decode.</param>
        /// <param name="inLen">The length of <paramref name="inStr"/></param>
        /// <returns>The <see cref="SecItem"/> (allocated or provided) if successful; <c>NULL</c> otherwise.</returns>
        [DllImport("nss3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr NSSBase64_DecodeBuffer(IntPtr arenaOpt, IntPtr outItemOpt, string inStr, int inLen);

        /// <summary>
        /// Free <paramref name="zap"/>. If <paramref name="freeit"/> is <c>true</c> then <paramref name="zap"/> itself is freed.
        /// </summary>
        /// <param name="zap">The <see cref="SecItem"/> to free.</param>
        /// <param name="freeit"></param>
        [DllImport("nss3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SECITEM_FreeItem(ref SecItem zap, [MarshalAs(UnmanagedType.I1)] bool freeit);

        /// <summary>
        /// Free <paramref name="zap"/>. If <paramref name="freeit"/> is <c>true</c> then <paramref name="zap"/> itself is freed.
        /// </summary>
        /// <param name="zap">Pointer to the <see cref="SecItem"/> to free.</param>
        /// <param name="freeit"></param>
        [DllImport("nss3.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SECITEM_FreeItem(IntPtr zap, [MarshalAs(UnmanagedType.I1)] bool freeit);
    }
}
