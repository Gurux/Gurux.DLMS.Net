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
using Gurux.Net;
using System;
using System.Threading;

namespace Gurux.DLMS.Meter.Listener.Net
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                GXNet server = new GXNet(NetworkType.Tcp, 7777);
                server.Open();
                server.OnClientConnected += OnClientConnected;
                server.OnClientDisconnected += OnClientDisconnected;
                server.OnReceived += OnReceived;
                Console.WriteLine("This server is listening port {0} and waiting incoming connections from the meters.", server.Port);
                Console.WriteLine("This server can be used with DLMS meters that are using dynamic IP addresses.");
                while (Console.ReadKey().Key != ConsoleKey.Enter) ;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        /// <summary>
        /// Server received data. This is newer called because we attach connection on ClientConnected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnReceived(object sender, Common.ReceiveEventArgs e)
        {
            Console.WriteLine("Data from client {0}:", e.SenderInfo, GXCommon.ToHex((byte[])e.Data, true));
        }

        /// <summary>
        /// Media is closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnClientDisconnected(object sender, Common.ConnectionEventArgs e)
        {
            Console.WriteLine("Client {0} is disconnected.", e.Info);
        }

        /// <summary>
        /// New client is connected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnClientConnected(object sender, Common.ConnectionEventArgs e)
        {
            Console.WriteLine("Client {0} is connected.", e.Info);
            GXNet server = (GXNet)sender;
            try
            {
                using (GXNet cl = server.Attach(e.Info))
                {
                    ReadMeter(cl);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Read data from the meter.
        /// </summary>
        private static void ReadMeter(GXNet media)
        {
            GXDLMSReader reader = new Net.GXDLMSReader(media);
            reader.ReadAll();
            //Create own thread for each meter if you are handling multiple meters simultaneously.
            //new Thread(new ThreadStart(reader.ReadAll));
        }
    }
}
