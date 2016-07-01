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
using Gurux.DLMS.Objects;
using Gurux.DLMS;
using Gurux.Net;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects.Enums;
using Gurux.DLMS.Secure;
using System.Diagnostics;

namespace GuruxDLMSServerExample
{
    /// <summary>
    /// All example servers are using same objects.
    /// </summary>
    class GXDLMSPushListener : GXDLMSNotify
    {
        /// <summary>
        /// Are messages traced.
        /// </summary>
        private bool trace = false;


        /// <summary>
        ///  TCP/IP port to listen.
        /// </summary>
        private GXNet media;
        /// <summary>
        /// Received data is saved to reply buffer because whole message is not always received in one packet.
        /// </summary>
        private GXByteBuffer reply = new GXByteBuffer();

        /// <summary>
        /// Received data. This is used if GBT is used and data is received on several data blocks.
        /// </summary>
        private GXReplyData data = new GXReplyData();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="port">Port to listen.</param>
        public GXDLMSPushListener(int port)
            : base(true, 1, 1, InterfaceType.WRAPPER)
        {
            // TODO: Must set communication specific settings.
            media = new GXNet(NetworkType.Tcp, port);
            media.Trace = TraceLevel.Verbose;
            media.OnReceived += new Gurux.Common.ReceivedEventHandler(OnReceived);
            media.OnClientConnected += new Gurux.Common.ClientConnectedEventHandler(OnClientConnected);
            media.OnClientDisconnected += new Gurux.Common.ClientDisconnectedEventHandler(OnClientDisconnected);
            media.OnError += new Gurux.Common.ErrorEventHandler(OnError);
            media.Open();
        }

        void OnError(object sender, Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }

        /// <summary>
        /// Client has close connection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnClientDisconnected(object sender, Gurux.Common.ConnectionEventArgs e)
        {
            Console.WriteLine("Client Disconnected.");
        }

        /// <summary>
        /// Client has made connection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnClientConnected(object sender, Gurux.Common.ConnectionEventArgs e)
        {
            Console.WriteLine("Client Connected.");
        }

        /// <summary>
        /// Client has send data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnReceived(object sender, Gurux.Common.ReceiveEventArgs e)
        {
            try
            {
                lock (this)
                {
                    if (trace)
                    {
                        Console.WriteLine("<- " + Gurux.Common.GXCommon.ToHex((byte[])e.Data, true));
                    }
                    reply.Set((byte[])e.Data);
                    GetData(reply, data);
                    // If all data is received.
                    if (data.IsComplete && !data.IsMoreData)
                    {
                        try
                        {
                            List<KeyValuePair<GXDLMSObject, int>> list;
                            list = ParsePush((Object[])data.Value);
                            // Print received data.
                            foreach (KeyValuePair<GXDLMSObject, int> it in list)
                            {
                                // Print LN.
                                Console.WriteLine(it.Key.ToString());
                                // Print Value.
                                Console.WriteLine(it.Key.GetValues()[it.Value - 1]);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        finally
                        {
                            data.Clear();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
