namespace Gurux.DLMS.XmlTester
{
    partial class Form1
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
            this.OpenBtn = new System.Windows.Forms.Button();
            this.TraceTB = new System.Windows.Forms.TextBox();
            this.PduOnlyCB = new System.Windows.Forms.CheckBox();
            this.CompletePDUCB = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // OpenBtn
            // 
            this.OpenBtn.Location = new System.Drawing.Point(335, 10);
            this.OpenBtn.Name = "OpenBtn";
            this.OpenBtn.Size = new System.Drawing.Size(69, 30);
            this.OpenBtn.TabIndex = 0;
            this.OpenBtn.Text = "Open";
            this.OpenBtn.UseVisualStyleBackColor = true;
            this.OpenBtn.Click += new System.EventHandler(this.OpenBtn_Click);
            // 
            // TraceTB
            // 
            this.TraceTB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TraceTB.Location = new System.Drawing.Point(12, 46);
            this.TraceTB.Multiline = true;
            this.TraceTB.Name = "TraceTB";
            this.TraceTB.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TraceTB.Size = new System.Drawing.Size(392, 239);
            this.TraceTB.TabIndex = 1;
            // 
            // PduOnlyCB
            // 
            this.PduOnlyCB.AutoSize = true;
            this.PduOnlyCB.Location = new System.Drawing.Point(17, 15);
            this.PduOnlyCB.Name = "PduOnlyCB";
            this.PduOnlyCB.Size = new System.Drawing.Size(69, 17);
            this.PduOnlyCB.TabIndex = 2;
            this.PduOnlyCB.Text = "Pdu Only";
            this.PduOnlyCB.UseVisualStyleBackColor = true;
            this.PduOnlyCB.CheckedChanged += new System.EventHandler(this.Update);
            // 
            // CompletePDUCB
            // 
            this.CompletePDUCB.AutoSize = true;
            this.CompletePDUCB.Location = new System.Drawing.Point(92, 12);
            this.CompletePDUCB.Name = "CompletePDUCB";
            this.CompletePDUCB.Size = new System.Drawing.Size(96, 17);
            this.CompletePDUCB.TabIndex = 3;
            this.CompletePDUCB.Text = "Complete PDU";
            this.CompletePDUCB.UseVisualStyleBackColor = true;
            this.CompletePDUCB.CheckedChanged += new System.EventHandler(this.Update);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(416, 297);
            this.Controls.Add(this.CompletePDUCB);
            this.Controls.Add(this.PduOnlyCB);
            this.Controls.Add(this.TraceTB);
            this.Controls.Add(this.OpenBtn);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button OpenBtn;
        private System.Windows.Forms.TextBox TraceTB;
        private System.Windows.Forms.CheckBox PduOnlyCB;
        private System.Windows.Forms.CheckBox CompletePDUCB;
    }
}

