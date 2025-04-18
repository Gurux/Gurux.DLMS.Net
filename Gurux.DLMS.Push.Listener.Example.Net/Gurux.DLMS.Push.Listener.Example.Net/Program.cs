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
using Gurux.Net;
using Gurux.DLMS.Objects;
using Gurux.DLMS.Enums;
using System.Threading;
using Gurux.DLMS.Secure;
using Gurux.DLMS;

namespace GuruxDLMSServerExample
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                Settings settings = new Settings();
                ////////////////////////////////////////
                //Handle command line parameters.
                int ret = Settings.GetParameters(args, settings);
                if (ret != 0)
                {
                    return ret;
                }
                GXNet media = new GXNet(
                    settings.interfaceType == InterfaceType.CoAP ? NetworkType.Udp : NetworkType.Tcp,
                    "localhost",
                    settings.port);
                GXDLMSSecureNotify notify = new GXDLMSSecureNotify(true, 16, 1, settings.interfaceType);
                // Un-comment this if you want to send encrypted push messages.
                // notify.Ciphering.Security = Security.AuthenticationEncryption;
                GXDLMSPushSetup p = new GXDLMSPushSetup();
                GXDLMSClock clock = new GXDLMSClock();
                //Un-comment this if you want to describe the content of the push message for the client.
                //                p.PushObjectList.Add(new GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>(p, new GXDLMSCaptureObject(2, 0)));
                p.PushObjectList.Add(new GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>(clock, new GXDLMSCaptureObject(2, 0)));

                ///////////////////////////////////////////////////////////////////////
                //Create Gurux DLMS server component for Short Name and start listen events.
                GXDLMSPushListener pushListener = new GXDLMSPushListener(settings.port, settings.interfaceType);
                Console.WriteLine("Listening DLMS Push " + settings.interfaceType + " messages on port " + settings.port + ".");
                Console.WriteLine("Press X to close and Enter to send a Push message.");
                ConsoleKey key;
                while ((key = Console.ReadKey().Key) != ConsoleKey.X)
                {
                    if (key == ConsoleKey.Enter)
                    {
                        Console.WriteLine("Sending Push message.");
                        media.Open();
                        clock.Time = DateTime.Now;
                        foreach (byte[] it in notify.GeneratePushSetupMessages(DateTime.MinValue, p))
                        {
                            media.Send(it, null);
                        }
                        Thread.Sleep(100);
                        media.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return 0;
        }
    }
}
