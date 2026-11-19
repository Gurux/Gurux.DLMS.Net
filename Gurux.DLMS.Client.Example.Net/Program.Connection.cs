// --------------------------------------------------------------------------
// Gurux Ltd
//
// Copyright (c) Gurux Ltd.
//
// This file is part of Gurux Device Framework.
//
// Commit: $Format:%H$
// Date:   $Format:%cI$
//
// This software is available under either of the following licenses:
//
// 1. GNU General Public License, version 2 only (GPL-2.0-only).
//    See the LICENSE file included with this distribution.
//
// 2. A commercial license granted by Gurux Ltd under a separate written
//    agreement. Use under this option requires a valid commercial license
//    and is governed by the terms of that agreement.
//
// If you have not obtained a commercial license from Gurux Ltd,
// use of this software is governed by GPL-2.0-only.
//
// Commercial licensing: https://www.gurux.fi/
//
// SPDX-License-Identifier: GPL-2.0-only OR LicenseRef-Gurux-Commercial
// --------------------------------------------------------------------------

using Gurux.Common;
using Gurux.DLMS.DeviceAccess;
using Gurux.DLMS.Enums;
using Gurux.Serial;
using System;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Gurux.DLMS.Client.Example.Net
{
    /// <summary>Runs meter operations while owning console diagnostics and connection lifetime.</summary>
    partial class Program
    {
        /// <summary>Opens the transport with its own timeout, waits for the meter, then initializes DLMS once.</summary>
        /// <param name="settings">Client transport, protocol and operation settings.</param>
        /// <param name="reader">Reader used for the current meter session.</param>
        /// <param name="media">Cancellation-aware transport used to open and communicate with the meter.</param>
        /// <param name="token">Token that cancels pending work and communication waits.</param>
        /// <returns>A task that completes after the requested meter or transport operation.</returns>
        /// <exception cref="TimeoutException">Opening the transport or completing a protocol exchange exceeds the configured timeout.</exception>
        /// <exception cref="OperationCanceledException">Cancellation interrupts opening, settling or the DLMS handshake.</exception>
        private static async Task ConnectMeterAsync(Settings settings, GXDLMSReader reader, IGXAsyncMedia media, CancellationToken token)
        {
            if (settings.client.InterfaceType == InterfaceType.HdlcWithModeE)
            {
                await ConfigureModeEAsync(settings, media, ModeEOperation.InitModeE, 300, token);
            }

            using (var timeout = CancellationTokenSource.CreateLinkedTokenSource(token))
            {
                timeout.CancelAfter(settings.WaitTime);
                try
                {
                    await media.OpenAsync(timeout.Token);
                }
                catch (OperationCanceledException)when (!token.IsCancellationRequested)
                {
                    throw new TimeoutException(Properties.Resources.TransportOpenTimeout);
                }
            }

            // Meter settling time is independent of the transport and response timeouts.
            await Task.Delay(settings.StartupDelay, token);
            await reader.ConnectAsync(new GXDLMSConnectOptions { WaitTime = settings.WaitTime, InvocationCounter = settings.invocationCounter, ReadAssociationView = false }, token);
        }

        /// <summary>Applies host-owned serial changes requested by the Mode E handshake.</summary>
        /// <param name="settings">Client transport, protocol and operation settings.</param>
        /// <param name="transport">Asynchronous transport participating in Mode E negotiation.</param>
        /// <param name="operation">Serial configuration or recovery operation requested by the reader.</param>
        /// <param name="baudRate">Negotiated baud rate for a mode switch; ignored by operations with a fixed rate.</param>
        /// <param name="token">Token that cancels pending work and communication waits.</param>
        /// <returns>A task that completes after the requested meter or transport operation.</returns>
        /// <exception cref="InvalidOperationException">The configured transport is not an asynchronous serial transport.</exception>
        /// <exception cref="OperationCanceledException">The requested Mode E operation is canceled.</exception>
        private static async Task ConfigureModeEAsync(Settings settings, IGXMedia transport, ModeEOperation operation, int baudRate, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            if (settings.media is not GXSerial serial || transport is not IGXAsyncMedia media)
            {
                throw new InvalidOperationException(Properties.Resources.ModeESerialRequired);
            }

            switch (operation)
            {
                case ModeEOperation.InitModeE:
                    serial.BaudRate = 300;
                    serial.DataBits = 7;
                    serial.Parity = Parity.Even;
                    serial.StopBits = StopBits.One;
                    break;
                case ModeEOperation.SwitchModeE:
                    await media.CloseAsync();
                    serial.BaudRate = baudRate;
                    serial.DataBits = 8;
                    serial.Parity = Parity.None;
                    serial.StopBits = StopBits.One;
                    await media.OpenAsync(token);
                    break;
                case ModeEOperation.RecoverModeE:
                    serial.DtrEnable = serial.RtsEnable = false;
                    serial.BaudRate = 9600;
                    serial.DtrEnable = serial.RtsEnable = true;
                    break;
                case ModeEOperation.DisconnectModeE:
                    using (media.BeginSynchronous())
                    {
                        await media.SendAsync(Encoding.ASCII.GetBytes("\u0001B0\u0003\r\n"), cancellationToken: token);
                        await media.ReceiveAsync(new ReceiveParameters<byte[]> { Eop = (byte)'\n', AllData = true, Count = 1, WaitTime = settings.WaitTime }, token);
                    }

                    break;
            }
        }
    }
}
