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

using Gurux.DLMS.Enums;

namespace Gurux.DLMS
{
    internal enum TranslatorGeneralTags
    {
        ApplicationContextName = 0xA1,
        NegotiatedQualityOfService = 0xBE00,
        ProposedDlmsVersionNumber = 0xBE01,
        ProposedMaxPduSize = 0xBE02,
        ProposedConformance = 0xBE03,
        VaaName = 0xBE04,
        NegotiatedConformance = 0xBE05,
        NegotiatedDlmsVersionNumber = 0xBE06,
        NegotiatedMaxPduSize = 0xBE07,
        ConformanceBit = 0xBE08,
        ProposedQualityOfService = 0xBE09,
        SenderACSERequirements = 0x8A,
        ResponderACSERequirement = 0x88,
        RespondingMechanismName = 0x89,
        CallingMechanismName = 0x8B,
        CallingAuthentication = 0xAC,
        RespondingAuthentication = 0x80,
        AssociationResult = 0xA2,
        ResultSourceDiagnostic = 0xA3,
        ACSEServiceUser = 0xA301,
        CallingAPTitle = 0xA6,
        RespondingAPTitle = 0xA4,
        ResponseAllowed = 0xA4,
        DedicatedKey = 0xA8,
        CallingAeInvocationId = 0xA9,
        CalledAeInvocationId = 0xA5,
        RespondingAeInvocationId = 0xA7,
        CharString = 0xAA,
        UserInformation = 0xAB,
    }
}
