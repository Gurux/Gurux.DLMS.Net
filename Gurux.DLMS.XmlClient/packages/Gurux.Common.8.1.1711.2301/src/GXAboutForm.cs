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
using System.Diagnostics;

namespace Gurux.Common
{
    /// <summary>
    /// This class is used to shown general About dialog.
    /// </summary>
    public class GXAboutForm
    {
        /// <summary>
		/// Application name.
		/// </summary>
        public string Application
		{
			get;
            set;
        }

		/// <summary>
		/// Title of the About dialog.
		/// </summary>
        public string Title
		{
			get;
            set;
		}

		/// <summary>
		/// Copyright text.
		/// </summary>
        public string CopyrightText
		{
			get;
            set;
		}

		/// <summary>
		/// About text.
		/// </summary>
        public string AboutText
		{
			get;
            set;
		}		

		/// <summary>
		/// Show About dialog.
		/// </summary>        
        /// <param name="owner"> An implementation of IWin32Window that will own the modal dialog box.</param>
		/// <param name="version">Version number.</param>
		public void ShowAbout(System.Windows.Forms.IWin32Window owner, string version)
		{
            AboutBox1 dlg = new AboutBox1(Application, Title, CopyrightText, AboutText, version);
            dlg.ShowDialog(owner);
		}
    }
}
