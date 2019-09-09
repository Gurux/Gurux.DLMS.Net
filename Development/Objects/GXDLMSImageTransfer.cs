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
using System.Text;
using System.Xml.Serialization;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects.Enums;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Online help:
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSImageTransfer
    /// </summary>
    public class GXDLMSImageTransfer : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Image size.
        /// </summary>
        public UInt32 ImageSize
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSImageTransfer()
        : this("0.0.44.0.0.255", 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSImageTransfer(string ln)
        : this(ln, 0)
        {
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
            ImageActivateInfo = null;
            ImageTransferStatus = ImageTransferStatus.NotInitiated;
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

        public byte[][] ImageTransferInitiate(GXDLMSClient client, byte[] imageIdentifier, long imageSize)
        {
            if (ImageBlockSize == 0)
            {
                throw new Exception("Invalid image block size");
            }
            GXByteBuffer data = new GXByteBuffer();
            data.SetUInt8((byte)DataType.Structure);
            data.SetUInt8((byte)2);
            GXCommon.SetData(client.Settings, data, DataType.OctetString, imageIdentifier);
            GXCommon.SetData(client.Settings, data, DataType.UInt32, imageSize);
            return client.Method(this, 1, data.Array(), DataType.Array);
        }

        public byte[][] ImageTransferInitiate(GXDLMSClient client, string imageIdentifier, long imageSize)
        {
            return ImageTransferInitiate(client, ASCIIEncoding.ASCII.GetBytes(imageIdentifier), imageSize);
        }

        /// <summary>
        /// Move image to the meter.
        /// </summary>
        /// <param name="client">DLMS Client.</param>
        /// <param name="imageBlock">Image</param>
        /// <param name="ImageBlockCount"></param>
        /// <returns></returns>
        public byte[][] ImageBlockTransfer(GXDLMSClient client, byte[] image, out int ImageBlockCount)
        {
            List<byte[]> packets = new List<byte[]>();
            byte[][] blocks = GetImageBlocks(image);
            ImageBlockCount = blocks.Length;
            foreach (byte[] it in blocks)
            {
                packets.AddRange(client.Method(this, 2, it, DataType.Array));
            }
            return packets.ToArray();
        }

        /// <summary>
        /// Returns image blocks to send to the meter.
        /// </summary>
        /// <param name="image">Image.</param>
        /// <returns>Sent blocks.</returns>
        public byte[][] GetImageBlocks(byte[] image)
        {
            int cnt = (int)(image.Length / ImageBlockSize);
            if (image.Length % ImageBlockSize != 0)
            {
                ++cnt;
            }
            List<byte[]> packets = new List<byte[]>();
            for (int pos = 0; pos != cnt; ++pos)
            {
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Structure);
                data.SetUInt8((byte)2);
                GXCommon.SetData(null, data, DataType.UInt32, pos);
                byte[] tmp;
                int bytes = (int)(image.Length - ((pos + 1) * ImageBlockSize));
                //If last packet
                if (bytes < 0)
                {
                    bytes = (int)(image.Length - (pos * ImageBlockSize));
                    tmp = new byte[bytes];
                    Array.Copy(image, pos * (int)ImageBlockSize, tmp, 0, bytes);
                }
                else
                {
                    tmp = new byte[ImageBlockSize];
                    Array.Copy(image, (pos * (int)ImageBlockSize), tmp, 0, (int)ImageBlockSize);
                }
                GXCommon.SetData(null, data, DataType.OctetString, tmp);
                packets.Add(data.Array());
            }
            return packets.ToArray();
        }

        /// <summary>
        /// Verify image.
        /// </summary>
        /// <param name="client">DLMS Client.</param>
        /// <returns>Bytes send to the meter.</returns>
        public byte[][] ImageVerify(GXDLMSClient client)
        {
            return client.Method(this, 3, 0, DataType.Int8);
        }

        /// <summary>
        /// Activate image.
        /// </summary>
        /// <param name="client">DLMS Client.</param>
        /// <returns>Bytes send to the meter.</returns>
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
            //Image transfer initiate
            if (e.Index == 1)
            {
                ImageFirstNotTransferredBlockNumber = 0;
                ImageTransferredBlocksStatus = "";
                List<object> value = (List<object>)e.Parameters;
                byte[] imageIdentifier = (byte[])value[0];
                ImageSize = (UInt32)value[1];
                ImageTransferStatus = ImageTransferStatus.TransferInitiated;
                List<GXDLMSImageActivateInfo> list;
                if (ImageActivateInfo == null)
                {
                    list = new List<GXDLMSImageActivateInfo>();
                }
                else
                {
                    list = new List<GXDLMSImageActivateInfo>(ImageActivateInfo);
                }
                GXDLMSImageActivateInfo item = null;
                foreach (GXDLMSImageActivateInfo it in list)
                {
                    if (it.Identification == imageIdentifier)
                    {
                        item = it;
                        break;
                    }
                }
                if (item == null)
                {
                    item = new Objects.GXDLMSImageActivateInfo();
                    list.Add(item);
                }
                item.Size = ImageSize;
                item.Identification = imageIdentifier;
                ImageActivateInfo = list.ToArray();
                int cnt = (int)Math.Ceiling((double)ImageSize / ImageBlockSize);
                StringBuilder sb = new StringBuilder(cnt);
                for (uint pos = 0; pos < cnt; ++pos)
                {
                    sb.Append('0');
                }
                ImageTransferredBlocksStatus = sb.ToString();
                return null;
            }
            //Image block transfer
            else if (e.Index == 2)
            {
                List<object> value = (List<object>)e.Parameters;
                uint imageIndex = (uint)value[0];
                char[] tmp = ImageTransferredBlocksStatus.ToCharArray();
                tmp[(int)imageIndex] = '1';
                ImageTransferredBlocksStatus = new string(tmp);
                ImageFirstNotTransferredBlockNumber = imageIndex + 1;
                return null;
            }
            //Image verify
            else if (e.Index == 3)
            {
                return null;
            }
            //Image activate.
            else if (e.Index == 4)
            {
                return null;
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
                return null;
            }
        }

        int[] IGXDLMSBase.GetAttributeIndexToRead(bool all)
        {
            List<int> attributes = new List<int>();
            //LN is static and read only once.
            if (all || string.IsNullOrEmpty(LogicalName))
            {
                attributes.Add(1);
            }
            //ImageBlockSize
            if (all || !IsRead(2))
            {
                attributes.Add(2);
            }
            //ImageTransferredBlocksStatus
            attributes.Add(3);
            //ImageFirstNotTransferredBlockNumber
            attributes.Add(4);
            //ImageTransferEnabled
            if (all || !IsRead(5))
            {
                attributes.Add(5);
            }
            //ImageTransferStatus
            attributes.Add(6);
            //ImageActivateInfo
            attributes.Add(7);
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

        private object GetImageActivateInfo(GXDLMSSettings settings)
        {
            GXByteBuffer data = new GXByteBuffer();
            data.SetUInt8((byte)DataType.Array);
            //ImageActivateInfo is returned only after verification is succeeded.
            if (ImageTransferStatus != ImageTransferStatus.VerificationSuccessful || ImageActivateInfo == null)
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
                    if (it.Identification == null)
                    {
                        GXCommon.SetData(settings, data, DataType.OctetString, null);
                    }
                    else
                    {
                        GXCommon.SetData(settings, data, DataType.OctetString, it.Identification);
                    }
                    if (it.Signature == null || it.Signature.Length == 0)
                    {
                        GXCommon.SetData(settings, data, DataType.OctetString, null);
                    }
                    else
                    {
                        GXCommon.SetData(settings, data, DataType.OctetString, it.Signature);
                    }
                }
            }
            return data.Array();
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            object ret;
            switch (e.Index)
            {
                case 1:
                    ret = GXCommon.LogicalNameToBytes(LogicalName);
                    break;
                case 2:
                    ret = ImageBlockSize;
                    break;
                case 3:
                    ret = ImageTransferredBlocksStatus;
                    break;
                case 4:
                    ret = ImageFirstNotTransferredBlockNumber;
                    break;
                case 5:
                    ret = ImageTransferEnabled;
                    break;
                case 6:
                    ret = ImageTransferStatus;
                    break;
                case 7:
                    ret = GetImageActivateInfo(settings);
                    break;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    ret = null;
                    break;
            }
            return ret;
        }
        private void SetImageActivateInfo(object value)
        {
            ImageActivateInfo = null;
            List<GXDLMSImageActivateInfo> list = new List<GXDLMSImageActivateInfo>();
            if (value != null)
            {
                foreach (List<object> it in (List<object>)value)
                {
                    GXDLMSImageActivateInfo item = new GXDLMSImageActivateInfo();
                    item.Size = Convert.ToUInt32(it[0]);
                    item.Identification = (byte[])it[1];
                    item.Signature = (byte[])it[2];
                    list.Add(item);
                }
            }
            ImageActivateInfo = list.ToArray();
        }

        void IGXDLMSBase.SetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            switch (e.Index)
            {
                case 1:
                    LogicalName = GXCommon.ToLogicalName(e.Value);
                    break;
                case 2:
                    ImageBlockSize = Convert.ToUInt32(e.Value);
                    break;
                case 3:
                    ImageTransferredBlocksStatus = Convert.ToString(e.Value);
                    break;
                case 4:
                    ImageFirstNotTransferredBlockNumber = Convert.ToUInt32(e.Value);
                    break;
                case 5:
                    ImageTransferEnabled = Convert.ToBoolean(e.Value);
                    break;
                case 6:
                    ImageTransferStatus = (ImageTransferStatus)Convert.ToUInt32(e.Value);
                    break;
                case 7:
                    SetImageActivateInfo(e.Value);
                    break;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
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
                    it.Identification = GXCommon.HexToBytes(reader.ReadElementContentAsString("Identification"));
                    it.Signature = GXCommon.HexToBytes(reader.ReadElementContentAsString("Signature"));
                    list.Add(it);
                }
                reader.ReadEndElement("ImageActivateInfo");
            }
            ImageActivateInfo = list.ToArray();

        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("ImageBlockSize", ImageBlockSize);
            writer.WriteElementString("ImageTransferredBlocksStatus", ImageTransferredBlocksStatus);
            writer.WriteElementString("ImageFirstNotTransferredBlockNumber", ImageFirstNotTransferredBlockNumber);
            writer.WriteElementString("ImageTransferEnabled", ImageTransferEnabled);
            writer.WriteElementString("ImageTransferStatus", (int)ImageTransferStatus);
            if (ImageActivateInfo != null)
            {
                writer.WriteStartElement("ImageActivateInfo");
                foreach (GXDLMSImageActivateInfo it in ImageActivateInfo)
                {
                    writer.WriteStartElement("Item");
                    writer.WriteElementString("Size", it.Size);
                    writer.WriteElementString("Identification", GXCommon.ToHex(it.Identification, false));
                    writer.WriteElementString("Signature", GXCommon.ToHex(it.Signature, false));
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
