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
// More information of Gurux products: https://www.gurux.org
//
// This code is licensed under the GNU General Public License v2.
// Full text may be retrieved at http://www.gnu.org/licenses/gpl-2.0.txt
//---------------------------------------------------------------------------

using Gurux.DLMS.Objects;
using System;
using System.Collections.Generic;
#if !__MOBILE__ && !WINDOWS_UWP && !NETCOREAPP2_0 && !NETCOREAPP2_1 && !NETSTANDARD2_0
using System.Windows.Forms;
#endif// !__MOBILE__ && !WINDOWS_UWP && !NETCOREAPP2_0 && !NETCOREAPP2_1 && !NETSTANDARD2_0

namespace Gurux.DLMS
{
    /// <summary>
    /// DC notifies that meter is added.
    /// </summary>
    /// <param name="sender">Sender DC.</param>
    /// <param name="meter">Added meter(s).</param>
    public delegate void MeterAddEventHandler(object sender, GXDLMSMeter[] meter);

    /// <summary>
    /// DC notifies that meter is removed.
    /// </summary>
    /// <param name="sender">Sender DC.</param>
    /// <param name="meter">Removed meter(s).</param>
    public delegate void MeterRemoveEventHandler(object sender, GXDLMSMeter[] meter);

    /// <summary>
    /// DC notifies that meter is modified.
    /// </summary>
    /// <param name="sender">Sender DC.</param>
    /// <param name="meter">Modified meter(s).</param>
    public delegate void MeterEditEventHandler(object sender, GXDLMSMeter[] meter);

    /// <summary>
    /// User actions
    /// </summary>
    [Flags]
    public enum Actions : int
    {
        All = -1,
        Add = 0x1,
        Edit = 0x2,
        Remove = 0x4
    }

    /// <summary>
    /// User functionality
    /// </summary>
    [Flags]
    public enum Functionality : int
    {
        All = -1,
        None = 0x0,
        DeviceSettings = 0x1,
        MediaSettings = 0x2,
        SecuritySettings = 0x4
    }

    /// <summary>
    /// Action parameter.
    /// </summary>
    public class GXActionParameter
    {
        /// <summary>
        /// Action target.
        /// </summary>
        public GXDLMSObject Target
        {
            get;
            set;
        }

        /// <summary>
        /// Action index.
        /// </summary>
        public int Index
        {
            get;
            set;
        }

        /// <summary>
        /// Action data.
        /// </summary>
        public object Data
        {
            get;
            set;
        }

    }

    public interface IGXDataConcentratorView
    {
        void Initialize();
    }

    /// <summary>
    /// Using data concentrator interface GXDLMSDirector can communicate with the custom DCs.
    /// </summary>
    public interface IGXDataConcentrator
    {
        /// <inheritdoc cref="MeterAddEventHandler"/>
        event MeterAddEventHandler OnMeterAdd;

        /// <inheritdoc cref="MeterRemoveEventHandler"/>
        event MeterRemoveEventHandler OnMeterRemove;

        /// <inheritdoc cref="MeterEditEventHandler"/>
        event MeterEditEventHandler OnMeterEdit;

        /// <summary>
        /// Name of the DC.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// User Actions.
        /// </summary>
        Actions Actions
        {
            get;
        }

        /// <summary>
        /// User functionality.
        /// </summary>
        Functionality Functionality
        {
            get;
        }

        /// <summary>
        /// Get devices from the DC.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>List of devices.</returns>
        GXDLMSMeter[] GetDevices(string name);

        /// <summary>
        /// Add new device(s) to DC.
        /// </summary>
        /// <param name="devices">Devices to add.</param>
        GXDLMSMeter[] AddDevices(GXDLMSMeter[] devices);

#if !__MOBILE__ && !WINDOWS_UWP && !NETCOREAPP2_0 && !NETCOREAPP2_1 && !NETSTANDARD2_0
        /// <summary>
        /// Add a new device.
        /// </summary>
        /// <returns>Returns new device.</returns>
        GXDLMSMeter AddDevice(IWin32Window owner);

        /// <summary>
        /// Edit device.
        /// </summary>
        /// <param name="devices">Device to edit.</param>
        /// <returns>Returns true if device is edit.</returns>
        bool EditDevice(IWin32Window owner, GXDLMSMeter devices);

        /// <summary>
        /// Add target to the schedule.
        /// </summary>
        /// <param name="target">Item to add to the schedule.</param>
        void AddToSchedule(IWin32Window owner, object target);

#endif// !__MOBILE__ && !WINDOWS_UWP && !NETCOREAPP2_0 && !NETCOREAPP2_1 && !NETSTANDARD2_0

        /// <summary>
        /// Remove devices(s) from the DC.
        /// </summary>
        /// <param name="devices">Devices to remove.</param>
        void RemoveDevices(GXDLMSMeter[] devices);

        /// <summary>
        /// Modify selected devices.
        /// </summary>
        /// <param name="devices">Devices to edit.</param>
        void EditDevices(GXDLMSMeter[] devices);

        /// <summary>
        /// Get errors from selected devices.
        /// </summary>
        /// <param name="objects">Devices to search.</param>
        List<KeyValuePair<GXDLMSMeter, string[]>> GetErrors(GXDLMSMeter[] devices);

        /// <summary>
        /// Add new objects(s) to DC.
        /// </summary>
        /// <param name="objects">Objects to add.</param>
        void AddObjects(GXDLMSMeter[] devices, GXDLMSObject[] objects);

        /// <summary>
        /// Remove objects(s) from the DC.
        /// </summary>
        /// <param name="objects">Objects to remove.</param>
        void RemoveObjects(GXDLMSMeter[] devices, GXDLMSObject[] objects);

        /// <summary>
        /// Modify selected objects.
        /// </summary>
        /// <param name="objects">Objects to edit.</param>
        void EditObjects(GXDLMSMeter[] devices, GXDLMSObject[] objects);

        /// <summary>
        /// Read selected objects.
        /// </summary>
        /// <param name="objects">Objects and attribute index to read.</param>
        void ReadObjects(GXDLMSMeter[] devices, List<KeyValuePair<GXDLMSObject, byte>> objects);

        /// <summary>
        /// Write selected objects.
        /// </summary>
        /// <param name="objects">Objects and attribute index to write.</param>
        void WriteObjects(GXDLMSMeter[] devices, List<KeyValuePair<GXDLMSObject, byte>> objects);

        /// <summary>
        /// Call methods of selected objects.
        /// </summary>
        /// <param name="actions">Actions.</param>
        void MethodObjects(GXDLMSMeter[] devices, List<GXActionParameter> actions);

        /// <summary>
        /// Add new device templates to DC.
        /// </summary>
        /// <param name="devices">Devices to add as device template.</param>
        void AddDeviceTemplates(GXDLMSMeter[] devices);

        /// <summary>
        /// Get values for selected objects.
        /// </summary>
        /// <param name="devices">List of devices.</param>
        /// <param name="objects">Objects and attribute index to read.</param>
        /// <param name="readAll">All objects are read from the meter.</param>
        void GetValues(GXDLMSMeter[] devices, List<KeyValuePair<GXDLMSObject, byte>> objects, bool readAll);

        /// <summary>
        /// Get rows by range.
        /// </summary>
        /// <param name="pg">Profile generic object to read.</param>
        /// <param name="start">Start time.</param>
        /// <param name="end">End time.</param>
        void GetRowsByRange(GXDLMSMeter device, GXDLMSProfileGeneric pg, DateTime start, DateTime end);

        /// <summary>
        /// Get rows by entry.
        /// </summary>
        /// <param name="pg">Profile generic object to read.</param>
        /// <param name="index">One based start index.</param>
        /// <param name="count">Rows count to read.</param>
        void GetRowsByEntry(GXDLMSMeter device, GXDLMSProfileGeneric pg, UInt64 index, UInt64 count);

        /// <summary>
        /// Returns custom pages for selected object.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
#if !__MOBILE__ && !WINDOWS_UWP && !NETCOREAPP2_0 && !NETCOREAPP2_1 && !NETSTANDARD2_0
        System.Windows.Forms.Form[] CustomPages(object target, object communication);
#endif// !__MOBILE__ && !WINDOWS_UWP && !NETCOREAPP2_0 && !NETCOREAPP2_1 && !NETSTANDARD2_0
    }
}