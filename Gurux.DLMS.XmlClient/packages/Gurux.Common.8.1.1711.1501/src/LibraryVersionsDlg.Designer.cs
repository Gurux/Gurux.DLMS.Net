using Gurux.Common.Properties;
namespace Gurux.Common
{
    partial class LibraryVersionsDlg
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.CopyBtn = new System.Windows.Forms.Button();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.NameHeader = new System.Windows.Forms.ColumnHeader();
            this.VersionHeader = new System.Windows.Forms.ColumnHeader();
            this.LocationHeader = new System.Windows.Forms.ColumnHeader();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.SuspendLayout();
            // 
            // CopyBtn
            // 
            this.CopyBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CopyBtn.Location = new System.Drawing.Point(8, 338);
            this.CopyBtn.Name = "CopyBtn";
            this.helpProvider1.SetShowHelp(this.CopyBtn, true);
            this.CopyBtn.Size = new System.Drawing.Size(72, 24);
            this.CopyBtn.TabIndex = 4;
            this.CopyBtn.Text = "CopyBtn";
            this.CopyBtn.Click += new System.EventHandler(this.CopyBtn_Click);
            // 
            // CancelBtn
            // 
            this.CancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.Location = new System.Drawing.Point(392, 338);
            this.CancelBtn.Name = "CancelBtn";
            this.helpProvider1.SetShowHelp(this.CancelBtn, true);
            this.CancelBtn.Size = new System.Drawing.Size(72, 24);
            this.CancelBtn.TabIndex = 5;
            this.CancelBtn.Text = "CancelBtn";
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.NameHeader,
            this.VersionHeader,
            this.LocationHeader});
            this.listView1.FullRowSelect = true;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView1.Location = new System.Drawing.Point(0, 2);
            this.listView1.Name = "listView1";
            this.helpProvider1.SetShowHelp(this.listView1, true);
            this.listView1.Size = new System.Drawing.Size(496, 328);
            this.listView1.TabIndex = 3;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // NameHeader
            // 
            this.NameHeader.Text = "NameHeader";
            this.NameHeader.Width = 181;
            // 
            // VersionHeader
            // 
            this.VersionHeader.Text = "VersionHeader";
            this.VersionHeader.Width = 185;
            // 
            // LocationHeader
            //             
            // 
            // LibraryVersionsDlg
            // 
            this.AcceptButton = this.CancelBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelBtn;
            this.ClientSize = new System.Drawing.Size(496, 365);
            this.ControlBox = false;
            this.Controls.Add(this.CopyBtn);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.listView1);
            this.Name = "LibraryVersionsDlg";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "LibraryVersionsDlg";
            this.Load += new System.EventHandler(this.LibraryVersionsDlg_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button CopyBtn;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader NameHeader;
        private System.Windows.Forms.ColumnHeader VersionHeader;
        private System.Windows.Forms.ColumnHeader LocationHeader;
    }
}