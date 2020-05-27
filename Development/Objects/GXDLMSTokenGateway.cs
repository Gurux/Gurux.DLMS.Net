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
using System.Xml.Serialization;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Internal;
using System.Text;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Enumerates token status codes.
    /// </summary>
    ///  <remarks>
    ///  Online help:<br/>
    ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSTokenGateway
    /// </remarks>
    public enum TokenStatusCode : int
    {
        /// <summary>
        /// Token format result OK.
        /// </summary>
        FormatOk,
        /// <summary>
        /// Authentication result OK.
        /// </summary>
        AuthenticationOk,
        /// <summary>
        /// Validation result OK.
        /// </summary>
        ValidationOk,
        /// <summary>
        /// Token execution result OK.
        /// </summary>
        TokenExecutionOk,
        /// <summary>
        /// Token format failure.
        /// </summary>
        TokenFormatFailure,
        /// <summary>
        /// Authentication failure.
        /// </summary>
        AuthenticationFailure,
        /// <summary>
        /// Validation result failure.
        /// </summary>
        ValidationResultFailure,
        /// <summary>
        /// Token execution result failure.
        /// </summary>
        TokenExecutionResultFailure,
        /// <summary>
        /// Token received and not yet processed.
        /// </summary>
        TokenReceived
    }

    /// <summary>
    /// Enumerates token delivery methods.
    /// </summary>
    /// <remarks>
    ///  Online help:<br/>
    ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSTokenGateway
    /// </remarks>
    public enum TokenDelivery : byte
    {
        /// <summary>
        /// Via remote communications.
        /// </summary>
        Remote,
        /// <summary>
        /// Via local communications.
        /// </summary>
        Local,
        /// <summary>
        /// Via manual entry.
        /// </summary>
        Manual
    }

    /// <summary>
    /// Online help:<br/>
    ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSTokenGateway
    /// </summary>
    public class GXDLMSTokenGateway : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSTokenGateway()
        : this("0.0.19.40.0.255", 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSTokenGateway(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSTokenGateway(string ln, ushort sn)
        : base(ObjectType.TokenGateway, ln, sn)
        {
            Descriptions = new List<string>();
        }

        /// <summary>
        /// Token.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSTokenGateway
        /// </remarks>
        [XmlIgnore()]
        public byte[] Token
        {
            get;
            set;
        }

        /// <summary>
        /// Time
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSTokenGateway
        /// </remarks>
        [XmlIgnore()]
        public GXDateTime Time
        {
            get;
            set;
        }

        /// <summary>
        /// Descriptions.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSTokenGateway
        /// </remarks>
        [XmlIgnore()]
        public List<string> Descriptions
        {
            get;
            set;
        }

        /// <summary>
        /// Token Delivery method.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSTokenGateway
        /// </remarks>
        [XmlIgnore()]
        public TokenDelivery DeliveryMethod
        {
            get;
            set;
        }

        /// <summary>
        /// Token status code.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSTokenGateway
        /// </remarks>
        [XmlIgnore()]
        public TokenStatusCode StatusCode
        {
            get;
            set;
        }

        /// <summary>
        /// Token data value.
        /// </summary>
        /// <remarks>
        ///  Online help:<br/>
        ///  https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSTokenGateway
        /// </remarks>
        [XmlIgnore()]
        public string DataValue
        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, Token, Time, Descriptions, DeliveryMethod, new object[] { StatusCode, DataValue } };
        }

        #region IGXDLMSBase Members

        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e)
        {
            e.Error = ErrorCode.ReadWriteDenied;
            return null;
        }

        int[] IGXDLMSBase.GetAttributeIndexToRead(bool all)
        {
            List<int> attributes = new List<int>();
            //LN is static and read only once.
            if (all || string.IsNullOrEmpty(LogicalName))
            {
                attributes.Add(1);
            }
            //Token
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            //Time
            if (all || CanRead(3))
            {
                attributes.Add(3);
            }
            //Description
            if (all || CanRead(4))
            {
                attributes.Add(4);
            }
            //DeliveryMethod
            if (all || CanRead(5))
            {
                attributes.Add(5);
            }
            //Status
            if (all || CanRead(6))
            {
                attributes.Add(6);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "Token", "Time", "Description", "DeliveryMethod", "Status" };
        }

        /// <inheritdoc cref="IGXDLMSBase.GetMethodNames"/>
        string[] IGXDLMSBase.GetMethodNames()
        {
            return new string[] { "Enter" };
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 6;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 1;
        }

        public override DataType GetUIDataType(int index)
        {
            if (index == 3)
            {
                return DataType.DateTime;
            }
            return base.GetUIDataType(index);
        }


        /// <inheritdoc cref="IGXDLMSBase.GetDataType"/>
        public override DataType GetDataType(int index)
        {
            switch (index)
            {
                case 1:
                    return DataType.OctetString;
                case 2:
                    return DataType.OctetString;
                case 3:
                    return DataType.OctetString;
                case 4:
                    return DataType.Array;
                case 5:
                    return DataType.Enum;
                case 6:
                    return DataType.Structure;
                default:
                    throw new ArgumentException("GetDataType failed. Invalid attribute index.");
            }
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            GXByteBuffer bb;
            switch (e.Index)
            {
                case 1:
                    return GXCommon.LogicalNameToBytes(LogicalName);
                case 2:
                    return Token;
                case 3:
                    return Time;
                case 4:
                    bb = new GXByteBuffer();
                    bb.SetUInt8(DataType.Array);
                    if (Descriptions == null)
                    {
                        bb.SetUInt8(0);
                    }
                    else
                    {
                        bb.SetUInt8((byte)Descriptions.Count);
                        foreach (string it in Descriptions)
                        {
                            bb.SetUInt8(DataType.OctetString);
                            bb.SetUInt8((byte)it.Length);
                            bb.Set(ASCIIEncoding.ASCII.GetBytes(it));
                        }
                    }
                    return bb.Array();
                case 5:
                    return DeliveryMethod;
                case 6:
                    bb = new GXByteBuffer();
                    bb.SetUInt8(DataType.Structure);
                    bb.SetUInt8(2);
                    GXCommon.SetData(settings, bb, DataType.Enum, StatusCode);
                    GXCommon.SetData(settings, bb, DataType.BitString, DataValue);
                    return bb.Array();
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
            return null;
        }

        void IGXDLMSBase.SetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            switch (e.Index)
            {
                case 1:
                    LogicalName = GXCommon.ToLogicalName(e.Value);
                    break;
                case 2:
                    Token = (byte[])e.Value;
                    break;
                case 3:
                    if (e.Value is byte[])
                    {
                        Time = (GXDateTime)GXDLMSClient.ChangeType((byte[])e.Value, DataType.DateTime, settings != null && settings.UseUtc2NormalTime);
                    }
                    else if (e.Value is string)
                    {
                        Time = new GXDateTime((string)e.Value);
                    }
                    else
                    {
                        Time = (GXDateTime)e.Value;
                    }
                    break;
                case 4:
                    Descriptions.Clear();
                    if (e.Value != null)
                    {
                        foreach (object it in (IEnumerable<object>)e.Value)
                        {
                            Descriptions.Add(ASCIIEncoding.ASCII.GetString((byte[])it));
                        }
                    }
                    break;
                case 5:
                    DeliveryMethod = (TokenDelivery)Convert.ToByte(e.Value);
                    break;
                case 6:
                    if (e.Value != null)
                    {
                        List<object> arr;
                        if (e.Value is List<object>)
                        {
                            arr = (List<object>)e.Value;
                        }
                        else
                        {
                            arr = new List<object>((object[])e.Value);
                        }
                        StatusCode = (TokenStatusCode)Convert.ToInt32(arr[0]);
                        DataValue = Convert.ToString(arr[1]);
                    }
                    else
                    {
                        StatusCode = TokenStatusCode.FormatOk;
                        DataValue = "";
                    }
                    break;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            Token = GXCommon.HexToBytes(reader.ReadElementContentAsString("Token"));
            string tmp = reader.ReadElementContentAsString("Time");
            if (tmp != null)
            {
                Time = new GXDateTime(tmp, System.Globalization.CultureInfo.InvariantCulture);
            }
            Descriptions.Clear();
            if (reader.IsStartElement("Descriptions", true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    Descriptions.Add(reader.ReadElementContentAsString("Name"));
                }
                reader.ReadEndElement("Descriptions");
            }
            DeliveryMethod = (TokenDelivery)reader.ReadElementContentAsInt("DeliveryMethod");
            StatusCode = (TokenStatusCode)reader.ReadElementContentAsInt("Status");
            DataValue = reader.ReadElementContentAsString("Data");
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("Token", GXCommon.ToHex(Token, false), 2);
            writer.WriteElementString("Time", Time, 3);
            writer.WriteStartElement("Descriptions", 4);
            if (Descriptions != null)
            {
                foreach (string it in Descriptions)
                {
                    writer.WriteStartElement("Item", 4);
                    writer.WriteElementString("Name", it, 4);
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
            writer.WriteElementString("DeliveryMethod", (int)DeliveryMethod, 5);
            writer.WriteElementString("Status", (int)StatusCode, 6);
            writer.WriteElementString("Data", DataValue, 7);
        }
        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
