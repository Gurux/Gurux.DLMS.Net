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
// More information of Gurux products: http://www.gurux.org
//
// This code is licensed under the GNU General Public License v2. 
// Full text may be retrieved at http://www.gnu.org/licenses/gpl-2.0.txt
//---------------------------------------------------------------------------

//
// --------------------------------------------------------------------------
//  Gurux Ltd
// 
//
//
// Filename:        $HeadURL: svn://utopia/projects/Gurux.DLMS/Development/ManufacturerSettings/GXManufacturer.cs $
//
// Version:         $Revision: 4781 $,
//                  $Date: 2012-03-19 10:23:38 +0200 (ma, 19 maalis 2012) $
//                  $Author: kurumi $
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
// More information of Gurux DLMS/COSEM Director: http://www.gurux.org/Gurux.DLMS
//
// This code is licensed under the GNU General Public License v2. 
// Full text may be retrieved at http://www.gnu.org/licenses/gpl-2.0.txt
//---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;
using Gurux.DLMS;

namespace Gurux.DLMS.ManufacturerSettings
{
    public enum StartProtocolType
    {
        IEC = 1,
        DLMS = 2
    }

    /// <summary>
    /// Enumerates inactivity modes that are used, when communicating with IEC using serial port connection.
    /// </summary>
    public enum InactivityMode
    {
        None = 0,
        /// <summary>
        /// Keep alive message is sent, only if there is no traffic on the active connection.
        /// </summary>
        KeepAlive = 1,
        /// <summary>
        /// Connection is reopened, if there is no traffic on the active connection.
        /// </summary>
        Reopen,
        /// <summary>
        /// Connection is reopened, even if there is traffic on the active connection.
        /// </summary>
        ReopenActive,
        /// <summary>
        /// Closes connection, if there is no traffic on the active connection.
        /// </summary>
        Disconnect
    }

    [Serializable]
    public class GXManufacturer
    {
        private GXObisCodeCollection m_ObisCodes;
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXManufacturer()
        {
            InactivityMode = InactivityMode.KeepAlive;
            StartProtocol = StartProtocolType.IEC;
            m_ObisCodes = new GXObisCodeCollection();
            Settings = new List<GXAuthentication>();
            ServerSettings = new List<GXServerAddress>();
            KeepAliveInterval = 40000;
        }      

        /// <summary>
        /// Manufacturer Identification.
        /// </summary>
        /// <remarks>
        /// Device returns manufacturer ID when connection to the meter is made.
        /// </remarks>
        public String Identification
        {
            get;
            set;
        }

        /// <summary>
        /// Real name of the manufacturer.
        /// </summary>
        public String Name
        {
            get;
            set;
        }

        /// <summary>
        /// Is Logical name referencing used.
        /// </summary>
        [DefaultValue(false)]
        [XmlElement("UseLN")]
        public bool UseLogicalNameReferencing
        {
            get;
            set;
        }       
        
        /// <summary>
        /// Is Keep alive message used.
        /// </summary>
        [DefaultValue(InactivityMode.KeepAlive)]
        public InactivityMode InactivityMode
        {
            get;
            set;
        }

        /// <summary>
        /// Is Keep alive message forced for network connection as well.
        /// </summary>
        [DefaultValue(false)]
        public bool ForceInactivity
        {
            get;
            set;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Browsable(false)]
        public GXObisCodeCollection ObisCodes
        {
            get
            {
                return m_ObisCodes;
            }
        }

        /// <summary>
        /// Is IEC 62056-47 supported.
        /// </summary>
        [DefaultValue(false)]
        public bool UseIEC47
        {
            get;
            set;
        }

        /// <summary>
        /// Is IEC 62056-21 skipped when using serial port connection.
        /// </summary>
        [DefaultValue(StartProtocolType.IEC)]
        public StartProtocolType StartProtocol
        {
            get;
            set;
        }

        /// <summary>
        /// Keep Alive interval.
        /// </summary>
        [DefaultValue(40000)]
        public int KeepAliveInterval
        {
            get;
            set;
        }

        /// <summary>
        /// Used extension class.
        /// </summary>
        public string Extension
        {
            get;
            set;
        } 

        [Browsable(false)]
        [System.Xml.Serialization.XmlIgnore()]
        public bool Removed
        {
            get;
            set;
        }               
       
        /// <summary>
        /// Initialize settings.
        /// </summary>
        // [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<GXAuthentication> Settings
        {
            get;
            set;
        }

        /// <summary>
        /// Server settings
        /// </summary>
        public List<GXServerAddress> ServerSettings
        {
            get;
            set;
        }

        public GXAuthentication GetAuthentication(Authentication authentication)
        {
            foreach (GXAuthentication it in Settings)
            {
                if (it.Type == authentication)
                {
                    return it;
                }
            }
            return null;
        }

        public GXAuthentication GetActiveAuthentication()
        {
            foreach (GXAuthentication it in Settings)
            {
                if (it.Selected)
                {
                    return it;
                }
            }
            if (Settings.Count != 0)
            {
                return Settings[0];
            }
            return null;
        }

        public GXServerAddress GetServer(HDLCAddressType type)
        {
            foreach (GXServerAddress it in this.ServerSettings)
            {
                if (it.HDLCAddress == type)
                {
                    return it;
                }
            }
            return null;
        }

        public GXServerAddress GetActiveServer()
        {
            foreach (GXServerAddress it in this.ServerSettings)
            {
                if (it.Selected)
                {
                    return it;
                }
            }
            if (this.ServerSettings.Count != 0)
            {
                return this.ServerSettings[0];
            }
            return null;
        }

        /// <summary>
        /// Count serial nummber.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        static int EvaluateSN(int sn, string expression)
        {
            expression = expression.Replace("SN", sn.ToString());
            var loDataTable = new System.Data.DataTable();
            var loDataColumn = new System.Data.DataColumn("Eval", typeof(int), expression);
            loDataTable.Columns.Add(loDataColumn);
            loDataTable.Rows.Add(0);
            return (int)(loDataTable.Rows[0]["Eval"]);
        }

        /// <summary>
        /// Count server address from physical and logical addresses.
        /// </summary>
        /// <param name="addressing"></param>
        /// <param name="formula"></param>
        /// <param name="physicalAddress"></param>
        /// <param name="LogicalAddress"></param>
        /// <returns></returns>
        public static object CountServerAddress(HDLCAddressType addressing, string formula, object physicalAddress, int LogicalAddress)
        {
            object value;
            if (addressing == HDLCAddressType.Custom)
            {
                value = Convert.ChangeType(physicalAddress, physicalAddress.GetType());
            }
            else if (addressing == HDLCAddressType.Default || addressing == HDLCAddressType.SerialNumber)
            {
                if (addressing == HDLCAddressType.SerialNumber)
                {
                    Type type = physicalAddress.GetType();
                    if (type != typeof(byte) && type != typeof(UInt16) && type != typeof(UInt32))
                    {
                        type = typeof(UInt16);
                    }
                    physicalAddress = Convert.ChangeType(EvaluateSN(Convert.ToInt32(physicalAddress), formula), type);
                }
                if (physicalAddress is byte)
                {
                    value = ((LogicalAddress & 0x7) << 5) | (((byte)physicalAddress & 0x7) << 1) | 0x1;
                    value = Convert.ChangeType(value, typeof(byte));
                }
                else if (physicalAddress is UInt16)
                {
                    int physicalID = Convert.ToInt32(physicalAddress);
                    int logicalID = Convert.ToInt32(LogicalAddress);
                    int total = (physicalID) << 1 | 1;
                    value = Convert.ToUInt32(total | (logicalID << 9));                    
                    value = Convert.ChangeType(value, typeof(UInt16));
                }
                else if (physicalAddress is UInt32)
                {
                    int physicalID = Convert.ToInt32(physicalAddress);
                    int logicalID = Convert.ToInt32(LogicalAddress);
                    int total = (((physicalID >> 7) & 0x7F) << 8) | (physicalID & 0x7F);
                    value = Convert.ToUInt32(((total << 1) | 1 | (logicalID << 17)));
                }
                else
                {
                    throw new Exception("Unknown physical address type.");
                }
            }
            else
            {
                throw new Exception("Invalid HDLCAddressing");
            }
            return value;
        }
    }
}