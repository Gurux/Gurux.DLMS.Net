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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using System.Xml;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects;
using Gurux.DLMS.Compression;
using Gurux.DLMS.Compression.Enums;
using Gurux.Serial;
using Gurux.Net;
using Gurux.DLMS.DeviceAccess;
using Gurux.Common;

namespace Gurux.DLMS.Client.Example.Net
{
    /// <summary>Runs the console client and owns cancellation, diagnostics and connection lifetime.</summary>
    partial class Program
    {
        /// <summary>Registers Ctrl+C cancellation, runs the client and optionally pauses once under the debugger.</summary>
        /// <param name="args">Command-line tokens configuring the invocation.</param>
        /// <returns>A task returning zero on success or explicit help, one on failure, or 130 on cancellation.</returns>
        static async Task<int> Main(string[] args)
        {
            using var cancellation = new CancellationTokenSource();
            // Cancel pending work while allowing the normal cleanup path to close the connection.
            void Cancel(object sender, ConsoleCancelEventArgs e)
            {
                e.Cancel = true;
                cancellation.Cancel();
            }
            Console.CancelKeyPress += Cancel;
            try
            {
                int result = await RunAsync(args, cancellation.Token);
                if (Debugger.IsAttached && !Console.IsInputRedirected && !cancellation.IsCancellationRequested)
                {
                    Console.WriteLine(Properties.Resources.EndedPrompt);
                    Console.ReadKey(true);
                }

                return result;
            }
            finally
            {
                Console.CancelKeyPress -= Cancel;
            }
        }

        /// <summary>Runs one client invocation and closes and disposes its transport even after failure or cancellation.</summary>
        /// <param name="args">Command-line tokens configuring the invocation.</param>
        /// <param name="cancellationToken">Token that cancels pending work and communication waits.</param>
        /// <returns>A task returning zero on success or explicit help, one on failure, or 130 on cancellation.</returns>
        internal static async Task<int> RunAsync(string[] args, CancellationToken cancellationToken)
        {
            var settings = new Settings();
            GXDLMSReader reader = null;
            // Resolve the current reader when the key request is raised, after reader construction.
            void OnKeys(object sender, GXCryptoKeyParameter request) => Client_OnKeys(reader, request);
            int result = 0;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                int parsed = Settings.GetParameters(args, settings);
                if (parsed != 0)
                {
                    return settings.HelpRequested ? 0 : parsed;
                }

                var media = new GXCancellationMedia(settings.media, cancellationToken);
                reader = new GXDLMSReader(settings.client, media, new GXDLMSReaderOptions { TraceLevel = settings.trace, WaitTime = settings.WaitTime })
                {
                    OnTrace = Console.WriteLine,
                    OnKeyPathRequested = request => SelectKeyPath(request, cancellationToken)
                };
                reader.OnNotification += Console.WriteLine;
                settings.client.OnKeys += OnKeys;
                ConfigureCallbacks(settings);
                reader.OnModeE = (transport, operation, baudRate, token) => ConfigureModeEAsync(settings, transport, operation, baudRate, token);
                await ReadMeterAsync(settings, reader, media, cancellationToken);
            }
            catch (Exception) when (cancellationToken.IsCancellationRequested)
            {
                Console.Error.WriteLine(Properties.Resources.OperationCanceled);
                result = 130;
            }
            catch (OperationCanceledException ex)
            {
                ReportError(ex);
                result = 130;
            }
            catch (Exception ex)
            {
                ReportError(ex);
                if (settings.media is GXSerial && ex is IOException)
                {
                    Console.Error.WriteLine(string.Format(CultureInfo.CurrentCulture, Properties.Resources.AvailablePorts, string.Join(" ", GXSerial.GetPortNames())));
                }

                result = 1;
            }
            finally
            {
                settings.client.OnKeys -= OnKeys;
                try
                {
                    if (reader != null)
                    {
                        await reader.DisposeAsync();
                    }
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                }
                catch (Exception ex)
                {
                    Warn(string.Format(CultureInfo.CurrentCulture, Properties.Resources.CleanupWarning, ex.Message));
                    if (result == 0)
                    {
                        result = 1;
                    }
                }
                finally
                {
                    try
                    {
                        if (settings.media != null)
                        {
                            await settings.media.CloseAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        Warn(string.Format(CultureInfo.CurrentCulture, Properties.Resources.CleanupWarning, ex.Message));
                        if (result == 0)
                        {
                            result = 1;
                        }
                    }
                    finally
                    {
                        try
                        {
                            (settings.media as IDisposable)?.Dispose();
                        }
                        catch (Exception ex)
                        {
                            Warn(string.Format(CultureInfo.CurrentCulture, Properties.Resources.CleanupWarning, ex.Message));
                            if (result == 0)
                            {
                                result = 1;
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>Loads cached definitions, establishes the connection and performs the selected meter operation.</summary>
        /// <param name="settings">Client transport, protocol and operation settings.</param>
        /// <param name="reader">Reader used for the current meter session.</param>
        /// <param name="media">Cancellation-aware transport used to open and communicate with the meter.</param>
        /// <param name="token">Token that cancels pending work and communication waits.</param>
        /// <returns>A task that completes after the requested meter or transport operation.</returns>
        /// <exception cref="OperationCanceledException">Cancellation interrupts the meter operation.</exception>
        private static async Task ReadMeterAsync(Settings settings, GXDLMSReader reader, GXCancellationMedia media, CancellationToken token)
        {
            if (settings.media is GXNet net && settings.client.InterfaceType == InterfaceType.CoAP)
            {
                settings.client.Coap.Token = 0x45;
                settings.client.Coap.Host = net.HostName;
                settings.client.Coap.MessageId = 1;
                settings.client.Coap.Port = (ushort)net.Port;
                settings.client.Coap.Options[65001] = (byte)1;
                settings.client.Coap.Options[65003] = (byte)settings.client.ClientAddress;
                settings.client.Coap.Options[65005] = (byte)settings.client.ServerAddress;
            }

            bool reading = settings.ExportSecuritySetupLN == null && settings.GenerateSecuritySetupLN == null;
            bool cached = reading && TryLoadAssociationCache(settings);
            await ConnectMeterAsync(settings, reader, media, token);
            Console.WriteLine(Properties.Resources.Connected);
            if (settings.ExportSecuritySetupLN != null)
            {
                await reader.ExportMeterCertificatesAsync(new GXDLMSSecuritySetup(settings.ExportSecuritySetupLN), Environment.CurrentDirectory, token);
            }
            else if (settings.GenerateSecuritySetupLN != null)
            {
                await reader.GenerateCertificatesAsync(new GXDLMSSecuritySetup(settings.GenerateSecuritySetupLN), Environment.CurrentDirectory, token);
            }
            else
            {
                if (!cached)
                {
                    await reader.GetAssociationViewAsync(cancellationToken: token);
                }

                if (settings.readObjects.Count != 0)
                {
                    foreach (KeyValuePair<string, int> item in settings.readObjects)
                    {
                        token.ThrowIfCancellationRequested();
                        var target = settings.client.Objects.FindByLN(ObjectType.None, item.Key) ?? throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.MissingReadObject, item.Key));
                        object value = await reader.ReadAsync(target, item.Value, cancellationToken: token);
                        reader.ShowValue(value, item.Value);
                    }
                }
                else
                {
                    // Preserve the library's ReadAll sequence while keeping file diagnostics in the app.
                    if (!cached)
                    {
                        await reader.GetScalersAndUnitsAsync();
                        token.ThrowIfCancellationRequested();
                        await reader.GetProfileGenericColumnsAsync();
                    }

                    token.ThrowIfCancellationRequested();
                    await ReadAttributesAsync(settings, reader, token);
                }

                token.ThrowIfCancellationRequested();
                TrySaveAssociationCache(settings);
            }

            token.ThrowIfCancellationRequested();
        }

        /// <summary>Writes an already localized warning message to standard error using the warning template.</summary>
        /// <param name="message">Localized warning message to write to standard error.</param>
        private static void Warn(string message) => Console.Error.WriteLine(string.Format(CultureInfo.CurrentCulture, Properties.Resources.WarningMessage, message));
        /// <summary>Writes an error to standard error, including diagnostic details only when a debugger is attached.</summary>
        /// <param name="ex">Failure to classify or report.</param>
        private static void ReportError(Exception ex) => Console.Error.WriteLine(Debugger.IsAttached ? ex.ToString() : ex.Message);
        /// <summary>Registers the example PDU, custom object and compression callbacks on the client.</summary>
        /// <param name="settings">Client transport, protocol and operation settings.</param>
        private static void ConfigureCallbacks(Settings settings)
        {
            settings.client.OnPdu += (sender, data) =>
            {
                try
                {
                    /*
                            //Encrypted PDUs are converted to XML.
                            GXDLMSTranslator translator = new GXDLMSTranslator();
                            translator.Comments = true;
                            translator.SecuritySuite = settings.client.Ciphering.SecuritySuite;
                            translator.BlockCipherKey = settings.client.Ciphering.BlockCipherKey;
                            translator.AuthenticationKey = settings.client.Ciphering.AuthenticationKey;
                            string xml = translator.PduToXml(data);
                            Console.WriteLine(xml);
                            */
                }
                catch (Exception ex)
                {
                    ReportError(ex);
                }
            };
            //Create manufacturer spesific custom COSEM object.
            settings.client.OnCustomObject += (type, version) =>
            {
                /*
                    if (type == 6001 && version == 0)
                    {
                        return new ManufacturerSpesificObject();
                    }
                    */
                return null;
            };
            //Enable Compression:
            //settings.client.CompressionOptions.EnableCompression = true;
            settings.client.OnCompression += e =>
            {
                if (e.Operation == CompressionOperation.Compress)
                {
                    // Compress data using GXV44 compression.
                    //  This is just an example, you can use any compression method you like.
                    // GXV44Encoder encoder = new GXV44Encoder();
                    // e.OutputData = encoder.Compress(e.InputData);
                }
                else
                {
                    // Decompress data using GXV44 compression.
                    //  This is just an example, you can use any compression method you like.
                    // GXV44Decoder decoder = new GXV44Decoder();
                    // e.OutputData = decoder.Decompress(e.InputData);
                }
            };
        }
    }
}
