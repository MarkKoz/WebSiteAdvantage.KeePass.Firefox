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
