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

using System;
using System.Collections.Generic;
using System.Text;
using Gurux.DLMS;
using System.ComponentModel;
using System.Xml.Serialization;
using Gurux.DLMS.ManufacturerSettings;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects.Enums;
using System.Xml;

namespace Gurux.DLMS.Objects
{
    public class GXDLMSImageTransfer : GXDLMSObject, IGXDLMSBase
    {
        UInt32 ImageSize;
        Dictionary<uint, byte[]> ImageData = new Dictionary<uint, byte[]>();
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSImageTransfer()
        : base(ObjectType.ImageTransfer, "0.0.44.0.0.255", 0)
        {
            ImageBlockSize = 200;
            ImageFirstNotTransferredBlockNumber = 0;
            ImageTransferEnabled = true;
            GXDLMSImageActivateInfo info = new GXDLMSImageActivateInfo();
            info.Size = 0;
            info.Signature = "";
            info.Identification = "";
            ImageActivateInfo = new GXDLMSImageActivateInfo[] { info };
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSImageTransfer(string ln)
        : base(ObjectType.ImageTransfer, ln, 0)
        {
            ImageBlockSize = 200;
            ImageFirstNotTransferredBlockNumber = 0;
            ImageTransferEnabled = true;
            GXDLMSImageActivateInfo info = new GXDLMSImageActivateInfo();
            info.Size = 0;
            info.Signature = "";
            info.Identification = "";
            ImageActivateInfo = new GXDLMSImageActivateInfo[] { info };
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSImageTransfer(string ln, ushort sn)
        : base(ObjectType.ImageTransfer, ln, sn)
        {
            ImageBlockSize = 200;
            ImageFirstNotTransferredBlockNumber = 0;
            ImageTransferEnabled = true;
            GXDLMSImageActivateInfo info = new GXDLMSImageActivateInfo();
            info.Size = 0;
            info.Signature = "";
            info.Identification = "";
            ImageActivateInfo = new GXDLMSImageActivateInfo[] { info };
        }

        /// <summary>
        /// Holds the ImageBlockSize, expressed in octets, which can be handled by the server.
        /// </summary>
        [XmlIgnore()]
        public UInt32 ImageBlockSize
        {
            get;
            set;
        }

        /// <summary>
        /// Provides information about the transfer status of each ImageBlock.
        /// Each bit in the bit-string provides information about one individual ImageBlock.
        /// </summary>
        [XmlIgnore()]
        public string ImageTransferredBlocksStatus
        {
            get;
            set;
        }

        /// <summary>
        /// Provides the ImageBlockNumber of the first ImageBlock not transferred.
        /// </summary>
        [XmlIgnore()]
        public long ImageFirstNotTransferredBlockNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Controls enabling the Image transfer process. The method can
        /// be invoked successfully only if the value of this attribute is true.
        /// </summary>
        [XmlIgnore()]
        public bool ImageTransferEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// Holds the status of the Image transfer process.
        /// </summary>
        [XmlIgnore()]
        public ImageTransferStatus ImageTransferStatus
        {
            get;
            set;
        }

        [XmlIgnore()]
        public GXDLMSImageActivateInfo[] ImageActivateInfo
        {
            get;
            set;
        }

        public byte[][] ImageTransferInitiate(GXDLMSClient client, string imageIdentifier, long imageSize)
        {
            if (ImageBlockSize == 0)
            {
                throw new Exception("Invalid image block size");
            }
            GXByteBuffer data = new GXByteBuffer();
            data.SetUInt8((byte)DataType.Structure);
            data.SetUInt8((byte)2);
            GXCommon.SetData(client.Settings, data, DataType.OctetString, ASCIIEncoding.ASCII.GetBytes(imageIdentifier));
            GXCommon.SetData(client.Settings, data, DataType.UInt32, imageSize);
            return client.Method(this, 1, data.Array(), DataType.Array);
        }

        public byte[][] ImageBlockTransfer(GXDLMSClient client, byte[] imageBlockValue, out int ImageBlockCount)
        {
            ImageBlockCount = (int)(imageBlockValue.Length / ImageBlockSize);
            if (imageBlockValue.Length % ImageBlockSize != 0)
            {
                ++ImageBlockCount;
            }
            List<byte[]> packets = new List<byte[]>();
            for (int pos = 0; pos != ImageBlockCount; ++pos)
            {
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Structure);
                data.SetUInt8((byte)2);
                GXCommon.SetData(client.Settings, data, DataType.UInt32, pos);
                byte[] tmp;
                int bytes = (int)(imageBlockValue.Length - ((pos + 1) * ImageBlockSize));
                //If last packet
                if (bytes < 0)
                {
                    bytes = (int)(imageBlockValue.Length - (pos * ImageBlockSize));
                    tmp = new byte[bytes];
                    Array.Copy(imageBlockValue, pos * (int)ImageBlockSize, tmp, 0, bytes);
                }
                else
                {
                    tmp = new byte[ImageBlockSize];
                    Array.Copy(imageBlockValue, (pos * (int)ImageBlockSize), tmp, 0, (int)ImageBlockSize);
                }
                GXCommon.SetData(client.Settings, data, DataType.OctetString, tmp);
                packets.AddRange(client.Method(this, 2, data.Array(), DataType.Array));
            }
            return packets.ToArray();
        }

        public byte[][] ImageVerify(GXDLMSClient client)
        {
            return client.Method(this, 3, 0, DataType.Int8);
        }

        public byte[][] ImageActivate(GXDLMSClient client)
        {
            return client.Method(this, 4, 0, DataType.Int8);
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, ImageBlockSize, ImageTransferredBlocksStatus,
                              ImageFirstNotTransferredBlockNumber, ImageTransferEnabled, ImageTransferStatus, ImageActivateInfo
                            };
        }

        #region IGXDLMSBase Members

        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e)
        {
            ImageTransferStatus = ImageTransferStatus.NotInitiated;
            //Image transfer initiate
            if (e.Index == 1)
            {
                ImageFirstNotTransferredBlockNumber = 0;
                ImageTransferredBlocksStatus = "";
                object[] value = (object[])e.Parameters;
                string ImageIdentifier = ASCIIEncoding.ASCII.GetString((byte[])value[0]);
                ImageSize = (UInt32)value[1];
                ImageTransferStatus = ImageTransferStatus.TransferInitiated;
                List<GXDLMSImageActivateInfo> list = new List<GXDLMSImageActivateInfo>(ImageActivateInfo);
                GXDLMSImageActivateInfo item = new GXDLMSImageActivateInfo();
                item.Size = ImageSize;
                item.Identification = ImageIdentifier;
                list.Add(item);
                ImageActivateInfo = list.ToArray();
                StringBuilder sb = new StringBuilder((int)ImageSize);
                for (uint pos = 0; pos < ImageSize; ++pos)
                {
                    sb.Append('0');
                }
                ImageTransferredBlocksStatus = sb.ToString();
                return new byte[] { 0 };
            }
            //Image block transfer
            else if (e.Index == 2)
            {
                object[] value = (object[])e.Parameters;
                uint imageIndex = (uint)value[0];
                char[] tmp = ImageTransferredBlocksStatus.ToCharArray();
                tmp[(int)imageIndex] = '1';
                ImageTransferredBlocksStatus = new string(tmp);
                ImageFirstNotTransferredBlockNumber = imageIndex + 1;
                ImageData[imageIndex] = (byte[])value[1];
                ImageTransferStatus = ImageTransferStatus.TransferInitiated;
                return new byte[] { 0 };
            }
            //Image verify
            else if (e.Index == 3)
            {
                ImageTransferStatus = ImageTransferStatus.VerificationInitiated;
                //Check that size match.
                int size = 0;
                foreach (KeyValuePair<uint, byte[]> it in ImageData)
                {
                    size += it.Value.Length;
                }
                if (size != ImageSize)
                {
                    //Return HW error.
                    ImageTransferStatus = ImageTransferStatus.VerificationFailed;
                    throw new Exception("Invalid image size.");
                }
                ImageTransferStatus = ImageTransferStatus.VerificationSuccessful;
                return new byte[] { 0 };
            }
            //Image activate.
            else if (e.Index == 4)
            {
                ImageTransferStatus = ImageTransferStatus.ActivationSuccessful;
                return new byte[] { 0 };
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
                return null;
            }
        }

        int[] IGXDLMSBase.GetAttributeIndexToRead()
        {
            List<int> attributes = new List<int>();
            //LN is static and read only once.
            if (string.IsNullOrEmpty(LogicalName))
            {
                attributes.Add(1);
            }
            //ImageBlockSize
            if (!IsRead(2))
            {
                attributes.Add(2);
            }
            //ImageTransferredBlocksStatus
            if (!IsRead(3))
            {
                attributes.Add(3);
            }
            //ImageFirstNotTransferredBlockNumber
            if (!IsRead(4))
            {
                attributes.Add(4);
            }
            //ImageTransferEnabled
            if (!IsRead(5))
            {
                attributes.Add(5);
            }
            //ImageTransferStatus
            if (!IsRead(6))
            {
                attributes.Add(6);
            }
            //ImageActivateInfo
            if (!IsRead(7))
            {
                attributes.Add(7);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] {Internal.GXCommon.GetLogicalNameString(),
                             "Image Block Size", "Image Transferred Blocks Status",
                             "Image FirstNot Transferred Block Number",
                             "Image Transfer Enabled", "Image Transfer Status", "Image Activate Info"
                            };
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 7;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 4;
        }

        /// <inheritdoc cref="IGXDLMSBase.GetDataType"/>
        public override DataType GetDataType(int index)
        {
            if (index == 1)
            {
                return DataType.OctetString;
            }
            if (index == 2)
            {
                return DataType.UInt32;
            }
            if (index == 3)
            {
                return DataType.BitString;
            }
            if (index == 4)
            {
                return DataType.UInt32;
            }
            if (index == 5)
            {
                return DataType.Boolean;
            }
            if (index == 6)
            {
                return DataType.Enum;
            }
            if (index == 7)
            {
                return DataType.Array;
            }
            throw new ArgumentException("GetDataType failed. Invalid attribute index.");
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                return GXCommon.LogicalNameToBytes(LogicalName);
            }
            if (e.Index == 2)
            {
                return ImageBlockSize;
            }
            if (e.Index == 3)
            {
                return ImageTransferredBlocksStatus;
            }
            if (e.Index == 4)
            {
                return ImageFirstNotTransferredBlockNumber;
            }
            if (e.Index == 5)
            {
                return ImageTransferEnabled;
            }
            if (e.Index == 6)
            {
                return ImageTransferStatus;
            }
            if (e.Index == 7)
            {
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Array);
                if (ImageActivateInfo == null)
                {
                    GXCommon.SetObjectCount(0, data);
                }
                else
                {
                    GXCommon.SetObjectCount(ImageActivateInfo.Length, data);
                    foreach (GXDLMSImageActivateInfo it in ImageActivateInfo)
                    {
                        data.SetUInt8((byte)DataType.Structure);
                        data.SetUInt8(3);
                        GXCommon.SetData(settings, data, DataType.UInt32, it.Size);
                        GXCommon.SetData(settings, data, DataType.OctetString, ASCIIEncoding.ASCII.GetBytes(Convert.ToString(it.Identification)));
                        if (it.Signature == null || it.Signature.Length == 0)
                        {
                            GXCommon.SetData(settings, data, DataType.OctetString, null);
                        }
                        else
                        {
                            GXCommon.SetData(settings, data, DataType.OctetString, ASCIIEncoding.ASCII.GetBytes(it.Signature));
                        }
                    }
                }
                return data.Array();
            }
            e.Error = ErrorCode.ReadWriteDenied;
            return null;
        }

        void IGXDLMSBase.SetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                LogicalName = GXCommon.ToLogicalName(e.Value);
            }
            else if (e.Index == 2)
            {
                ImageBlockSize = Convert.ToUInt32(e.Value);
            }
            else if (e.Index == 3)
            {
                ImageTransferredBlocksStatus = (string)e.Value;
            }
            else if (e.Index == 4)
            {
                ImageFirstNotTransferredBlockNumber = Convert.ToUInt32(e.Value);
            }
            else if (e.Index == 5)
            {
                ImageTransferEnabled = Convert.ToBoolean(e.Value);
            }
            else if (e.Index == 6)
            {
                ImageTransferStatus = (ImageTransferStatus)Convert.ToUInt32(e.Value);
            }
            else if (e.Index == 7)
            {
                ImageActivateInfo = null;
                List<GXDLMSImageActivateInfo> list = new List<GXDLMSImageActivateInfo>();
                if (e.Value != null)
                {
                    foreach (Object it in (Object[])e.Value)
                    {
                        GXDLMSImageActivateInfo item = new GXDLMSImageActivateInfo();
                        Object[] tmp = (Object[])it;
                        item.Size = Convert.ToUInt32(tmp[0]);
                        item.Identification = ASCIIEncoding.ASCII.GetString((byte[])tmp[1]);
                        item.Signature = ASCIIEncoding.ASCII.GetString((byte[])tmp[2]);
                        list.Add(item);
                    }
                }
                ImageActivateInfo = list.ToArray();
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            ImageBlockSize = (UInt32)reader.ReadElementContentAsInt("ImageBlockSize");
            ImageTransferredBlocksStatus = reader.ReadElementContentAsString("ImageTransferredBlocksStatus");
            ImageFirstNotTransferredBlockNumber = reader.ReadElementContentAsLong("ImageFirstNotTransferredBlockNumber");
            ImageTransferEnabled = reader.ReadElementContentAsInt("ImageTransferEnabled") != 0;
            ImageTransferStatus = (ImageTransferStatus)reader.ReadElementContentAsInt("ImageTransferStatus");


            List<GXDLMSImageActivateInfo> list = new List<GXDLMSImageActivateInfo>();
            if (reader.IsStartElement("ImageActivateInfo", true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    GXDLMSImageActivateInfo it = new GXDLMSImageActivateInfo();
                    it.Size = reader.ReadElementContentAsULong("Size");
                    it.Identification = reader.ReadElementContentAsString("Identification");
                    it.Signature = reader.ReadElementContentAsString("Signature");
                    list.Add(it);
                }
                reader.ReadEndElement("ImageActivateInfo");
            }
            ImageActivateInfo = list.ToArray();

        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("ImageBlockSize", ImageBlockSize.ToString());
            writer.WriteElementString("ImageTransferredBlocksStatus", ImageTransferredBlocksStatus);
            writer.WriteElementString("ImageFirstNotTransferredBlockNumber", ImageFirstNotTransferredBlockNumber.ToString());
            writer.WriteElementString("ImageTransferEnabled", ImageTransferEnabled);
            writer.WriteElementString("ImageTransferStatus", (int)ImageTransferStatus);
            if (ImageActivateInfo != null)
            {
                writer.WriteStartElement("ImageActivateInfo");
                foreach (GXDLMSImageActivateInfo it in ImageActivateInfo)
                {
                    writer.WriteStartElement("Item");
                    writer.WriteElementString("Size", it.Size.ToString());
                    writer.WriteElementString("Identification", it.Identification);
                    writer.WriteElementString("Signature", it.Signature);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
