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

namespace Gurux.DLMS
{
    /// <summary>
    /// Long get or set information is saved here.
    /// </summary>
    class GXDLMSLongTransaction
    {
        /// <summary>
        /// Executed command.
        /// </summary>
        public Command command;

        /// <summary>
        /// Targets.
        /// </summary>
        public ValueEventArgs[] targets;

        /// <summary>
        /// Extra data from PDU.
        /// </summary>
        public GXByteBuffer data;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="forTarget"></param>
        /// <param name="forCommand"></param>
        /// <param name="forData"></param>
        public GXDLMSLongTransaction(ValueEventArgs[] forTargets, Command forCommand, GXByteBuffer forData)
        {
            targets = forTargets;
            command = forCommand;
            data = new GXByteBuffer();
            data.Set(forData.Data, forData.Position, forData.Size - forData.Position);
        }
    }
}
