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

using System;
using System.Drawing.Design;
using System.Windows.Forms;

namespace Gurux.DLMS.ManufacturerSettings
{
    /// <summary>
    /// An UITypeEditor used with byte arrays.
    /// </summary>
    public class UIXmlTypeEditor : UITypeEditor
    {
        /// <summary>
        /// Shows editor dlg.
        /// </summary>
        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        /// <summary>
        /// Overrides the method used to provide basic behaviour for selecting editor.
        /// Shows our custom control for editing the value.
        /// </summary>
        /// <param name="context">The context of the editing control</param>
        /// <param name="provider">A valid service provider</param>
        /// <param name="value">The current value of the object to edit</param>
        /// <returns>The new value of the object</returns>
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context == null || context.Instance == null || provider == null)
            {
                return base.EditValue(context, provider, value);
            }
            GXXmlEditor dlg = new GXXmlEditor((string)value);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                value = dlg.Value;
            }
            return value;
        }

    }
}
