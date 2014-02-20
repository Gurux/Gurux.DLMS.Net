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

namespace Gurux.DLMS.Objects
{
    public class GXDLMSImageTransfer : GXDLMSObject, IGXDLMSBase
    {
        string ImageIdentifier;
        UInt32 ImageSize = 0;
        Dictionary<uint, byte[]> ImageData = new Dictionary<uint, byte[]>();
        /// <summary> 
        /// Constructor.
        /// </summary> 
        public GXDLMSImageTransfer()
            : base(ObjectType.ImageTransfer, "0.0.44.0.0.255", 0)
        {
            ImageBlockSize = 200;
            ImageFirstNotTransferredBlockNumber = 0;
            ImageTransferEnabled = false;
            GXDLMSImageActivateInfo info = new GXDLMSImageActivateInfo();
            info.Size = 0;
            info.Signature = "";
            info.Identification = "";
            ImageActivateInfo = new GXDLMSImageActivateInfo[] { info };
        }

        /// <summary> 
        /// Constructor.
        /// </summary> 
        /// <param name="ln">Logican Name of the object.</param>
        public GXDLMSImageTransfer(string ln)
            : base(ObjectType.ImageTransfer, ln, 0)
        {
            ImageBlockSize = 200;
            ImageFirstNotTransferredBlockNumber = 0;
            ImageTransferEnabled = false;
            GXDLMSImageActivateInfo info = new GXDLMSImageActivateInfo();
            info.Size = 0;
            info.Signature = "";
            info.Identification = "";
            ImageActivateInfo = new GXDLMSImageActivateInfo[] { info };
        }

        /// <summary> 
        /// Constructor.
        /// </summary> 
        /// <param name="ln">Logican Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSImageTransfer(string ln, ushort sn)
            : base(ObjectType.ImageTransfer, ln, 0)
        {
            ImageBlockSize = 200;
            ImageFirstNotTransferredBlockNumber = 0;
            ImageTransferEnabled = false;
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
            List<byte> data = new List<byte>();
            data.Add((byte)DataType.Structure);
            data.Add((byte)2);
            GXCommon.SetData(data, DataType.OctetString, ASCIIEncoding.ASCII.GetBytes(imageIdentifier));
            GXCommon.SetData(data, DataType.UInt32, imageSize);
            return client.Method(this, 1, data.ToArray(), DataType.Array);
        }

        public byte[][] ImageBlockTransfer(GXDLMSClient client, byte[] imageBlockValue)
        {
            int cnt = (int)(imageBlockValue.Length / ImageBlockSize);
            if (imageBlockValue.Length % ImageBlockSize != 0)
            {
                ++cnt;
            }
            List<byte[]> packets = new List<byte[]>();
            for (int pos = 0; pos != cnt; ++pos)
            {
                List<byte> data = new List<byte>();
                data.Add((byte)DataType.Structure);
                data.Add((byte)2);
                GXCommon.SetData(data, DataType.UInt32, pos);
                byte[] tmp;
                int bytes = (int)(imageBlockValue.Length - ((pos + 1) * ImageBlockSize));
                //If last packet
                if (bytes < 0)
                {
                    bytes = (int)(imageBlockValue.Length - (pos * ImageBlockSize));
                    tmp = new byte[bytes];
                    Array.Copy(imageBlockValue, pos * ImageBlockSize, tmp, 0, bytes);
                }
                else
                {
                    tmp = new byte[ImageBlockSize];
                    Array.Copy(imageBlockValue, (pos * ImageBlockSize), tmp, 0, ImageBlockSize);
                }
                GXCommon.SetData(data, DataType.OctetString, tmp);
                packets.AddRange(client.Method(this, 2, data.ToArray(), DataType.Array));
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
            ImageFirstNotTransferredBlockNumber, ImageTransferEnabled, ImageTransferStatus, ImageActivateInfo};
        }

        #region IGXDLMSBase Members

        /// <summary>
        /// Data interface do not have any methods.
        /// </summary>
        /// <param name="index"></param>
        byte[] IGXDLMSBase.Invoke(object sender, int index, Object parameters)
        {
            ImageTransferStatus = ImageTransferStatus.NotInitiated;
            GXDLMSServerBase s = sender as GXDLMSServerBase;
            //Image transfer initiate
            if (index == 1)
            {
                ImageFirstNotTransferredBlockNumber = 0;
                ImageTransferredBlocksStatus = "";
                object[] value = (object[]) parameters;
                ImageIdentifier = ASCIIEncoding.ASCII.GetString((byte[]) value[0]);
                ImageSize = (UInt32)value[1];
                ImageTransferStatus = ImageTransferStatus.TransferInitiated;

                List<GXDLMSImageActivateInfo> list = new List<GXDLMSImageActivateInfo>(ImageActivateInfo);
                GXDLMSImageActivateInfo item = new GXDLMSImageActivateInfo();
                item.Size = ImageSize;
                item.Identification = ImageIdentifier;
                list.Add(item);
                ImageActivateInfo = list.ToArray();
                StringBuilder sb = new StringBuilder();
                for (uint pos = 0; pos < ImageSize; ++pos)
                {
                    sb.Append('0');                    
                }
                ImageTransferredBlocksStatus = sb.ToString();
                return s.Acknowledge(Command.MethodResponse, 0);
            }
            //Image block transfer
            else if (index == 2)
            {                
                object[] value = (object[])parameters;
                uint imageIndex = (uint)value[0];
                char[] tmp = ImageTransferredBlocksStatus.ToCharArray();
                tmp[(int)imageIndex] = '1';
                ImageTransferredBlocksStatus = new string(tmp);
                ImageFirstNotTransferredBlockNumber = imageIndex + 1;
                ImageData[imageIndex] = (byte[])value[1];
                ImageTransferStatus = ImageTransferStatus.TransferInitiated;
                return s.Acknowledge(Command.MethodResponse, 0);
            }
            //Image verify
            else if (index == 3)
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
                return s.Acknowledge(Command.MethodResponse, 0);
            }
            //Image activate.
            else if (index == 4)
            {
                ImageTransferStatus = ImageTransferStatus.ActivationSuccessful;
                return s.Acknowledge(Command.MethodResponse, 0);
            }
            else
            {
                throw new ArgumentException("Invoke failed. Invalid attribute index.");
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

        int IGXDLMSBase.GetAttributeCount()
        {
            return 7;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 4;
        }

        override public DataType GetDataType(int index)
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

        object IGXDLMSBase.GetValue(int index, int selector, object parameters)
        {
            if (index == 1)
            {                
                return GXDLMSObject.GetLogicalName(this.LogicalName);
            }
            if (index == 2)
            {
                return ImageBlockSize;
            }
            if (index == 3)
            {
                return ImageTransferredBlocksStatus;
            }
            if (index == 4)
            {
                return ImageFirstNotTransferredBlockNumber;
            }
            if (index == 5)
            {
                return ImageTransferEnabled;
            }
            if (index == 6)
            {
                return ImageTransferStatus;
            }
            if (index == 7)
            {
                List<byte> data = new List<byte>();
                data.Add((byte) DataType.Array);
                if (ImageActivateInfo == null)
                {
                    GXCommon.SetObjectCount(0, data);
                }
                else
                {
                    GXCommon.SetObjectCount(ImageActivateInfo.Length, data);
                    foreach (GXDLMSImageActivateInfo it in ImageActivateInfo)
                    {
                        data.Add((byte)DataType.Structure);
                        data.Add(3);
                        GXCommon.SetData(data, DataType.UInt32, it.Size);
                        GXCommon.SetData(data, DataType.OctetString, ASCIIEncoding.ASCII.GetBytes(Convert.ToString(it.Identification)));
                        if (it.Signature == null || it.Signature.Length == 0)
                        {
                            GXCommon.SetData(data, DataType.OctetString, "");
                        }
                        else
                        {
                            GXCommon.SetData(data, DataType.OctetString, ASCIIEncoding.ASCII.GetBytes(it.Signature));
                        }
                    }
                }
                return data.ToArray();
            }
            throw new ArgumentException("GetValue failed. Invalid attribute index.");
        }

        void IGXDLMSBase.SetValue(int index, object value, bool raw)
        {
            if (index == 1)
            {
                if (value is string)
                {
                    LogicalName = value.ToString();
                }
                else
                {
                    LogicalName = GXDLMSClient.ChangeType((byte[])value, DataType.OctetString).ToString();
                }                
            }
            else if (index == 2)
            {
                ImageBlockSize = Convert.ToUInt32(value);                
            }
            else if (index == 3)
            {
                ImageTransferredBlocksStatus = (string)value;
            }
            else if (index == 4)
            {
                ImageFirstNotTransferredBlockNumber = Convert.ToUInt32(value);
            }
            else if (index == 5)
            {
                ImageTransferEnabled = Convert.ToBoolean(value);
            }
            else if (index == 6)
            {
                ImageTransferStatus = (ImageTransferStatus)Convert.ToUInt32(value);                
            }
            else if (index == 7)
            {
                ImageActivateInfo = null;
                List<GXDLMSImageActivateInfo> list = new List<GXDLMSImageActivateInfo>();
                if (value != null)
                {                    
                    foreach (Object it in (Object[])value)
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
                throw new ArgumentException("SetValue failed. Invalid attribute index.");
            }
        }
        #endregion
    }
}
