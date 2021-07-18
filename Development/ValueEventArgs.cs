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
using Gurux.DLMS.Objects;
using Gurux.DLMS.Enums;

namespace Gurux.DLMS
{
    public class ValueEventArgs
    {
        /// <summary>
        /// Target COSEM object.
        /// </summary>
        public GXDLMSObject Target
        {
            get;
            private set;
        }

        /// <summary>
        /// DLMS server.
        /// </summary>
        internal GXDLMSServer Server
        {
            get;
            set;
        }

        /// <summary>
        /// Attribute index of queried object.
        /// </summary>
        public int Index
        {
            get;
            set;
        }

        /// <summary>
        /// object value
        /// </summary>
        public object Value
        {
            get;
            set;
        }

        /// <summary>
        /// Is request handled.
        /// </summary>
        public bool Handled
        {
            get;
            set;
        }

        /// <summary>
        /// Parameterised access selector.
        /// </summary>
        public int Selector
        {
            get;
            internal set;
        }

        /// <summary>
        /// Optional parameters.
        /// </summary>
        public object Parameters
        {
            get;
            internal set;
        }

        /// <summary>
        /// Occurred error.
        /// </summary>
        public ErrorCode Error
        {
            get;
            set;
        }

        /// <summary>
        /// Is user updating the value.
        /// </summary>
        /// <remarks>
        /// This is used example with register object. Scaler is not used if user is updating the value.
        /// </remarks>
        public bool User
        {
            get;
            set;
        }

        /// <summary>
        /// Is action. This is reserved for internal use.
        /// </summary>
        internal bool action;

        /// <summary>
        /// DLMS settings.
        /// </summary>
        public GXDLMSSettings Settings
        {
            get;
            private set;
        }

        /// <summary>
        /// Is value max PDU size skipped when converting data to bytes.
        /// </summary>
        public bool SkipMaxPduSize
        {
            get;
            set;
        }

        /// <summary>
        /// Is reply handled as byte array or octet string.
        /// </summary>
        public bool ByteArray
        {
            get;
            set;
        }


        /// <summary>
        /// Row to PDU is used with Profile Generic to tell how many rows are fit to one PDU.
        /// </summary>
        public UInt16 RowToPdu
        {
            get;
            set;
        }

        /// <summary>
        /// Rows begin index.
        /// </summary>
        public UInt32 RowBeginIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Rows end index.
        /// </summary>
        public UInt32 RowEndIndex
        {
            get;
            set;
        }

        ///<summary>
        /// Received invoke ID.
        ///</summary>
        public UInt32 InvokeId
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        internal ValueEventArgs(GXDLMSServer server, GXDLMSObject target, int index, int selector, object parameters)
        {
            Server = server;
            Settings = server == null ? null : server.Settings;
            Target = target;
            Index = (byte)index;
            Selector = selector;
            Parameters = parameters;
        }


        /// <summary>
        /// Constructor.
        /// </summary>
        internal ValueEventArgs(GXDLMSSettings settings, GXDLMSObject target, int index, int selector, object parameters)
        {
            Settings = settings;
            Target = target;
            Index = (byte)index;
            Selector = selector;
            Parameters = parameters;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ValueEventArgs(GXDLMSObject target, int index, int selector, object parameters)
        {
            Target = target;
            Index = (byte)index;
            Selector = selector;
            Parameters = parameters;
        }
    }
}
