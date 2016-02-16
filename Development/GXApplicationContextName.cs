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
using System.Linq;
using System.Text;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Secure;
using Gurux.DLMS.Enums;

namespace Gurux.DLMS
{
    /// <summary>
    /// AARQ PDUs
    /// </summary>
    enum GXAarqApdu
    {
        /// <summary>
        /// IMPLICIT BIT STRING {version1 (0)} DEFAULT {version1}
        /// </summary>
        ProtocolVersion = 0,
        /// <summary>
        /// Application-context-name
        /// </summary>
        ApplicationContextName,
        /// <summary>
        /// AP-title OPTIONAL
        /// </summary>
        CalledApTitle,
        /// <summary>
        /// AE-qualifier OPTIONAL.
        /// </summary>
        CalledAeQualifier,
        /// <summary>
        /// AP-invocation-identifier OPTIONAL.
        /// </summary>
        CalledApInvocationId,
        /// <summary>
        /// AE-invocation-identifier OPTIONAL
        /// </summary>
        CalledAeInvocationId,
        /// <summary>
        /// AP-title OPTIONAL
        /// </summary>
        CallingApTitle,
        /// <summary>
        /// AE-qualifier OPTIONAL
        /// </summary>
        CallingAeQualifier,
        /// <summary>
        /// AP-invocation-identifier OPTIONAL
        /// </summary>
        CallingApInvocationId,
        /// <summary>
        /// AE-invocation-identifier OPTIONAL
        /// </summary>
        CallingAeInvocationId,
        /// <summary>
        /// The following field shall not be present if only the kernel is used.
        /// </summary>
        SenderAcseRequirements,
        /// <summary>
        /// The following field shall only be present if the authentication functional unit is selected.     
        /// </summary>
        MechanismName = 11,
        /// <summary>
        /// The following field shall only be present if the authentication functional unit is selected.
        /// </summary>
        CallingAuthenticationValue = 12,
        /// <summary>
        /// Implementation-data.
        /// </summary>
        ImplementationInformation = 29,
        /// <summary>
        /// Association-information OPTIONAL 
        /// </summary>
        UserInformation = 30
    }

    /// <summary>
    /// Reserved for internal use.
    /// </summary>
    class GXApplicationContextName
    {
        /// <summary>
        /// Code application context name.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="data">Byte buffer where data is saved.</param>
        /// <param name="cipher">Is ciphering settings.</param>
        internal static void CodeData(GXDLMSSettings settings, GXByteBuffer data, GXICipher cipher)
        {
            //Application context name tag
            data.SetUInt8(((byte)GXBer.ContextClass | (byte)GXBer.Constructed | (byte)GXAarqApdu.ApplicationContextName));           
            //Len
            data.SetUInt8(0x09);
            data.SetUInt8(GXBer.ObjectIdentifierTag);
            //Len
            data.SetUInt8(0x07);
            bool ciphered = cipher != null && cipher.IsCiphered();
            if (settings.UseLogicalNameReferencing)
            {
                if (ciphered)
                {
                    data.Set(GXCommon.LogicalNameObjectIdWithCiphering);
                }
                else
                {
                    data.Set(GXCommon.LogicalNameObjectID);
                }
            }
            else
            {
                if (ciphered)
                {
                    data.Set(GXCommon.ShortNameObjectIdWithCiphering);
                }
                else
                {
                    data.Set(GXCommon.ShortNameObjectID);
                }
            }
            //Add system title.
            if (!settings.IsServer && ciphered)
            {
                if (cipher.SystemTitle == null || cipher.SystemTitle.Length == 0)
                {
                    throw new ArgumentNullException("SystemTitle");
                }
               //Add calling-AP-title 
                data.SetUInt8(((byte)GXBer.ContextClass | (byte)GXBer.Constructed | 6));
                //LEN
                data.SetUInt8((byte) (2 + cipher.SystemTitle.Length));
                data.SetUInt8((byte) GXBer.OctetStringTag);
                //LEN
                data.SetUInt8((byte) cipher.SystemTitle.Length);
                data.Set(cipher.SystemTitle);
            }
        }

        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="buff"></param>
        internal static bool EncodeData(GXDLMSSettings settings, GXByteBuffer buff)
        {
            //Get length.
            int len = buff.GetUInt8();
            if (buff.Size - buff.Position < len)
            {
                throw new Exception("Encoding failed. Not enough data.");
            }
            if (buff.GetUInt8() != 0x6)
            {
                throw new Exception("Encoding failed. Not an Object ID.");
            }
            //Object ID length.
            len = buff.GetUInt8();
            if (settings.UseLogicalNameReferencing)
            {
                if (buff.Compare(GXCommon.LogicalNameObjectID))
                {
                    return true;
                }
                // If ciphering is used.
                return buff.Compare(GXCommon.LogicalNameObjectIdWithCiphering);
            }
            if (buff.Compare(GXCommon.ShortNameObjectID))
            {
                return true;
            }
            // If ciphering is used.
            return buff.Compare(GXCommon.ShortNameObjectIdWithCiphering);
        }
    }
}
