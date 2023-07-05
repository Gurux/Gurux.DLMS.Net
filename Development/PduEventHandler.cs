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

using Gurux.DLMS.Objects;

namespace Gurux.DLMS
{
    /// <summary>
    /// This event handler is called when PDU is decrypted.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="data">Decrypted PDU.</param>
    public delegate void PduEventHandler(object sender, byte[] data);

    /// <summary>
    /// This event handler is called when meter uses custom object and it's needs to be created.
    /// </summary>
    /// <param name="type">Object type.</param>
    /// <param name="version">Object Version.</param>
    public delegate GXDLMSObject ObjectCreateEventHandler(int type, byte version);

    public class GXCustomPduArgs
    {
        /// <summary>
        /// Received PDU data.
        /// </summary>
        public byte[] Data
        {
            get;
            internal set;
        }

        /// <summary>
        /// Parsed value.
        /// </summary>
        public byte[] Value
        {
            get;
            set;
        }
    }

    /// <summary>
    /// This event handler is called meter uses non DLMS standard PDU.
    /// </summary>
    /// <param name="e">PDU arguments.</param>
    public delegate void CustomPduEventHandler(GXCustomPduArgs e);
}
