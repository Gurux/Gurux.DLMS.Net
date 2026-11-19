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
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects.Enums;
using Gurux.DLMS.Secure;
using Gurux.Net;
using Gurux.Net.Enums;
using Gurux.Serial;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace Gurux.DLMS.Client.Example.Net
{
    /// <summary>Validated connection, security and readout options for the console client.</summary>
    public class Settings
    {
        /// <summary>Transport selected by the network or serial options; null before selection.</summary>
        public IGXAsyncMedia media;
        /// <summary>Trace verbosity used by the reader; defaults to informational messages.</summary>
        public TraceLevel trace = TraceLevel.Info;
        /// <summary>DLMS client populated with the requested addressing and security options.</summary>
        public GXDLMSSecureClient client = new(true);
        /// <summary>Logical name used for public invocation-counter discovery, or null to disable discovery.</summary>
        public string invocationCounter;
        /// <summary>Logical names and attribute indices selected for explicit reads.</summary>
        public List<KeyValuePair<string, int>> readObjects = [];
        /// <summary>Association cache and XML output path, or null to disable file access.</summary>
        public string outputFile;
        /// <summary>Security setup logical name whose certificates are exported, or null when not requested.</summary>
        public string ExportSecuritySetupLN;
        /// <summary>Security setup logical name used for certificate generation, or null when not requested.</summary>
        public string GenerateSecuritySetupLN;
        /// <summary>Positive transport-open and response timeout in milliseconds; defaults to 5000.</summary>
        public int WaitTime = 5000;
        /// <summary>Delay after opening the transport, in milliseconds. Zero disables the delay.</summary>
        public int StartupDelay = 1000;
        /// <summary>Whether an explicit help option was supplied, distinguishing help from missing connection arguments.</summary>
        public bool HelpRequested;
        /// <summary>Supported single-character command-line options that require a following value.</summary>
        private const string ValueOptions = "hpcsritaPgSCnvoTABDdlFmEVGMKNWwfLbRxOz";
        /// <summary>Parses and validates all command-line options before opening the transport.</summary>
        /// <param name="args">Command-line tokens configuring the invocation.</param>
        /// <param name="settings">Client transport, protocol and operation settings.</param>
        /// <returns>Zero for validated settings, or one after displaying help because arguments are absent or help was requested.</returns>
        /// <exception cref="ArgumentNullException">The arguments or target settings are null.</exception>
        /// <exception cref="ArgumentException">An option is missing, malformed, out of range or inconsistent with the selected transport or security suite.</exception>
        public static int GetParameters(string[] args, Settings settings)
        {
            ArgumentNullException.ThrowIfNull(args);
            ArgumentNullException.ThrowIfNull(settings);
            if (args.Length == 0 || Array.Exists(args, a => a == "--help" || a == "-?" || a == "/?"))
            {
                settings.HelpRequested = args.Length != 0;
                ShowHelp();
                return 1;
            }

            bool serialFramingSpecified = false;
            var suppliedKeys = new HashSet<char>();
            int serverAddress = settings.client.ServerAddress;
            int? logicalServerAddress = null;
            for (int index = 0; index < args.Length; ++index)
            {
                string option = args[index];
                if (option == null || option.Length != 2 || (option[0] != '-' && option[0] != '/') || (!ValueOptions.Contains(option[1]) && option[1] != 'I' && option[1] != 'u'))
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.UnknownOption, option));
                }

                char tag = option[1];
                string value = null;
                if (tag != 'I' && tag != 'u')
                {
                    if (++index >= args.Length || args[index] == null || (args[index].Length == 2 && args[index][0] == '-' && char.IsLetter(args[index][1])))
                    {
                        throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.MissingOptionValue, option));
                    }

                    value = args[index];
                }

                if ("ABbD".Contains(tag))
                {
                    suppliedKeys.Add(tag);
                }

                switch (tag)
                {
                    case 'h':
                        Network(settings, tag).HostName = Required(tag, value);
                        break;
                    case 'p':
                        Network(settings, tag).Port = (int)Number(tag, value, 1, 65535);
                        break;
                    case 'u':
                        Network(settings, tag).Protocol = NetworkType.Udp;
                        break;
                    case 'r':
                        if (!value.Equals("sn", StringComparison.OrdinalIgnoreCase) && !value.Equals("ln", StringComparison.OrdinalIgnoreCase))
                        {
                            throw Invalid(tag, value, Properties.Resources.ExpectedReferencing);
                        }

                        settings.client.UseLogicalNameReferencing = value.Equals("ln", StringComparison.OrdinalIgnoreCase);
                        break;
                    case 't':
                        settings.trace = EnumValue<TraceLevel>(tag, value);
                        break;
                    case 'i':
                        settings.client.InterfaceType = EnumValue<InterfaceType>(tag, value);
                        settings.client.Plc.Reset();
                        break;
                    case 'I':
                        settings.client.AutoIncreaseInvokeID = true;
                        break;
                    case 'P':
                        settings.client.Password = value.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Hex(tag, value) : Encoding.ASCII.GetBytes(value);
                        break;
                    case 'a':
                        settings.client.Authentication = EnumValue<Authentication>(tag, value);
                        break;
                    case 'C':
                        settings.client.Ciphering.Security = EnumValue<Security>(tag, value);
                        break;
                    case 'V':
                        settings.client.Ciphering.SecuritySuite = EnumValue<SecuritySuite>(tag, value);
                        break;
                    case 'K':
                        settings.client.Ciphering.Signing = EnumValue<Signing>(tag, value);
                        settings.client.ProposedConformance |= Conformance.GeneralProtection;
                        break;
                    case 'T':
                        settings.client.Ciphering.SystemTitle = Hex(tag, value, 8);
                        break;
                    case 'M':
                        settings.client.ServerSystemTitle = Hex(tag, value, 8);
                        break;
                    // Validate key lengths after all options, so -V can appear before or after a key.
                    case 'A':
                        settings.client.Ciphering.AuthenticationKey = Hex(tag, value, 16, 32);
                        break;
                    case 'B':
                        settings.client.Ciphering.BlockCipherKey = Hex(tag, value, 16, 32);
                        break;
                    case 'b':
                        settings.client.Ciphering.BroadcastBlockCipherKey = Hex(tag, value, 16, 32);
                        break;
                    case 'D':
                        settings.client.Ciphering.DedicatedKey = Hex(tag, value, 16, 32);
                        break;
                    case 'F':
                        settings.client.Ciphering.InvocationCounter = (uint)Number(tag, value, 0, uint.MaxValue);
                        break;
                    case 'v':
                        settings.invocationCounter = LogicalName(tag, value);
                        break;
                    case 'N':
                        settings.GenerateSecuritySetupLN = LogicalName(tag, value);
                        break;
                    case 'E':
                        settings.ExportSecuritySetupLN = LogicalName(tag, value);
                        break;
                    case 'o':
                        settings.outputFile = Required(tag, value);
                        break;
                    case 'd':
                        settings.client.Standard = EnumValue<Standard>(tag, value);
                        settings.client.UseUtc2NormalTime = settings.client.Standard == Standard.Italy || settings.client.Standard == Standard.India || settings.client.Standard == Standard.SaudiArabia;
                        break;
                    case 'c':
                        settings.client.ClientAddress = (int)Number(tag, value, 1, 65535);
                        break;
                    case 's':
                        int server = (int)Number(tag, value, 0, 0x0FFFFFFF);
                        serverAddress = server;
                        break;
                    case 'l':
                        logicalServerAddress = (int)Number(tag, value, 0, 0x3FFF);
                        break;
                    case 'n':
                        serverAddress = GXDLMSClient.GetServerAddressFromSerialNumber((int)Number(tag, value, 0, int.MaxValue), 1);
                        break;
                    case 'm':
                        settings.client.Plc.MacDestinationAddress = (ushort)Number(tag, value, 0, ushort.MaxValue);
                        break;
                    case 'W':
                        settings.client.GbtWindowSize = (byte)Number(tag, value, 1, 63);
                        break;
                    case 'w':
                        settings.client.HdlcSettings.WindowSizeRX = settings.client.HdlcSettings.WindowSizeTX = (byte)Number(tag, value, 1, 7);
                        break;
                    case 'f':
                        settings.client.HdlcSettings.MaxInfoRX = settings.client.HdlcSettings.MaxInfoTX = (ushort)Number(tag, value, 32, 2030);
                        break;
                    case 'L':
                        settings.client.ManufacturerId = Required(tag, value);
                        break;
                    case 'x':
                        settings.WaitTime = (int)Number(tag, value, 1, int.MaxValue);
                        break;
                    case 'z':
                        settings.StartupDelay = (int)Number(tag, value, 0, int.MaxValue);
                        break;
                    case 'R':
                        settings.client.ServiceClass = EnumValue<ServiceClass>(tag, value);
                        settings.client.Broacast = true;
                        break;
                    case 'O':
                        if (!Enum.TryParse(value, true, out Conformance conformance) || ((uint)conformance & ~Enum.GetValues<Conformance>().Aggregate(0u, (all, item) => all | (uint)item)) != 0)
                        {
                            throw Invalid(tag, value, string.Join(", ", Enum.GetNames<Conformance>()));
                        }

                        settings.client.ProposedConformance = conformance;
                        break;
                    case 'g':
                        foreach (string item in value.Split(';', ','))
                        {
                            string[] parts = item.Split(':');
                            if (parts.Length != 2)
                            {
                                throw Invalid(tag, value, Properties.Resources.ExpectedObjectAttribute);
                            }

                            settings.readObjects.Add(new KeyValuePair<string, int>(LogicalName(tag, parts[0]), (int)Number(tag, parts[1], 1, 127)));
                        }

                        break;
                    case 'G':
                        string[] gateway = value.Split(':', 2);
                        if (gateway.Length != 2 || string.IsNullOrWhiteSpace(gateway[1]))
                        {
                            throw Invalid(tag, value, Properties.Resources.ExpectedGateway);
                        }

                        settings.client.Gateway = new GXDLMSGateway
                        {
                            NetworkId = (byte)Number(tag, gateway[0], 0, 255),
                            PhysicalDeviceAddress = gateway[1].StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Hex(tag, gateway[1]) : Encoding.ASCII.GetBytes(gateway[1])
                        };
                        break;
                    case 'S':
                        if (settings.media is GXNet)
                        {
                            throw Invalid(tag, value, Properties.Resources.ExpectedSingleTransport);
                        }

                        string[] port = value.Split(':');
                        if (port.Length != 1 && port.Length != 3)
                        {
                            throw Invalid(tag, value, Properties.Resources.ExpectedSerial);
                        }

                        var serial = new GXSerial
                        {
                            PortName = Required(tag, port[0]),
                            BaudRate = 9600,
                            DataBits = 8,
                            Parity = Parity.None,
                            StopBits = StopBits.One
                        };
                        settings.media = serial;
                        if (port.Length == 3)
                        {
                            serialFramingSpecified = true;
                            serial.BaudRate = (int)Number(tag, port[1], 1, int.MaxValue);
                            string framing = port[2];
                            if (framing.Length < 3)
                            {
                                throw Invalid(tag, value, Properties.Resources.ExpectedSerial);
                            }

                            serial.DataBits = (int)Number(tag, framing[..1], 5, 8);
                            serial.Parity = EnumValue<Parity>(tag, framing[1..^1]);
                            serial.StopBits = EnumValue<StopBits>(tag, framing[^1..]);
                            if (serial.StopBits == StopBits.None)
                            {
                                throw Invalid(tag, value, Properties.Resources.ExpectedSerial);
                            }
                        }

                        break;
                }
            }

            if (settings.media == null)
            {
                throw new ArgumentException(Properties.Resources.TransportRequired);
            }

            if (settings.media is GXNet net)
            {
                Required('h', net.HostName);
                Number('p', net.Port.ToString(CultureInfo.InvariantCulture), 1, 65535);
                if (settings.client.InterfaceType == InterfaceType.CoAP)
                {
                    net.Protocol = NetworkType.Udp;
                }
            }

            if (settings.media is GXSerial optical && !serialFramingSpecified && settings.client.InterfaceType == InterfaceType.HdlcWithModeE)
            {
                optical.BaudRate = 300;
                optical.DataBits = 7;
                optical.Parity = Parity.Even;
                optical.StopBits = StopBits.One;
            }

            if (logicalServerAddress.HasValue)
            {
                Number('s', serverAddress.ToString(CultureInfo.InvariantCulture), 0, 0x3FFF);
                settings.client.ServerAddress = GXDLMSClient.GetServerAddress(logicalServerAddress.Value, serverAddress);
            }
            else
            {
                settings.client.ServerAddress = serverAddress;
            }

            int maxClient = settings.client.InterfaceType == InterfaceType.WRAPPER ? 65535 : settings.client.InterfaceType == InterfaceType.CoAP ? 255 : 127;
            Number('c', settings.client.ClientAddress.ToString(CultureInfo.InvariantCulture), 1, maxClient);
            int maxServer = settings.client.InterfaceType == InterfaceType.WRAPPER ? 65535 : settings.client.InterfaceType == InterfaceType.CoAP ? 255 : 0x0FFFFFFF;
            Number('s', settings.client.ServerAddress.ToString(CultureInfo.InvariantCulture), 0, maxServer);
            bool needsSymmetricKeys = settings.client.Ciphering.Security != Security.None || settings.client.Authentication == Authentication.HighGMAC;
            int keyLength = settings.client.Ciphering.SecuritySuite == SecuritySuite.Suite2 ? 32 : 16;
            foreach (var key in new[]
            {
                ('A', settings.client.Ciphering.AuthenticationKey),
                ('B', settings.client.Ciphering.BlockCipherKey),
                ('b', settings.client.Ciphering.BroadcastBlockCipherKey),
                ('D', settings.client.Ciphering.DedicatedKey)
            }

            )
            {
                if (key.Item2 != null && (suppliedKeys.Contains(key.Item1) || (needsSymmetricKeys && (key.Item1 == 'A' || key.Item1 == 'B'))) && key.Item2.Length != keyLength)
                {
                    throw Invalid(key.Item1, Properties.Resources.HiddenValue, string.Format(CultureInfo.CurrentCulture, Properties.Resources.ExpectedBytes, keyLength));
                }
            }

            if (settings.GenerateSecuritySetupLN != null && settings.ExportSecuritySetupLN != null)
            {
                throw new ArgumentException(Properties.Resources.ConflictingCertificateOperations);
            }

            return 0;
        }

        /// <summary>Returns the selected network transport, creating it when no transport has been selected.</summary>
        /// <param name="settings">Client transport, protocol and operation settings.</param>
        /// <param name="tag">Short command-line option identifying the value being validated.</param>
        /// <returns>The existing or newly allocated network transport.</returns>
        /// <exception cref="ArgumentException">Serial options have already selected a serial transport.</exception>
        private static GXNet Network(Settings settings, char tag)
        {
            if (settings.media is GXSerial)
            {
                throw Invalid(tag, string.Empty, Properties.Resources.ExpectedSingleTransport);
            }

            return (GXNet)(settings.media ??= new GXNet());
        }

        /// <summary>Creates a localized validation error naming the option, its safe display value and the expected input.</summary>
        /// <param name="tag">Short command-line option identifying the value being validated.</param>
        /// <param name="value">Raw option value to validate; sensitive display values must be redacted before reporting.</param>
        /// <param name="expected">Localized description of the accepted input.</param>
        /// <returns>An argument exception containing the localized validation error.</returns>
        private static ArgumentException Invalid(char tag, string value, string expected) => new(string.Format(CultureInfo.CurrentCulture, Properties.Resources.InvalidOptionValue, "-" + tag, value, expected));
        /// <summary>Rejects blank option values and trims surrounding whitespace.</summary>
        /// <param name="tag">Short command-line option identifying the value being validated.</param>
        /// <param name="value">Raw option value to validate; sensitive display values must be redacted before reporting.</param>
        /// <returns>The nonempty value with surrounding whitespace removed.</returns>
        /// <exception cref="ArgumentException">The value does not meet the documented input requirements.</exception>
        private static string Required(char tag, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw Invalid(tag, value, Properties.Resources.ExpectedNonempty);
            }

            return value.Trim();
        }

        /// <summary>Parses an invariant decimal integer and checks an inclusive range.</summary>
        /// <param name="tag">Short command-line option identifying the value being validated.</param>
        /// <param name="value">Raw option value to validate; sensitive display values must be redacted before reporting.</param>
        /// <param name="minimum">Smallest accepted integer, inclusive.</param>
        /// <param name="maximum">Largest accepted integer, inclusive.</param>
        /// <returns>The parsed integer within the inclusive limits.</returns>
        /// <exception cref="ArgumentException">The value does not meet the documented input requirements.</exception>
        private static long Number(char tag, string value, long minimum, long maximum)
        {
            if (!long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out long number) || number < minimum || number > maximum)
            {
                throw Invalid(tag, value, string.Format(CultureInfo.CurrentCulture, Properties.Resources.ExpectedIntegerRange, minimum, maximum));
            }

            return number;
        }

        /// <summary>Parses an enum value without case sensitivity and rejects undefined numeric values.</summary>
        /// <param name="tag">Short command-line option identifying the value being validated.</param>
        /// <param name="value">Raw option value to validate; sensitive display values must be redacted before reporting.</param>
        /// <typeparam name="T">Enum type whose defined values are accepted.</typeparam>
        /// <returns>The defined enum value represented by the input.</returns>
        /// <exception cref="ArgumentException">The value does not meet the documented input requirements.</exception>
        private static T EnumValue<T>(char tag, string value)
            where T : struct, Enum
        {
            if (!Enum.TryParse(value, true, out T result) || !Enum.IsDefined(result))
            {
                throw Invalid(tag, value, string.Join(", ", Enum.GetNames<T>()));
            }

            return result;
        }

        /// <summary>Validates a logical name containing exactly six decimal byte components.</summary>
        /// <param name="tag">Short command-line option identifying the value being validated.</param>
        /// <param name="value">Raw option value to validate; sensitive display values must be redacted before reporting.</param>
        /// <returns>The validated logical name with surrounding whitespace removed.</returns>
        /// <exception cref="ArgumentException">The value does not meet the documented input requirements.</exception>
        private static string LogicalName(char tag, string value)
        {
            string[] parts = value.Trim().Split('.');
            if (parts.Length != 6 || parts.Any(p => !byte.TryParse(p, NumberStyles.None, CultureInfo.InvariantCulture, out _)))
            {
                throw Invalid(tag, value, Properties.Resources.ExpectedLogicalName);
            }

            return value.Trim();
        }

        /// <summary>Decodes hexadecimal input and validates optional byte lengths without exposing key material in errors.</summary>
        /// <param name="tag">Short command-line option identifying the value being validated.</param>
        /// <param name="value">Raw option value to validate; sensitive display values must be redacted before reporting.</param>
        /// <param name="lengths">Accepted byte counts; an empty list accepts any nonempty byte sequence.</param>
        /// <returns>Decoded bytes satisfying the requested length constraints.</returns>
        /// <exception cref="ArgumentException">The value does not meet the documented input requirements.</exception>
        private static byte[] Hex(char tag, string value, params int[] lengths)
        {
            string hex = new([.. value.Where(c => !char.IsWhiteSpace(c))]);
            if (hex.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                hex = hex[2..];
            }

            byte[] bytes;
            try
            {
                bytes = Convert.FromHexString(hex);
            }
            catch (FormatException)
            {
                throw Invalid(tag, Properties.Resources.HiddenValue, Properties.Resources.ExpectedHex);
            }

            if (bytes.Length == 0 || (lengths.Length != 0 && !lengths.Contains(bytes.Length)))
            {
                throw Invalid(tag, Properties.Resources.HiddenValue, lengths.Length == 0 ? Properties.Resources.ExpectedHex : string.Format(CultureInfo.CurrentCulture, Properties.Resources.ExpectedBytes, string.Join(" / ", lengths)));
            }

            return bytes;
        }

        /// <summary>Writes the localized command-line option list and connection examples to standard output.</summary>
        private static void ShowHelp() => Console.WriteLine(Properties.Resources.CommandLineHelp);
    }
}
