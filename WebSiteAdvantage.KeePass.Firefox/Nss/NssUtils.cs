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
using System.Runtime.InteropServices;

using NLog;

using static WebSiteAdvantage.KeePass.Firefox.Nss.Native.NssNativeMethods;

namespace WebSiteAdvantage.KeePass.Firefox.Nss
{
    /// <summary>
    /// Contains utility methods for the Network Security Services API.
    /// </summary>
    internal static class NssUtils
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Decodes and, if encrypted, decrypts Base64-encoded data.
        /// </summary>
        /// <param name="data">The data to decode (and decrypt).</param>
        /// <returns>The decoded (and decrypted) data.</returns>
        public static string DecodeAndDecrypt(string data)
        {
            if (data == null)
                return null;

            string buf = null;

            if (data.StartsWith("~"))
            {
                data = data.Substring(1);
                Decode(data, ref buf);
            }
            else
            {
                Decrypt(data, ref buf);
            }

            return buf;
        }

        /// <summary>
        /// Decodes and decrypts Base64-encoded encrypted data.
        /// </summary>
        /// <param name="data">The data to decode and decrypt.</param>
        /// <param name="result">The decoded and decrypted data.</param>
        /// <returns>The status of the function's execution.</returns>
        private static SecStatus Decrypt(string data, ref string result)
        {
            var status = SecStatus.Success;
            var decodedItem = new SecItem { Data = IntPtr.Zero, Length = 0 };
            IntPtr decodedObject = IntPtr.Zero;
            result = string.Empty;

            try
            {
                decodedObject = NSSBase64_DecodeBuffer(IntPtr.Zero, IntPtr.Zero, data, data.Length);

                if (decodedObject == IntPtr.Zero)
                {
                    status = SecStatus.Failure;
                }
                else
                {
                    status = PK11SDR_Decrypt(decodedObject, ref decodedItem, IntPtr.Zero);

                    if (status != SecStatus.Success)
                        throw new NsprException("Failed to decrypt data.");

                    try
                    {
                        result = Marshal.PtrToStringAnsi(decodedItem.Data, decodedItem.Length);
                    }
                    finally
                    {
                        SECITEM_FreeItem(ref decodedItem, false);
                    }

                }
            }
            catch(Exception ex)
            {

                status = SecStatus.Failure;
                Logger.Error(ex, "Decryption failed.");
            }
            finally
            {
                if (decodedObject != IntPtr.Zero)
                    SECITEM_FreeItem(decodedObject, true);

                if (decodedItem.Data != IntPtr.Zero)
                    Marshal.FreeHGlobal(decodedItem.Data);
            }

            return status;
        }

        /// <summary>
        /// Decodes Base64-encoded data.
        /// </summary>
        /// <param name="data">The data to decode.</param>
        /// <param name="result">The decoded data.</param>
        /// <returns>The status of the function's execution.</returns>
        private static SecStatus Decode(string data, ref string result)
        {
            var status = SecStatus.Success;
            var decodedItem = new SecItem();
            IntPtr decodedObject = IntPtr.Zero;
            result = string.Empty;

            try
            {
                decodedObject = NSSBase64_DecodeBuffer(IntPtr.Zero, IntPtr.Zero, data, data.Length);

                if (decodedObject == IntPtr.Zero)
                {
                    status = SecStatus.Failure;
                }
                else
                {
                    try
                    {
                        decodedItem = (SecItem)Marshal.PtrToStructure(decodedObject, typeof(SecItem));
                        result = Marshal.PtrToStringAnsi(decodedItem.Data, decodedItem.Length);
                    }
                    finally
                    {
                        SECITEM_FreeItem(decodedObject, true);
                    }
                }
            }
            catch(Exception ex)
            {
                status = SecStatus.Failure;
                Logger.Error(ex, "Decoding failed.");
            }
            finally
            {
                if (decodedObject != IntPtr.Zero)
                    SECITEM_FreeItem(decodedObject, true);

                if (decodedItem.Data != IntPtr.Zero)
                    Marshal.FreeHGlobal(decodedItem.Data);
            }

            return status;
        }
    }
}
