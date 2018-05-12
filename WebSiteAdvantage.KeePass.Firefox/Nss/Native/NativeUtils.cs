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
            new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath,
            @"Nss\",
            Environment.Is64BitProcess ? @"x64\" : @"x86\");

        /// <summary>
        /// Attempts to load a native library module. Wraps <see cref="NativeMethods.LoadLibrary"/>.
        /// </summary>
        /// <exception cref="FileNotFoundException">Thrown when the library cannot be found.</exception>
        /// <exception cref="FileLoadException">Thrown when the library fails to load.</exception>
        /// <param name="name">The name of the library to load.</param>
        public static void LoadLibrary(string name)
        {
            string path = Path.Combine(BasePath, name);

            if (!File.Exists(path))
                throw new FileNotFoundException($"Could not find the library {name} at {path}", name);

            if (NativeMethods.LoadLibrary(Path.Combine(BasePath, name)) == IntPtr.Zero)
                throw new FileLoadException(
                    $"Could not load the library {name} at {path}",
                    name,
                    new Win32Exception(Marshal.GetLastWin32Error()));
        }
    }
}
