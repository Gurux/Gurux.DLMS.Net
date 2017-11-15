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
using System.Threading;
using Gurux.Common;
using System.Windows.Forms;

namespace Gurux.Common
{
    /// <summary>
    /// Method to execute.
    /// </summary>
    /// <param name="sender">Work creator.</param>
    /// <param name="work">Work to execute.</param>
    /// <param name="parameters">Additional parameters.</param>
    public delegate void AsyncTransaction(object sender, GXAsyncWork work, object[] parameters);
    
    /// <summary>
    /// Status of work is changed.
    /// </summary>
    /// <param name="work">Work to execute</param>
    /// <param name="sender">Sender Form.</param>
    /// <param name="parameters">Work parameters.</param>   
    /// <param name="state">New state.</param>
    /// <param name="text">Shown text.</param>
    public delegate void AsyncStateChangeEventHandler(object sender, GXAsyncWork work, object[] parameters, AsyncState state, string text);

    /// <summary>
    /// This class is used to start work that requires thread.
    /// </summary>
    public class GXAsyncWork
    {
        ManualResetEvent Done = new ManualResetEvent(false);
        string Text;
        object Sender;
        AsyncTransaction Command;
        object[] Parameters;
        Thread Thread;
        AsyncStateChangeEventHandler OnAsyncStateChangeEventHandler;
        ErrorEventHandler OnError;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="command"></param>
        /// <param name="error"></param>
        /// <param name="text"></param>
        /// <param name="parameters"></param>
        public GXAsyncWork(object sender, AsyncStateChangeEventHandler e, 
            AsyncTransaction command, ErrorEventHandler error, 
            string text, object[] parameters)
        {
            OnError = error;
            Text = text;
            OnAsyncStateChangeEventHandler = e;
            Sender = sender;
            Command = command;
            Parameters = parameters;
        }       

        /// <summary>
        /// Result of async work.
        /// </summary>
        public object Result
        {
            get;
            set;
        }

        void ShowError(object sender, Exception ex)
        {
            System.Windows.Forms.MessageBox.Show(Sender as System.Windows.Forms.Control, ex.Message);
        }               

        void Run()
        {
            System.Windows.Forms.Control tmp = Sender as System.Windows.Forms.Control;
            try
            {
                Command(Sender, this, Parameters);
                if (OnAsyncStateChangeEventHandler != null)
                {
                    if (tmp != null && tmp.InvokeRequired)
                    {
                        tmp.BeginInvoke(OnAsyncStateChangeEventHandler, Sender, this, Parameters, AsyncState.Finish, null);
                    }
                    else
                    {                    
                        OnAsyncStateChangeEventHandler(Sender, this, Parameters, AsyncState.Finish, null);
                    }                    
                }                
            }
            catch (Exception ex)
            {
                if (OnError == null)
                {
                    if (tmp != null && tmp.InvokeRequired)
                    {
                        tmp.Invoke(new ErrorEventHandler(ShowError), Sender, ex);
                    }
                    else
                    {
                        ShowError(Sender, ex);
                    }
                }
                else
                {
                    if (tmp != null && tmp.InvokeRequired)
                    {
                        tmp.Invoke(new ErrorEventHandler(OnError), Sender, ex);
                    }
                    else
                    {
                        OnError(Sender, ex);
                    }
                }
                if (OnAsyncStateChangeEventHandler != null)
                {
                    if (tmp != null && tmp.InvokeRequired)
                    {
                        tmp.BeginInvoke(OnAsyncStateChangeEventHandler, Sender, this, Parameters, AsyncState.Finish, null);
                    }
                    else
                    {
                        OnAsyncStateChangeEventHandler(Sender, this, Parameters, AsyncState.Finish, null);
                    }
                }
            }
            finally
            {
                Done.Set();
            }
        }

        /// <summary>
        /// Is Async work active.
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return Thread != null && Thread.IsAlive;
            }
        }

        /// <summary>
        /// Is work canceled.
        /// </summary>
        public bool IsCanceled
        {
            get;
            private set;
        }

        /// <summary>
        /// Start work.
        /// </summary>
        public void Start()
        {
            lock (this)
            {
                Result = null;
                IsCanceled = false;
                Done.Reset();
                if (OnAsyncStateChangeEventHandler != null)
                {
                    OnAsyncStateChangeEventHandler(Sender, this, Parameters, AsyncState.Start, Text);
                }
                Thread = new Thread(new ThreadStart(Run));
                Thread.IsBackground = true;
                Thread.Start();
            }
        }

        /// <summary>
        /// Cancel work.
        /// </summary>
        public void Cancel()
        {
            try
            {
                if (IsRunning)
                {
                    IsCanceled = true;
                    if (OnAsyncStateChangeEventHandler != null)
                    {
                        OnAsyncStateChangeEventHandler(Sender, this, Parameters, AsyncState.Cancel, null);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Wait until work is done.
        /// </summary>
        /// <param name="wt">Wait time in ms.</param>
        public bool Wait(int wt)
        {
            DateTime start = DateTime.Now;
            while (!Done.WaitOne(100))
            {
                if (wt > 0 && (DateTime.Now - start).TotalMilliseconds > wt)
                {
                    return false;
                }
                System.Windows.Forms.Application.DoEvents();
            }
            return true;
        }
    }
}
