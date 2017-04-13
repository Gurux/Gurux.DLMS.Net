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


namespace Gurux.DLMS.Enums
{
    /// <summary>
    ///  Enumerated DLMS error codes.
    /// </summary>
    public enum ErrorCode
    {
        /// <summary>
        /// Disconnect Mode.
        /// </summary>
        DisconnectMode = -4,
        /// <summary>
        /// Receive Not Ready.
        /// </summary>
        ReceiveNotReady = -3,
        /// <summary>
        /// Connection is rejected.
        /// </summary>
        Rejected = -2,
        /// <summary>
        /// Unacceptable frame.
        /// </summary>
        UnacceptableFrame = -1,
        /// <summary>
        /// No error has occurred.
        /// </summary>
        Ok = 0,
        /// <summary>
        /// Access Error : Device reports a hardware fault.
        /// </summary>
        HardwareFault = 1,

        /// <summary>
        /// Access Error : Device reports a temporary failure.
        /// </summary>
        TemporaryFailure = 2,

        /// <summary>
        /// Access Error : Device reports Read-Write denied.
        /// </summary>
        ReadWriteDenied = 3,

        /// <summary>
        /// Access Error : Device reports a undefined object
        /// </summary>
        UndefinedObject = 4,

        /// <summary>
        /// Access Error : Device reports a inconsistent Class or object
        /// </summary>
        InconsistentClass = 9,

        /// <summary>
        /// Access Error : Device reports a unavailable object
        /// </summary>
        UnavailableObject = 11,

        /// <summary>
        /// Access Error : Device reports a unmatched type
        /// </summary>
        UnmatchedType = 12,

        /// <summary>
        /// Access Error : Device reports scope of access violated
        /// </summary>
        AccessViolated = 13,

        /// <summary>
        /// Access Error : Data Block Unavailable.
        /// </summary>
        DataBlockUnavailable = 14,

        /// <summary>
        /// Access Error : Long Get Or Read Aborted.
        /// </summary>
        LongGetOrReadAborted = 15,

        /// <summary>
        /// Access Error : No Long Get Or Read In Progress.
        /// </summary>
        NoLongGetOrReadInProgress = 16,

        /// <summary>
        /// Access Error : Long Set Or Write Aborted.
        /// </summary>
        LongSetOrWriteAborted = 17,

        /// <summary>
        /// Access Error : No Long Set Or Write In Progress.
        /// </summary>
        NoLongSetOrWriteInProgress = 18,

        /// <summary>
        /// Access Error : Data Block Number Invalid.
        /// </summary>
        DataBlockNumberInvalid = 19,

        /// <summary>
        /// Access Error : Other Reason.
        /// </summary>
        OtherReason = 250
    }
}
