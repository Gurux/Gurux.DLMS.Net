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

using Gurux.DLMS.DeviceAccess;
using Gurux.DLMS.Objects;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Threading;
using System.Threading.Tasks;

namespace Gurux.DLMS.Client.Example.Net
{
    /// <summary>Runs meter operations while owning console diagnostics and connection lifetime.</summary>
    partial class Program
    {
        /// <summary>Loads complete cached object definitions, preserving current objects and warning when loading fails.</summary>
        /// <param name="settings">Client transport, protocol and operation settings.</param>
        /// <returns>True when a nonempty cache was loaded; false when disabled, empty or recoverably invalid.</returns>
        internal static bool TryLoadAssociationCache(Settings settings)
        {
            if (settings.outputFile == null)
            {
                return false;
            }

            try
            {
                // Load completely before replacing the existing collection.
                var objects = GXDLMSObjectCollection.Load(settings.outputFile);
                if (objects.Count == 0)
                {
                    return false;
                }

                settings.client.Objects.Clear();
                settings.client.Objects.AddRange(objects);
                return true;
            }
            catch (Exception ex)when (IsCacheError(ex))
            {
                Warn(string.Format(CultureInfo.CurrentCulture, Properties.Resources.CacheLoadWarning, settings.outputFile, ex.Message));
                return false;
            }
        }

        /// <summary>Saves association definitions and selected read values, reporting recoverable file errors as warnings.</summary>
        /// <param name="settings">Client transport, protocol and operation settings.</param>
        internal static void TrySaveAssociationCache(Settings settings)
        {
            if (settings.outputFile == null)
            {
                return;
            }

            try
            {
                settings.client.Objects.Save(settings.outputFile, new GXXmlWriterSettings { UseMeterTime = true, IgnoreDefaultValues = false, Values = settings.readObjects.Count != 0 });
            }
            catch (Exception ex)when (IsCacheError(ex))
            {
                Warn(string.Format(CultureInfo.CurrentCulture, Properties.Resources.CacheSaveWarning, settings.outputFile, ex.Message));
            }
        }

        /// <summary>Identifies file, XML and value-conversion errors that allow cache recovery.</summary>
        /// <param name="ex">Failure to classify or report.</param>
        /// <returns>True for recoverable cache errors; otherwise false.</returns>
        private static bool IsCacheError(Exception ex) => ex is IOException || ex is UnauthorizedAccessException || ex is XmlException || ex is InvalidOperationException || ex is ArgumentException || ex is FormatException || ex is OverflowException;
        /// <summary>Reads ordinary attributes with cancellation and diagnostics independent of trace verbosity.</summary>
        /// <param name="settings">Client transport, protocol and operation settings.</param>
        /// <param name="reader">Reader used for the current meter session.</param>
        /// <param name="token">Token that cancels pending work and communication waits.</param>
        /// <returns>A task that completes after the requested meter or transport operation.</returns>
        /// <exception cref="OperationCanceledException">Cancellation interrupts the meter operation.</exception>
        private static async Task ReadAttributesAsync(Settings settings, GXDLMSReader reader, CancellationToken token)
        {
            foreach (GXDLMSObject target in settings.client.Objects)
            {
                token.ThrowIfCancellationRequested();
                // Preserve ReadAll's policy of omitting potentially large profile buffers.
                if (target is GXDLMSProfileGeneric)
                {
                    continue;
                }

                if (target is not IGXDLMSBase obj)
                {
                    continue;
                }

                foreach (int index in obj.GetAttributeIndexToRead(true))
                {
                    token.ThrowIfCancellationRequested();
                    if (!settings.client.CanRead(target, index))
                    {
                        continue;
                    }

                    try
                    {
                        object value = await reader.ReadAsync(target, index, cancellationToken: token);
                        if (settings.trace > TraceLevel.Warning)
                        {
                            reader.ShowValue(value, index);
                        }
                    }
                    catch (Exception ex)when (ex is not OperationCanceledException && !token.IsCancellationRequested)
                    {
                        Warn(string.Format(CultureInfo.CurrentCulture, Properties.Resources.AttributeReadWarning, target.LogicalName, index, ex.Message));
                    }
                }
            }
        }
    }
}
