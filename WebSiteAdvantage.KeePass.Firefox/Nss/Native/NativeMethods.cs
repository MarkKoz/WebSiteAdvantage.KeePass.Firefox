using System;
using System.Runtime.InteropServices;

namespace WebSiteAdvantage.KeePass.Firefox.Nss.Native
{
    internal static class NativeMethods
    {
        /// <summary>
        /// Loads the specified module into the address space of the calling process. The specified module may cause other
        /// modules to be loaded.
        /// </summary>
        /// <param name="lpFileName">
        /// The name of the module. This can be either a library module (a .dll file) or an executable module (an .exe file).
        /// </param>
        /// <returns>A handle to the module if successful; otherwise <c>NULL</c></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr LoadLibrary(string lpFileName);

        /// <summary>
        /// Adds a directory to the search path used to locate DLLs for the application.
        /// </summary>
        /// <param name="lpPathName">
        /// The directory to be added to the search path. If this parameter is an empty string, the call removes the current
        /// directory from the default DLL search order. If this parameter is <c>NULL</c>, the function restores the default
        /// search order.
        /// </param>
        /// <returns><c>true</c> if successful; <c>false</c> otherwise.</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetDllDirectory(string lpPathName);
    }
}
