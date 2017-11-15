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

namespace Gurux.Common
{
    /// <summary>
    /// Defines the type of a button on a wizard dialog.
    /// </summary>
    public enum GXWizardButtons
    {
        /// <summary>
        /// All buttons are shown.
        /// </summary>
        All = -1,
        /// <summary>
        /// No buttons is shown.
        /// </summary>
        None = 0,
        /// <summary>
        /// Next button is shown.
        /// </summary>
        Next = 1,
        /// <summary>
        /// Back button is shown.
        /// </summary>
        Back = 2,
        /// <summary>
        /// Finish button is shown.
        /// </summary>
        Finish = 4,
        /// <summary>
        /// Cancel button is shown.
        /// </summary>
        Cancel = 8
    }

    /// <summary>
    /// Property page must implement this interface.
    /// </summary>
    /// <remarks>
    /// Media components that want to show property pages must implement this interface.
    /// </remarks>
    public interface IGXPropertyPage
    {
        /// <summary>
        /// Page becomes visible.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Updates the data to the GXDevice.
        /// </summary>
        void Apply();

        /// <summary>
        /// Has user change values or property page.
        /// </summary>
        bool Dirty
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Interface for Gurux wizard pages.
    /// </summary>
    /// <remarks>
    /// The IGXWizardPage interface defines methods common to all wizard pages.
    /// </remarks>
    public interface IGXWizardPage
    {
        /// <summary>
        /// Returns True, if the page is shown.
        /// </summary>
        /// <remarks>
        /// This property is used to hide wizard pages at run time.
        /// </remarks>
        /// <returns>True if the page is shown.</returns>
        bool IsShown();

        /// <summary>
        /// Validates the input and gets ready to move to next page.
        /// </summary>
        void Next();

        /// <summary>
        /// Resets the input if needed.
        /// </summary>
        void Back();

        /// <summary>
        /// Updates the data to the GXDevice.
        /// </summary>
        void Finish();

        /// <summary>
        /// Closes connections etc.
        /// </summary>
        void Cancel();

        /// <summary>
        /// Initialize page
        /// </summary>
        void Initialize();

        /// <summary>
        /// Returns enabled buttons.
        /// </summary>
        /// <remarks>
        /// By default all buttons are enabled.
        /// </remarks>
        GXWizardButtons EnabledButtons
        {
            get;
        }

        /// <summary>
        /// The title test of the dialog.
        /// </summary>
        string Caption
        {
            get;
        }
        /// <summary>
        /// Gets a string that describes the wizard page object.
        /// </summary>
        string Description
        {
            get;
        }

        /// <summary>
        /// Get/Set target.
        /// </summary>
        /// <remarks>
        /// This is used so user can change target type from the wizard.
        /// </remarks>
        object Target
        {
            get;
            set;
        }
    }
}
