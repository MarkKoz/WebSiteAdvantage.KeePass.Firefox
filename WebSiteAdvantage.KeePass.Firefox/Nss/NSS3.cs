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

namespace WebSiteAdvantage.KeePass.Firefox.Gecko
{
    /// <summary>
    /// A Network Security Services API wraper class.
    /// </summary>
    public static class NSS3
    {
        private static readonly Logger _Logger = LogManager.GetCurrentClassLogger();

        public static void LoadDependencies()
        {
            switch (Gecko.Version)
            {
                //case "NSS310":
                //    NSS310.NSS3.LoadDependencies();
                //    break;
                case "NSS312":
                    NSS312.NSS3.LoadDependencies();
                    break;
                case "NSS64":
                    NSS64.NSS3.LoadDependencies();
                    break;
                default:
                    throw new InvalidOperationException("Unsupported Gecko version.");
            }
        }

        #region DLL Methods
        public static SECStatus NSS_Init(string profilePath)
        {
            LoadDependencies();

            switch (Gecko.Version)
            {
                //case "NSS310":
                //    return NSS310.NSS3.NSS_Init(profilePath);
                case "NSS312":
                    return NSS312.NSS3.NSS_Init(profilePath);
                case "NSS64":
                    return NSS64.NSS3.NSS_Init(profilePath);
                default:
                    throw new InvalidOperationException("Unsupported Gecko version.");
            }
        }

        public static SECStatus NSS_Shutdown()
        {
            LoadDependencies();

            switch (Gecko.Version)
            {
                //case "NSS310":
                //    return NSS310.NSS3.NSS_Shutdown();
                case "NSS312":
                    return NSS312.NSS3.NSS_Shutdown();
                case "NSS64":
                    return NSS64.NSS3.NSS_Shutdown();
                default:
                    throw new InvalidOperationException("Unsupported Gecko version.");
            }
        }

        public static IntPtr PK11_GetInternalKeySlot()
        {
            LoadDependencies();

            switch (Gecko.Version)
            {
                //case "NSS310":
                //    return NSS310.NSS3.PK11_GetInternalKeySlot();
                case "NSS312":
                    return NSS312.NSS3.PK11_GetInternalKeySlot();
                case "NSS64":
                    return NSS64.NSS3.PK11_GetInternalKeySlot();
                default:
                    throw new InvalidOperationException("Unsupported Gecko version.");
            }
        }

        public static void PK11_FreeSlot(IntPtr slot)
        {
            LoadDependencies();

            switch (Gecko.Version)
            {
                //case "NSS310":
                //    NSS310.NSS3.PK11_FreeSlot(slot);
                //    break;
                case "NSS312":
                    NSS312.NSS3.PK11_FreeSlot(slot);
                    break;
                case "NSS64":
                    NSS64.NSS3.PK11_FreeSlot(slot);
                    break;
                default:
                    throw new InvalidOperationException("Unsupported Gecko version.");
            }
        }

        public static SECStatus PK11_CheckUserPassword(IntPtr slot, string password)
        {
            LoadDependencies();

            switch (Gecko.Version)
            {
                //case "NSS310":
                //    return NSS310.NSS3.PK11_CheckUserPassword(slot, password);
                case "NSS312":
                    return NSS312.NSS3.PK11_CheckUserPassword(slot, password);
                case "NSS64":
                    return NSS64.NSS3.PK11_CheckUserPassword(slot, password);
                default:
                    throw new InvalidOperationException("Unsupported Gecko version.");
            }
        }

        public static SECStatus PK11_Authenticate(IntPtr slot, bool loadCerts, IntPtr wincx)
        {
            LoadDependencies();

            switch (Gecko.Version)
            {
                //case "NSS310":
                //    return NSS310.NSS3.PK11_Authenticate(slot, loadCerts, wincx);
                case "NSS312":
                    return NSS312.NSS3.PK11_Authenticate(slot, loadCerts, wincx);
                case "NSS64":
                    return NSS64.NSS3.PK11_Authenticate(slot, loadCerts, wincx);
                default:
                    throw new InvalidOperationException("Unsupported Gecko version.");
            }
        }

        public static SECStatus PK11SDR_Decrypt(IntPtr encryptedItem, ref SECItem text, IntPtr cx)
        {
            LoadDependencies();

            switch (Gecko.Version)
            {
                //case "NSS310":
                //    return NSS310.NSS3.PK11SDR_Decrypt(encryptedItem, ref  text, cx);
                case "NSS312":
                    return NSS312.NSS3.PK11SDR_Decrypt(encryptedItem, ref  text, cx);
                case "NSS64":
                    return NSS64.NSS3.PK11SDR_Decrypt(encryptedItem, ref  text, cx);
                default:
                    throw new InvalidOperationException("Unsupported Gecko version.");
            }
        }

        public static IntPtr NSSBase64_DecodeBuffer(IntPtr p1, IntPtr p2, string encoded, int encoded_len)
        {
            LoadDependencies();

            switch (Gecko.Version)
            {
                //case "NSS310":
                //    return NSS310.NSS3.NSSBase64_DecodeBuffer(p1, p2, encoded, encoded_len);
                case "NSS312":
                    return NSS312.NSS3.NSSBase64_DecodeBuffer(p1, p2, encoded, encoded_len);
                case "NSS64":
                    return NSS64.NSS3.NSSBase64_DecodeBuffer(p1, p2, encoded, encoded_len);
                default:
                    throw new InvalidOperationException("Unsupported Gecko version.");
            }
        }

        public static void SECITEM_FreeItem(ref SECItem item, int bDestroy)
        {
            LoadDependencies();

            switch (Gecko.Version)
            {
                //case "NSS310":
                //    NSS310.NSS3.SECITEM_FreeItem(ref  item, bDestroy);
                //    break;
                case "NSS312":
                    NSS312.NSS3.SECITEM_FreeItem(ref  item, bDestroy);
                    break;
                case "NSS64":
                    NSS64.NSS3.SECITEM_FreeItem(ref  item, bDestroy);
                    break;
                default:
                    throw new InvalidOperationException("Unsupported Gecko version.");
            }
        }

        public static void SECITEM_FreeItem(IntPtr item, int bDestroy)
        {
            LoadDependencies();

            switch (Gecko.Version)
            {
                //case "NSS310":
                //    NSS310.NSS3.SECITEM_FreeItem(item, bDestroy);
                //    break;
                case "NSS312":
                    NSS312.NSS3.SECITEM_FreeItem(item, bDestroy);
                    break;
                case "NSS64":
                    NSS64.NSS3.SECITEM_FreeItem(item, bDestroy);
                    break;
                default:
                    throw new InvalidOperationException("Unsupported Gecko version.");
            }
        }
        #endregion

        #region Utilities
        public static string DecodeAndDecrypt(string data)
        {
            if (data == null)
                return null;

            if (data.StartsWith("~"))
            {
                data = data.Substring(1);

                string buf = null;
                Decode(data, ref buf);
                return buf;
            }
            else
            {
                string buf = null;
                Decrypt(data, ref buf);

                return buf;
            }
        }

        /// <summary>
        /// use NSS to decode and decrypt a string
        /// </summary>
        /// <param name="base64EncryptedData">data that is encrypted and then base64 encoded</param>
        /// <param name="result">clear text result</param>
        /// <returns>success status</returns>
        private static SECStatus Decrypt(string base64EncryptedData, ref string result)
        {
            var status = SECStatus.Success;
            var decodedItem = new SECItem();
            IntPtr decodedObject = IntPtr.Zero;
            result = string.Empty;

            decodedItem.Data = IntPtr.Zero;
            decodedItem.Length = 0;

            try
            {

                decodedObject = NSSBase64_DecodeBuffer(IntPtr.Zero, IntPtr.Zero, base64EncryptedData, base64EncryptedData.Length);

                if (decodedObject == IntPtr.Zero)
                {
                    status = SECStatus.Failure;
                }
                else
                {
                    status = PK11SDR_Decrypt(decodedObject, ref decodedItem, IntPtr.Zero);

                    if (status != SECStatus.Success)
                        throw new NsprException("Failed to decrypt data.");

                    try
                    {
                        result = Marshal.PtrToStringAnsi(decodedItem.Data, decodedItem.Length);
                    }
                    finally
                    {
                        SECITEM_FreeItem(ref decodedItem, 0);
                    }

                }
            }
            catch(Exception ex)
            {

                status = SECStatus.Failure;
                _Logger.Error(ex, "Decription failed.");
            }
            finally
            {
                if (decodedObject != IntPtr.Zero)
                {
                    SECITEM_FreeItem(decodedObject, 1);
                }

                if (decodedItem.Data != IntPtr.Zero)
                    Marshal.FreeHGlobal(decodedItem.Data);
            }

            return status;
        }

        /// <summary>
        /// use NSS to decode a string
        /// </summary>
        /// <param name="base64Data">data that is base64 encoded</param>
        /// <param name="result">clear text result</param>
        /// <returns>success status</returns>
        private static SECStatus Decode(string base64Data, ref string result)
        {
            var status = SECStatus.Success;
            var decodedItem = new SECItem();
            IntPtr decodedObject = IntPtr.Zero;
            result = string.Empty;

            try
            {
                decodedObject = NSSBase64_DecodeBuffer(IntPtr.Zero, IntPtr.Zero, base64Data, base64Data.Length);

                if (decodedObject == IntPtr.Zero)
                {
                    status = SECStatus.Failure;
                }
                else
                {
                    try
                    {
                        decodedItem = (SECItem)Marshal.PtrToStructure(decodedObject, typeof(SECItem));
                        result = Marshal.PtrToStringAnsi(decodedItem.Data, decodedItem.Length);
                    }
                    finally
                    {
                        SECITEM_FreeItem(decodedObject, 1);
                    }
                }
            }
            catch(Exception ex)
            {
                status = SECStatus.Failure;
                _Logger.Error(ex, "Decoding failed.");
            }
            finally
            {
                if (decodedObject != IntPtr.Zero)
                    SECITEM_FreeItem(decodedObject, 1);

                if (decodedItem.Data != IntPtr.Zero)
                    Marshal.FreeHGlobal(decodedItem.Data);
            }

            return status;
        }

        #endregion
    }
}
