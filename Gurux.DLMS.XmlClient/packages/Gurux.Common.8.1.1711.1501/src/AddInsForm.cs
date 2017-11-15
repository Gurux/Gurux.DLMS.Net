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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using System.Xml;
using System.Runtime.Serialization;
using Gurux.Common.Properties;

namespace Gurux.Common
{
	/// <summary>
	/// This class is used to show a text editor in property grid.
	/// </summary>
    internal class AddInsForm : System.Windows.Forms.Form
    {
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.Button OKBtn;
        private ListView listView1;
        private IContainer components;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem EnableMnu;
        private ColumnHeader NameCH;
        private ToolStripMenuItem DownloadMnu;
        bool OnlyNewItems, ShowAddins;
        private ColumnHeader StateCH;
        private System.ComponentModel.Container m_Components = null;
        GXAddInList AddIns;
        private ColumnHeader InstalledCH;
        private ColumnHeader AvailableCH;
        internal ProtocolUpdateStatus Status = ProtocolUpdateStatus.None;
        /// <summary>
        /// Initializes a new instance of the AddIns form.
        /// </summary>
        public AddInsForm(GXAddInList addIns, bool showAddins, bool onlyNew)
        {
            InitializeComponent();
            //Update resources.
            this.CancelBtn.Text = Resources.CancelTxt;
            this.NameCH.Text = Resources.NameTxt;
            this.StateCH.Text = Resources.State;
            this.InstalledCH.Text = Resources.Installed;
            this.AvailableCH.Text = Resources.Available;
            this.DownloadMnu.Text = Resources.Download;
            this.EnableMnu.Text = Resources.Enable;
            this.Text = Resources.AvailableAddIns;

            ShowAddins = showAddins;
            AddIns = addIns;
            OnlyNewItems = onlyNew;
            if (onlyNew)
            {
                this.Text = Resources.AvailableUpdatesTxt;
            }
            else
            {
                this.Text = Resources.ProtocolsTxt;
            }
            if (addIns.Count == 0)
            {
                listView1.Items.Add(Resources.FindingProtocols);
                OKBtn.Enabled = listView1.Enabled = false;
            }                                   
            foreach (GXAddIn it in addIns)
            {
                //Show only new AddIns.                
                if (onlyNew && (it.State != AddInStates.Available && it.State != AddInStates.Update))
                {
                    continue;
                }
                if (!ShowAddins && it.Type == GXAddInType.AddIn)
                {
                    continue;
                }
                //If installed protocols are shown.
                if (!onlyNew && it.Type == GXAddInType.Application)
                {
                    continue;
                }
                if (it.Type == GXAddInType.Application && string.Compare(it.Name, System.IO.Path.GetFileNameWithoutExtension(Application.ExecutablePath), true) != 0)
                {
                    continue;
                }
                if (onlyNew && !GXUpdateChecker.IsNewVersion(it.Version, it.InstalledVersion))
                {
                    continue;
                }
                ListViewItem li = listView1.Items.Add(it.Name);
                li.SubItems.Add(it.State.ToString());
                li.SubItems.Add(it.InstalledVersion);
                li.SubItems.Add(it.Version);
                li.Tag = it;
                UpdateState(li, it.State);
            }
            ThreadPool.QueueUserWorkItem(CheckUpdates, this);
        }        

        static public void CheckUpdates(object target)
        {
            AddInsForm form = target as AddInsForm;
            GXAddInList list = GXUpdateChecker.GetUpdatesOnline(!form.ShowAddins);
            if (list.Count != 0)
            {                
                if (form.IsHandleCreated)
                {
                    form.BeginInvoke(new CheckUpdatesEventHandler(form.OnCheckUpdatesDone), list);
                }
            }
        }

        public delegate void CheckUpdatesEventHandler(GXAddInList list);

        void OnCheckUpdatesDone(GXAddInList list)
        {
            //If done first time.
            if (!listView1.Enabled)
            {
                listView1.Items.Clear();
                OKBtn.Enabled = listView1.Enabled = true;
                foreach (GXAddIn it in list)
                {
                    //Show only new AddIns.                
                    if (OnlyNewItems && (it.State != AddInStates.Available && it.State != AddInStates.Update))
                    {
                        continue;
                    }
                    if (!ShowAddins && it.Type == GXAddInType.AddIn)
                    {
                        continue;
                    }
                    //If installed protocols are shown.
                    if (!OnlyNewItems && it.Type == GXAddInType.Application)
                    {
                        continue;
                    }
                    if (it.Type == GXAddInType.Application && string.Compare(it.Name, System.IO.Path.GetFileNameWithoutExtension(Application.ExecutablePath), true) != 0)
                    {
                        continue;
                    }
                    if (!GXUpdateChecker.IsNewVersion(it.Version, it.InstalledVersion))
                    {
                        continue;
                    }
                    ListViewItem li = listView1.Items.Add(it.Name);
                    li.SubItems.Add(it.State.ToString());
                    li.SubItems.Add(it.InstalledVersion);
                    li.SubItems.Add(it.Version);
                    li.Tag = it;
                    UpdateState(li, it.State);
                }
            }
            else //Update exist items...
            {
                foreach (GXAddIn it in list)
                {
                    //Show only new AddIns.                
                    if (OnlyNewItems && it.State != AddInStates.Available)
                    {
                        continue;
                    }
                    if (!ShowAddins && it.Type == GXAddInType.AddIn)
                    {
                        continue;
                    }
                    //If installed protocols are shown.
                    if (!OnlyNewItems && it.Type == GXAddInType.Application)
                    {
                        continue;
                    }
                    if (it.Type == GXAddInType.Application && string.Compare(it.Name, System.IO.Path.GetFileNameWithoutExtension(Application.ExecutablePath), true) != 0)
                    {
                        continue;
                    }
                    if (!GXUpdateChecker.IsNewVersion(it.Version, it.InstalledVersion))
                    {
                        continue;
                    }
                    ListViewItem item = null;
                    foreach (ListViewItem li in listView1.Items)
                    {
                        if (string.Compare(li.Text, it.Name, true) == 0)
                        {
                            item = li;
                            break;
                        }
                    }                    
                    if (item == null)
                    {
                        item = listView1.Items.Add(it.Name);
                        item.SubItems.Add(it.InstalledVersion);
                        item.SubItems.Add(it.Version);
                        item.Tag = it;
                    }
                    else
                    {                        
                        GXAddIn t = item.Tag as GXAddIn;
                        t.State = it.State;
                    }
                    UpdateState(item, it.State);
                }
            }
        }        

        static void UpdateState(ListViewItem it, AddInStates state)
        {
            if (state == AddInStates.Available)
            {
                it.SubItems[1].Text = Resources.Available;
            }
            else if (state == AddInStates.Download)
            {
                it.SubItems[1].Text = Resources.Download;
            }
            else if (state == AddInStates.Installed)
            {
                it.SubItems[1].Text = Resources.InUse;
            }
            else if ((state & AddInStates.Disabled) != 0)
            {
                it.SubItems[1].Text = Resources.Disabled;
            }
            else if (state == AddInStates.Update)
            {
                it.SubItems[1].Text = Resources.UpdateAvailable;
            }
        }

        /// <summary> 
        /// Cleans up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_Components != null)
                {
                    m_Components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.OKBtn = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.NameCH = new System.Windows.Forms.ColumnHeader();
            this.StateCH = new System.Windows.Forms.ColumnHeader();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.DownloadMnu = new System.Windows.Forms.ToolStripMenuItem();
            this.EnableMnu = new System.Windows.Forms.ToolStripMenuItem();
            this.InstalledCH = new System.Windows.Forms.ColumnHeader();
            this.AvailableCH = new System.Windows.Forms.ColumnHeader();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // CancelBtn
            // 
            this.CancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.Location = new System.Drawing.Point(291, 237);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(80, 24);
            this.CancelBtn.TabIndex = 1;
            this.CancelBtn.Text = "Cancel";
            // 
            // OKBtn
            // 
            this.OKBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OKBtn.Location = new System.Drawing.Point(195, 237);
            this.OKBtn.Name = "OKBtn";
            this.OKBtn.Size = new System.Drawing.Size(80, 24);
            this.OKBtn.TabIndex = 2;
            this.OKBtn.Text = "OK";
            this.OKBtn.Click += new System.EventHandler(this.OKBtn_Click);
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.NameCH,
            this.StateCH,
            this.InstalledCH,
            this.AvailableCH});
            this.listView1.ContextMenuStrip = this.contextMenuStrip1;
            this.listView1.FullRowSelect = true;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(12, 12);
            this.listView1.Name = "listView1";
            this.listView1.OwnerDraw = true;
            this.listView1.Size = new System.Drawing.Size(359, 219);
            this.listView1.TabIndex = 3;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.listView1_DrawColumnHeader);
            this.listView1.DoubleClick += new System.EventHandler(this.listView1_DoubleClick);
            this.listView1.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.listView1_PreviewKeyDown);
            this.listView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseDown);
            this.listView1.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.listView1_DrawSubItem);
            // 
            // NameCH
            // 
            this.NameCH.Text = "Name";
            this.NameCH.Width = 161;
            // 
            // StateCH
            //             
            // 
            this.StateCH.Text = "State";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DownloadMnu,
            this.EnableMnu});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(129, 48);
            // 
            // DownloadMnu
            // 
            this.DownloadMnu.Name = "DownloadMnu";
            this.DownloadMnu.Size = new System.Drawing.Size(128, 22);
            this.DownloadMnu.Text = "Download";
            this.DownloadMnu.Click += new System.EventHandler(this.DownloadMnu_Click);
            // 
            // EnableMnu
            // 
            this.EnableMnu.Name = "EnableMnu";
            this.EnableMnu.Size = new System.Drawing.Size(128, 22);
            this.EnableMnu.Text = "Enable";
            this.EnableMnu.Click += new System.EventHandler(this.EnableMnu_Click);
            // 
            // InstalledCH
            // 
            this.InstalledCH.Text = "Installed";
            // 
            // AvailableCH
            // 
            this.AvailableCH.Text = "Available";
            // 
            // AddInsForm
            // 
            this.AcceptButton = this.OKBtn;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.CancelBtn;
            this.ClientSize = new System.Drawing.Size(379, 266);
            this.ControlBox = false;
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.OKBtn);
            this.Controls.Add(this.CancelBtn);
            this.Name = "AddInsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AvailableAddIns";
            this.ResizeEnd += new System.EventHandler(this.AddInsForm_ResizeEnd);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private void EnableMnu_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem it in listView1.SelectedItems)
            {
                GXAddIn addIn = it.Tag as GXAddIn;
                addIn.State ^= AddInStates.Disabled;
                UpdateState(it, addIn.State);
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            foreach (ListViewItem it in listView1.SelectedItems)
            {
                GXAddIn addIn = it.Tag as GXAddIn;
                if ((addIn.State & AddInStates.Disabled) != 0)
                {
                    addIn.State ^= AddInStates.Disabled;
                }
                else if (addIn.State == AddInStates.Available)
                {
                    addIn.State = AddInStates.Download;
                }
                else if (addIn.State == AddInStates.Download)
                {
                    addIn.State = AddInStates.Available;
                }
                else if (addIn.State == AddInStates.Installed)
                {
                    addIn.State |= AddInStates.Disabled;
                }
                UpdateState(it, addIn.State);
            }
        }

        private void AddInsForm_ResizeEnd(object sender, EventArgs e)
        {
            StateCH.Width = NameCH.Width = (listView1.Width / 2) - 8;
        }

        private void listView1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                foreach (ListViewItem item in listView1.Items)
                {
                    item.Selected = true;
                }
            }
        }

        private void listView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ListViewItem li = listView1.GetItemAt(e.X, e.Y);
                if (li != null)
                {
                    GXAddIn addIn = li.Tag as GXAddIn;
                    contextMenuStrip1.Enabled = true;
                    if ((addIn.State & AddInStates.Disabled) != 0)
                    {
                        EnableMnu.Text = Resources.Enable;                        
                    }
                    else
                    {
                        EnableMnu.Text = Resources.Disable;
                    }
                    DownloadMnu.Visible = addIn.State == AddInStates.Update || addIn.State == AddInStates.Available || addIn.State == AddInStates.Disabled;
                }
                else
                {
                    contextMenuStrip1.Enabled = false;
                }
            }
        }

        private void DownloadMnu_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem it in listView1.SelectedItems)
            {
                GXAddIn addIn = it.Tag as GXAddIn;
                if (addIn.State == AddInStates.Available ||
                    addIn.State == AddInStates.Update ||
                    addIn.State == AddInStates.Disabled)
                {
                    addIn.State = AddInStates.Download;
                    UpdateState(it, addIn.State);
                }
            }
        }

        private void OKBtn_Click(object sender, EventArgs e)
        {
            try
            {
                bool updatesAvailable = false;
                foreach (ListViewItem it in listView1.Items)
                {
                    GXAddIn addIn = it.Tag as GXAddIn;
                    if (addIn.State == AddInStates.Available || addIn.State == AddInStates.Update)
                    {
                        updatesAvailable = true;                        
                    }
                    //If user has select items to download do not notify.
                    else if (addIn.State == AddInStates.Download)
                    {
                        updatesAvailable = false;
                        break;
                    }
                }
                DialogResult res = DialogResult.Yes;
                if (updatesAvailable)
                {
                    res = MessageBox.Show(this, Resources.NewProtocolsDownloadTxt, Resources.NewProtocolsAvailableTxt, MessageBoxButtons.YesNoCancel);
                    if (res != DialogResult.Yes)
                    {
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                        return;
                    }
                }
                //Add downloadable items to the list or they are not shown anymore.
                foreach (ListViewItem it in listView1.Items)
                {
                    GXAddIn addIn = it.Tag as GXAddIn;
                    if (addIn.State == AddInStates.Available ||
                        addIn.State == AddInStates.Download)
                    {
                        if (addIn.State == AddInStates.Available)
                        {
                            if (res == DialogResult.Yes)
                            {
                                addIn.State = AddInStates.Download;
                            }
                            else if (res == DialogResult.No)
                            {
                                addIn.State = AddInStates.Disabled;
                            }
                        }
                        //Add new Addins.
                        bool exists = false;
                        foreach (var a in AddIns)
                        {
                            if (a.Name == addIn.Name)
                            {
                                exists = true;
                                break;
                            }
                        }
                        if (!exists)
                        {
                            AddIns.Add(addIn);
                        }
                    }
                }                
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.Encoding = System.Text.Encoding.UTF8;
                settings.CloseOutput = true;
                settings.CheckCharacters = false;
                string path = System.IO.Path.Combine(GXCommon.ProtocolAddInsPath, "updates.xml");
                DataContractSerializer x = new DataContractSerializer(typeof(GXAddInList));
                using (XmlWriter writer = XmlWriter.Create(path, settings))
                {
                    x.WriteObject(writer, AddIns);
                    writer.Close();
                }
                Gurux.Common.GXFileSystemSecurity.UpdateFileSecurity(path);
                GXUpdateChecker updater = new GXUpdateChecker();
                updater.OnProgress += new GXUpdateChecker.ProgressEventHandler(updater_OnProgress);
                Status = updater.UpdateProtocols();
                updater.UpdateApplications();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                GXCommon.ShowError(this, ex);
            }
        }

        void updater_OnProgress(GXAddIn sender)
        {
            ListViewItem target = null;
            foreach (ListViewItem it in listView1.Items)
            {
                GXAddIn addIn = it.Tag as GXAddIn;
                if (addIn.Name == sender.Name)
                {
                    addIn.ProgressPercentage = sender.ProgressPercentage;
                    target = it;
                    break;
                }
            }
            if (target != null)
            {
                listView1.RedrawItems(target.Index, target.Index, false);
            }
        }

        private void listView1_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void listView1_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            GXAddIn w = (GXAddIn)e.Item.Tag;
            if (e.ColumnIndex == 1 && w.ProgressPercentage != 0)//"ProgressBar"
            {
                e.DrawBackground();
                Rectangle progressBarRect = e.Bounds;
                double progress = w.ProgressPercentage * progressBarRect.Width;
                progress /= 100;
                progressBarRect.Width = Convert.ToInt32(progress);
                e.Graphics.FillRectangle(System.Drawing.SystemBrushes.ActiveCaption, progressBarRect);
                ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            }
            else
            {
                e.DrawDefault = true;
            }
        }

    }
}
