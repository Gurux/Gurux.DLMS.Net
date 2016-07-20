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
using Gurux.DLMS;
using Gurux.Net;
using Gurux.DLMS.Objects;

namespace GuruxDLMSServerExample
{
    class Program
    {
        static void Main(string[] args)
        {
            //Create Network media component and start listen events.
            //4059 is Official DLMS port.
            try
            {
                ///////////////////////////////////////////////////////////////////////
                //Create Gurux DLMS server component for Short Name and start listen events.
                GXDLMSServerSN SNServer = new GXDLMSServerSN();
                SNServer.Initialize(4060);
                Console.WriteLine("Short Name DLMS Server in port 4060.");
                ///////////////////////////////////////////////////////////////////////
                //Create Gurux DLMS server component for Short Name and start listen events.
                GXDLMSServerLN LNServer = new GXDLMSServerLN();
                LNServer.Initialize(4061);
                Console.WriteLine("Logical Name DLMS Server in port 4061.");
                ///////////////////////////////////////////////////////////////////////
                //Create Gurux DLMS server component for Short Name and start listen events.
                GXDLMSServerSN_47 SN_47Server = new GXDLMSServerSN_47();
                SN_47Server.Initialize(4062);
                Console.WriteLine("Short Name DLMS Server with IEC 62056-47 in port 4062.");
                ///////////////////////////////////////////////////////////////////////
                //Create Gurux DLMS server component for Short Name and start listen events.
                GXDLMSServerLN_47 LN_47Server = new GXDLMSServerLN_47();
                LN_47Server.Initialize(4063);
                Console.WriteLine("Logical Name DLMS Server with IEC 62056-47 in port 4063.");
                while (Console.ReadKey().Key != ConsoleKey.Enter);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }        
    }
}
