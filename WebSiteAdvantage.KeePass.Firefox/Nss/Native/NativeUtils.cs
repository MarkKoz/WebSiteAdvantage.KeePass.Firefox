using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace WebSiteAdvantage.KeePass.Firefox.Nss.Native
{
    internal static class NativeUtils
    {
        /// <summary>
        /// Base path for native library modules.
        /// </summary>
        public static string BasePath => Path.Combine(
            Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath),
            @"Nss\",
            Environment.Is64BitProcess ? @"x64\" : @"x86\");

        /// <summary>
        /// Attempts to load a native library module. Wraps <see cref="NativeMethods.LoadLibrary"/>.
        /// </summary>
        /// <exception cref="FileLoadException">Thrown when the library fails to load.</exception>
        /// <exception cref="Win32Exception">Thrown when the DLL search path fails be set.</exception>
        /// <param name="name">The name of the library to load.</param>
        public static void LoadLibrary(string name)
        {
            if (!NativeMethods.SetDllDirectory(BasePath))
                throw new Win32Exception(Marshal.GetLastWin32Error());

            if (NativeMethods.LoadLibrary(name) == IntPtr.Zero)
                throw new FileLoadException(
                    $"Could not load the library {name} at {BasePath}",
                    name,
                    new Win32Exception(Marshal.GetLastWin32Error()));

            // Resets the search path.
            if (!NativeMethods.SetDllDirectory(null))
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }
}
