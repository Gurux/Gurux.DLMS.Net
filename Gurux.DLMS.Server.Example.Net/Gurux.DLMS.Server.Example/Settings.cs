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

using Gurux.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GuruxDLMSServerExample
{
    class Settings
    {
        public TraceLevel trace = TraceLevel.Info;
        public int port = 4060;
        public string serial;
        public bool useLogicalNameReferencing = true;

        static public int GetParameters(string[] args, Settings settings)
        {
            List<GXCmdParameter> parameters = GXCommon.GetParameters(args, "t:p:S:r:");
            foreach (GXCmdParameter it in parameters)
            {
                switch (it.Tag)
                {
                    case 't':
                        //Trace.
                        try
                        {
                            settings.trace = (TraceLevel)Enum.Parse(typeof(TraceLevel), it.Value);
                        }
                        catch (Exception)
                        {
                            throw new ArgumentException("Invalid Authentication option. (Error, Warning, Info, Verbose, Off)");
                        }
                        break;
                    case 'p':
                        //Port.
                        settings.port = int.Parse(it.Value);
                        break;
                    case 'S':
                        //serial port.
                        settings.serial = it.Value;
                        break;
                    case 'r':
                        if (string.Compare(it.Value, "sn", true) == 0)
                        {
                            settings.useLogicalNameReferencing = false;
                        }
                        else if (string.Compare(it.Value, "ln", true) == 0)
                        {
                            settings.useLogicalNameReferencing = true;
                        }
                        else
                        {
                            throw new ArgumentException("Invalid reference option.");
                        }
                        break;
                    case '?':
                        switch (it.Tag)
                        {
                            case 'p':
                                throw new ArgumentException("Missing mandatory port option.");
                            case 't':
                                throw new ArgumentException("Missing mandatory trace option.\n");
                            case 'S':
                                throw new ArgumentException("Missing mandatory Serial port option.");
                            case 'r':
                                throw new ArgumentException("Missing mandatory reference option.");
                            default:
                                ShowHelp();
                                return 1;
                        }
                    default:
                        ShowHelp();
                        return 1;
                }
            }
            return 0;
        }

        static void ShowHelp()
        {
            Console.WriteLine("Gurux DLMS example Server implements four DLMS/COSEM devices.");
            Console.WriteLine(" -t [Error, Warning, Info, Verbose] Trace messages.");
            Console.WriteLine(" -p Start port number. Default is 4060.");
            Console.WriteLine(" -S Serial port.");
            Console.WriteLine(" -S Serial port.");
            Console.WriteLine(" -r [sn, sn]\t Short name or Logican Name (default) referencing is used.");
        }
    }
}
