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
        [DllImport("kernel32.dll", EntryPoint = "LoadLibraryA", SetLastError = true)]
        public static extern IntPtr LoadLibrary(string lpFileName);
    }
}
