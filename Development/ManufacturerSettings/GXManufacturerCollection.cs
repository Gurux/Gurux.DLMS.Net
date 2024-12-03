//
// --------------------------------------------------------------------------
//  Gurux Ltd
//
//
//
// Filename:        $HeadURL$
//
// Version:         $Revision$,
//                  $Date$
//                  $Author$
//
// Copyright (c) Gurux Ltd
//
//---------------------------------------------------------------------------
//
//  DESCRIPTION
//
// This file is a part of Gurux Device Framework.
//
// Gurux Device Framework is Open Source software; you can redistribute it
// and/or modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; version 2 of the License.
// Gurux Device Framework is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU General Public License for more details.
//
// More information of Gurux products: https://www.gurux.org
//
// This code is licensed under the GNU General Public License v2.
// Full text may be retrieved at http://www.gnu.org/licenses/gpl-2.0.txt
//---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Globalization;
using System.Net;

namespace Gurux.DLMS.ManufacturerSettings
{
#if !WINDOWS_UWP
    [Serializable]
#endif
    public class GXManufacturerCollection : List<GXManufacturer>
    {
        public GXManufacturer FindByIdentification(string id)
        {
            foreach (GXManufacturer it in this)
            {
                if (string.Compare(it.Identification, id, true) == 0)
                {
                    return it;
                }
            }
            return null;
        }
#if !WINDOWS_UWP

        private static string GetDownloadPath()
        {
            return "https://www.gurux.fi/obis/files.xml" + "?random = " + new Random().Next().ToString();
        }

        /// <summary>
        /// Is this first run.
        /// </summary>
        /// <returns></returns>
        public static bool IsFirstRun()
        {
            string path = Path.Combine(ObisCodesPath, "files.xml");
            if (!System.IO.Directory.Exists(ObisCodesPath))
            {
                System.IO.Directory.CreateDirectory(ObisCodesPath);
            }
            if (!System.IO.File.Exists(path))
            {
                System.IO.FileStream stream = System.IO.File.Create(path);
                stream.Close();
                GXFileInfo.UpdateFileSecurity(path);
                return true;
            }
            return new FileInfo(path).Length == 0;
        }

        /// <summary>
        /// Check if there are any updates available in Gurux www server.
        /// </summary>
        /// <returns>Returns true if there are any updates available.</returns>
        public static bool IsUpdatesAvailable()
        {
            //Do not check updates while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                return false;
            }
            string path = Path.Combine(ObisCodesPath, "files.xml");
            if (!System.IO.File.Exists(path))
            {
                return true;
            }

            System.Xml.XmlDocument xml = new System.Xml.XmlDocument();
            try
            {
                xml.Load(path);
                WebClient client = new WebClient();
                IWebProxy proxy = WebRequest.GetSystemWebProxy();
                if (proxy != null && proxy.Credentials != null)
                {
                    client.Proxy = proxy;
                }
                //This will fix the error: request was aborted could not create ssl/tls secure channel.
                //For Net45 ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                // Put the byte array into a stream and rewind it to the beginning
                MemoryStream ms = new MemoryStream(client.DownloadData(GetDownloadPath()));
                ms.Flush();
                ms.Position = 0;
                System.Xml.XmlDocument downloadsXml = new System.Xml.XmlDocument();
                downloadsXml.Load(ms);
                //Find manufacturer setting and compare dates.
                foreach (System.Xml.XmlNode it in downloadsXml.ChildNodes[1].ChildNodes)
                {
                    bool updated = true;
                    foreach (System.Xml.XmlNode node in xml.ChildNodes[1].ChildNodes)
                    {
                        if (string.Compare(node.InnerText, it.InnerText, true) == 0)
                        {
                            if (DateTime.ParseExact(node.Attributes["Modified"].Value, "dd-MM-yyyy", null).Date == DateTime.ParseExact(it.Attributes["Modified"].Value, "dd-MM-yyyy", CultureInfo.CurrentCulture).Date)
                            {
                                updated = false;
                            }
                            break;
                        }
                    }
                    if (updated)
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return true;
            }
            return false;
        }

        static protected string GetMD5Hash(byte[] data)
        {
            byte[] buff;
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            buff = md5.ComputeHash(data);
            return BitConverter.ToString(buff);
        }


        static protected string GetMD5HashFromFile(string fileName)
        {
            byte[] buff;
            using (FileStream file = new FileStream(fileName, FileMode.Open))
            {
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                buff = md5.ComputeHash(file);
                file.Close();
            }
            return BitConverter.ToString(buff);
        }

        /// <summary>
        /// Update manufacturer settings from the Gurux www server.
        /// </summary>
        /// <returns>Returns directory where backups are made. Return null if no backups are made.</returns>
        public static string UpdateManufactureSettings()
        {
            string backupPath = Path.Combine(ObisCodesPath, "backup");
            if (!System.IO.Directory.Exists(backupPath))
            {
                System.IO.Directory.CreateDirectory(backupPath);
            }
            backupPath = Path.Combine(backupPath, Guid.NewGuid().ToString());
            System.Net.WebClient client = new System.Net.WebClient();
            IWebProxy proxy = WebRequest.GetSystemWebProxy();
            if (proxy != null && proxy.Credentials != null)
            {
                client.Proxy = proxy;
            }
            //This will fix the error: request was aborted could not create ssl/tls secure channel.
            //For Net45 ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            byte[] files = client.DownloadData(GetDownloadPath());
            if (files != null)
            {
                System.IO.File.WriteAllBytes(Path.Combine(ObisCodesPath, "files.xml"), files);
                Gurux.DLMS.ManufacturerSettings.GXFileInfo.UpdateFileSecurity(Path.Combine(ObisCodesPath, "files.xml"));
                // Put the byte array into a stream and rewind it to the beginning
                MemoryStream ms = new MemoryStream(files);
                ms.Flush();
                ms.Position = 0;
                XmlDocument xml = new XmlDocument();
                xml.Load(ms);
                bool backup = false;
                foreach (XmlNode it in xml.ChildNodes[1].ChildNodes)
                {
                    string path = Path.Combine(ObisCodesPath, it.InnerText);
                    byte[] data = client.DownloadData("https://www.gurux.fi/obis/" + it.InnerText + "?random = " + new Random().Next().ToString());
                    //Make backup if file exists or content has change.
                    if (System.IO.File.Exists(path) && GetMD5Hash(data) != GetMD5HashFromFile(path))
                    {
                        if (!System.IO.Directory.Exists(backupPath))
                        {
                            System.IO.Directory.CreateDirectory(backupPath);
                        }
                        backup = true;
                        System.IO.File.Copy(path, Path.Combine(backupPath, it.InnerText));
                    }
                    System.IO.File.WriteAllBytes(path, data);
                    Gurux.DLMS.ManufacturerSettings.GXFileInfo.UpdateFileSecurity(path);
                }
                if (backup)
                {
                    return backupPath;
                }
            }
            return null;
        }
#endif

        public static string ObisCodesPath
        {
            get
            {
                string path = string.Empty;
#if WINDOWS_UWP
                path = "OBIS";
#else
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    path = Path.Combine(path, ".Gurux");
                }
                else
                {
                    //Vista: C:\ProgramData
                    //XP: c:\Program Files\Common Files
                    //XP = 5.1 & Vista = 6.0
                    if (Environment.OSVersion.Version.Major >= 6)
                    {
                        path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                    }
                    else
                    {
                        path = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles);
                    }
                    path = Path.Combine(path, "Gurux");
                }
                path = Path.Combine(path, "OBIS");
#endif
                return path;
            }
        }

        public static void ReadManufacturerSettings(GXManufacturerCollection Manufacturers)
        {
            ReadManufacturerSettings(Manufacturers, ObisCodesPath);
        }

        public static void ReadManufacturerSettings(GXManufacturerCollection Manufacturers, String path)
        {
            Manufacturers.Clear();
            if (Directory.Exists(path))
            {
                Type[] extraTypes = new Type[] { typeof(GXManufacturerCollection), typeof(GXManufacturer), typeof(GXObisCodeCollection), typeof(GXObisCode), typeof(GXObisValueItem), typeof(GXObisValueItemCollection), typeof(GXDLMSAttribute), typeof(GXAttributeCollection) };
                XmlSerializer x = new XmlSerializer(typeof(GXManufacturer), extraTypes);
                foreach (string it in Directory.GetFiles(path, "*.obx"))
                {
                    using (Stream stream = File.Open(it, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (TextReader reader = new StreamReader(stream))
                        {
                            try
                            {
                                GXManufacturer man = (GXManufacturer)x.Deserialize(reader);
                                Manufacturers.Add(man);
                            }
                            catch (Exception Ex)
                            {
                                System.Diagnostics.Debug.WriteLine(Ex.Message);
                            }
                        }
                    }
                }
            }
        }

        public void WriteManufacturerSettings()
        {
            WriteManufacturerSettings(ObisCodesPath);
        }

        public void WriteManufacturerSettings(string directory)
        {
            //Do not save empty list.
            if (this.Count != 0)
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                Type[] extraTypes = new Type[] { typeof(GXManufacturerCollection), typeof(GXManufacturer), typeof(GXObisCodeCollection), typeof(GXObisCode), typeof(GXObisValueItem), typeof(GXObisValueItemCollection), typeof(GXDLMSAttribute), typeof(GXAttributeCollection) };
                XmlSerializer x = new XmlSerializer(typeof(GXManufacturer), extraTypes);
                foreach (GXManufacturer it in this)
                {
                    string name = it.Identification.ToLower();
                    string path = Path.Combine(directory, name) + ".obx";
                    if (it.Identification == "AUX")
                    {
                        path = Path.Combine(directory, "_" + name) + ".obx";
                    }
                    else if (it.Identification == "CON")
                    {
                        path = Path.Combine(directory, "_" + name) + ".obx";
                    }
                    if (!it.Removed)
                    {
                        using (Stream stream = File.Open(path, FileMode.Create))
                        {
                            GXFileInfo.UpdateFileSecurity(path);
                            using (TextWriter writer = new StreamWriter(stream))
                            {
                                x.Serialize(writer, it);
                            }
                        }
                    }
                    else
                    {
                        File.Delete(path);
                    }
                }
            }
        }
    }
}
