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
// This code is licensed under the GNU General Public License v2.
// Full text may be retrieved at http://www.gnu.org/licenses/gpl-2.0.txt
//---------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace Gurux.Common
{
    /// <summary>
    /// Async work is done.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="result">Result of async work.</param>
    public delegate void DoneEventHandler(object sender, object result);

    /// <summary>
    /// Represents the method that will handle the error event of a Gurux component.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="ex">An Exception object that contains the event data.</param>
    public delegate void ErrorEventHandler(object sender, Exception ex);


    /// <summary>
    /// Media component sends received data through this method.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event arguments.</param>
    public delegate void ReceivedEventHandler(object sender, ReceiveEventArgs e);

    /// <summary>
    /// Media component sends notification, when its state changes.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event arguments.</param>
    public delegate void MediaStateChangeEventHandler(object sender, MediaStateEventArgs e);

    /// <summary>
    /// Called when the client is establishing a connection with a GXNet Server.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event arguments.</param>
    public delegate void ClientConnectedEventHandler(object sender, ConnectionEventArgs e);

    /// <summary>
    /// Called when the client has been disconnected from the GXNet server.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event arguments.</param>
    public delegate void ClientDisconnectedEventHandler(object sender, ConnectionEventArgs e);

    /// <summary>
    /// Called when the Media is sending or receiving data.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <seealso cref="IGXMedia.Trace">Trace</seealso>seealso>
    public delegate void TraceEventHandler(object sender, TraceEventArgs e);

    /// <summary>
    /// Common interface for all Media components.<br/>
    /// Using this interface GXCommunication library enables communication with
    /// different medias.
    /// </summary>
    public interface IGXMedia
    {
        /// <inheritdoc cref="ReceivedEventHandler"/>
        event ReceivedEventHandler OnReceived;

        /// <inheritdoc cref="ErrorEventHandler"/>
        event ErrorEventHandler OnError;

        /// <inheritdoc cref="MediaStateChangeEventHandler"/>
        event MediaStateChangeEventHandler OnMediaStateChange;

        /// <inheritdoc cref="ClientConnectedEventHandler"/>
        event ClientConnectedEventHandler OnClientConnected;

        /// <inheritdoc cref="ClientDisconnectedEventHandler"/>
        event ClientDisconnectedEventHandler OnClientDisconnected;

        /// <inheritdoc cref="TraceEventHandler"/>
        event TraceEventHandler OnTrace;

        /// <summary>
        /// Copies the content of the media to target media.
        /// </summary>
        void Copy(object target);

        /// <summary>
        /// Returns name of the media.
        /// </summary>
        /// <remarks>
        /// Media name is used to identify media connection, so two different media connection can not return same media name.
        /// </remarks>
        string Name
        {
            get;
        }

        /// <summary>
        /// Trace level of the IGXMedia for System.Diagnostic.Trace.Writes.
        /// </summary>
        /// <seealso cref="OnTrace"/>
        System.Diagnostics.TraceLevel Trace
        {
            get;
            set;
        }

        /// <summary>
        /// Opens the media.
        /// </summary>
        void Open();

        /// <summary>
        /// Checks if the connection is established.
        /// </summary>
        /// <returns>True, if the connection is established.</returns>
        bool IsOpen
        {
            get;
        }

        /// <summary>
        /// Closes the active connection.
        /// </summary>
        /// <seealso cref="Open">Open</seealso>
        void Close();

#if !__MOBILE__
    /// <summary>
    /// Shows the media Properties dialog.
    /// </summary>
    /// <param name="parent">Parent window.</param>
    /// <returns>Returns True if user has accect changes. Otherwice false.</returns>
    bool Properties(System.Windows.Forms.Form parent);

    /// <summary>
    /// Returns Media debended Properties form.
    /// </summary>
    System.Windows.Forms.Form PropertiesForm
    {
        get;
    }
#endif


        /// <summary>
        /// Sends data asynchronously. <br/>
        /// No reply from the receiver, whether or not the operation was successful, is expected.
        /// </summary>
        /// <param name="data">Data to send to the device.</param>
        /// <param name="receiver">Media depend information of the receiver (optional).</param>
        /// <seealso cref="Receive">Receive</seealso>
        void Send(object data, string receiver);

        /// <summary>
        /// Returns media type as a string.
        /// </summary>
        string MediaType
        {
            get;
        }

        /// <summary>
        /// Return is media enabled.
        /// </summary>
        /// <remarks>
        /// Media is disabled example if there is no serial port in the machine.
        /// </remarks>
        bool Enabled
        {
            get;
        }

        /// <summary>
        /// Media settings as a XML string.
        /// </summary>
        string Settings
        {
            get;
            set;
        }

        /// <summary>
        /// Locking this property makes the connection synchronized and stops sending OnReceived events.
        /// </summary>
        object Synchronous
        {
            get;
        }

        /// <summary>
        /// Checks if the connection is in synchronous mode.
        /// </summary>
        /// <returns>True, if the connection is in synchronous mode.</returns>
        bool IsSynchronous
        {
            get;
        }

        /// <summary>
        /// Waits for more reply data After SendSync if whole packet is not received yet.
        /// </summary>
        /// <param name="args">Receive data arguments.</param>
        /// <returns>True, if the send operation was successful.</returns>
        /// <seealso cref="Send">SendSync</seealso>
        /// <seealso cref="Synchronous">Synchronous</seealso>
        bool Receive<T>(ReceiveParameters<T> args);

        /// <summary>
        /// Resets synchronous buffer.
        /// </summary>
        void ResetSynchronousBuffer();

        /// <summary>
        /// Sent byte count.
        /// </summary>
        /// <seealso cref="BytesReceived">BytesReceived</seealso>
        /// <seealso cref="ResetByteCounters">ResetByteCounters</seealso>
        UInt64 BytesSent
        {
            get;
        }

        /// <summary>
        /// Received byte count.
        /// </summary>
        /// <seealso cref="BytesSent">BytesSent</seealso>
        /// <seealso cref="ResetByteCounters">ResetByteCounters</seealso>
        UInt64 BytesReceived
        {
            get;
        }

        /// <summary>
        /// Resets BytesReceived and BytesSent counters.
        /// </summary>
        /// <seealso cref="BytesSent">BytesSent</seealso>
        /// <seealso cref="BytesReceived">BytesReceived</seealso>
        void ResetByteCounters();

        /// <summary>
        /// Validate Media settings for connection open. Returns table of media properties that must be set before media is valid to open. Return NULL if media is capable to open with these settings.
        /// </summary>
        /// <returns>List of media properties that must set before connection can be established.</returns>
        void Validate();

        /// <summary>
        /// Data is buffered until EOP is received.
        /// </summary>
        object Eop
        {
            get;
            set;
        }

        /// <summary>
        /// Visible controls on the properties dialog.
        /// </summary>
        int ConfigurableSettings
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an object that contains data to associate with the item.
        /// </summary>
        object Tag
        {
            get;
            set;
        }

        /// <summary>
        /// Parent media container if Gateway is used.
        /// </summary>
        IGXMediaContainer MediaContainer
        {
            get;
            set;
        }

        /// <summary>
        /// Gets an object that can be used to synchronize the media.
        /// </summary>
        [Browsable(false), ReadOnly(true)]
        object SyncRoot
        {
            get;
        }
    }

    /// <summary>
    /// Media container is used to create Gateway Media.
    /// </summary>
    public interface IGXMediaContainer
    {
        /// <summary>
        /// Child media.
        /// </summary>
        IGXMedia Media
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Get value of virtual property.
    /// </summary>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public delegate string GetPropertyValueEventHandler(string propertyName);

    /// <summary>
    /// Virtual media is used to create virtual media.
    /// </summary>
    public interface IGXVirtualMedia
    {
        /// <summary>
        /// Is media virtual.
        /// </summary>
        /// <remarks>
        /// If media is virtual actual data is not send only notifications.
        /// </remarks>
        bool Virtual
        {
            get;
            set;
        }

        /// <summary>
        /// Occurs when a property value is asked.
        /// </summary>
        event GetPropertyValueEventHandler OnGetPropertyValue;

        /// <summary>
        /// Notifies that new data is received.
        /// </summary>
        event ReceivedEventHandler OnDataSend;

        /// <summary>
        /// Called when new data is received to the virtual media.
        /// </summary>
        /// <param name="data">Received data.</param>
        /// <param name="sender">Data sender.</param>
        void DataReceived(byte[] data, string sender);

    }
}
