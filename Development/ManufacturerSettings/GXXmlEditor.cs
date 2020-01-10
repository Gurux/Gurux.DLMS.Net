//
// --------------------------------------------------------------------------
//  Gurux Ltd
//
//
//
//
// Version:         $Revision: 9442 $,
//                  $Date: 2017-05-23 15:21:03 +0300 (ti, 23 touko 2017) $
//                  $Author: gurux01 $
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
// More information of Gurux DLMS/COSEM Director: https://www.gurux.org/GXDLMSDirector
//
// This code is licensed under the GNU General Public License v2.
// Full text may be retrieved at http://www.gnu.org/licenses/gpl-2.0.txt
//---------------------------------------------------------------------------
#if !__MOBILE__ && !WINDOWS_UWP && !NETCOREAPP2_0 && !NETCOREAPP2_1 && !NETSTANDARD2_0 && !NETCOREAPP3_0 && !NETCOREAPP3_1

using System;
using System.Windows.Forms;

namespace Gurux.DLMS.ManufacturerSettings
{
    public partial class GXXmlEditor : Form
    {
        public GXXmlEditor(string value)
        {
            InitializeComponent();
            ValueTb.Text = value;
        }

        public string Value
        {
            get;
            private set;
        }

        private void OKBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Value = ValueTb.Text;
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
            }
        }
    }
}
#endif