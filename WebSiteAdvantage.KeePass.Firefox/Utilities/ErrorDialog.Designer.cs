/*
 * WebSiteAdvantage KeePass to Firefox
 *
 * Copyright (C) 2008 - 2012 Anthony James McCreath
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
 */

namespace WebSiteAdvantage.KeePass.Firefox.Utilities
{
    partial class ErrorDialog
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
            this.textBoxLog = new System.Windows.Forms.TextBox();
            this.labelMessage = new System.Windows.Forms.Label();
            this.labelLog = new System.Windows.Forms.Label();
            this.buttonClose = new System.Windows.Forms.Button();
            this.linkEmail = new System.Windows.Forms.LinkLabel();
            this.labelEmail = new System.Windows.Forms.Label();
            this.labelTroubleshooting = new System.Windows.Forms.Label();
            this.linkTroubleshooting = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            //
            // textBoxLog
            //
            this.textBoxLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxLog.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxLog.Location = new System.Drawing.Point(12, 114);
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ReadOnly = true;
            this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxLog.Size = new System.Drawing.Size(522, 233);
            this.textBoxLog.TabIndex = 0;
            this.textBoxLog.WordWrap = false;
            //
            // labelMessage
            //
            this.labelMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMessage.Location = new System.Drawing.Point(12, 19);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(522, 34);
            this.labelMessage.TabIndex = 1;
            this.labelMessage.Text = "Something Went Wrong";
            //
            // labelLog
            //
            this.labelLog.AutoSize = true;
            this.labelLog.Location = new System.Drawing.Point(9, 98);
            this.labelLog.Name = "labelLog";
            this.labelLog.Size = new System.Drawing.Size(317, 13);
            this.labelLog.TabIndex = 2;
            this.labelLog.Text = "When reporting this error please copy and include the data below.";
            //
            // buttonClose
            //
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonClose.Location = new System.Drawing.Point(459, 353);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 3;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            //
            // linkEmail
            //
            this.linkEmail.AutoSize = true;
            this.linkEmail.Location = new System.Drawing.Point(228, 75);
            this.linkEmail.Name = "linkEmail";
            this.linkEmail.Size = new System.Drawing.Size(163, 13);
            this.linkEmail.TabIndex = 4;
            this.linkEmail.TabStop = true;
            this.linkEmail.Text = "tony@websiteadvantage.com.au";
            this.linkEmail.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.EmailClickedEventHandler);
            //
            // labelEmail
            //
            this.labelEmail.AutoSize = true;
            this.labelEmail.Location = new System.Drawing.Point(9, 75);
            this.labelEmail.Name = "labelEmail";
            this.labelEmail.Size = new System.Drawing.Size(222, 13);
            this.labelEmail.TabIndex = 5;
            this.labelEmail.Text = "If that does not help then send error reports to";
            //
            // labelTroubleshooting
            //
            this.labelTroubleshooting.AutoSize = true;
            this.labelTroubleshooting.Location = new System.Drawing.Point(9, 53);
            this.labelTroubleshooting.Name = "labelTroubleshooting";
            this.labelTroubleshooting.Size = new System.Drawing.Size(109, 13);
            this.labelTroubleshooting.TabIndex = 6;
            this.labelTroubleshooting.Text = "Please first check the";
            //
            // linkTroubleshooting
            //
            this.linkTroubleshooting.AutoSize = true;
            this.linkTroubleshooting.Location = new System.Drawing.Point(113, 53);
            this.linkTroubleshooting.Name = "linkTroubleshooting";
            this.linkTroubleshooting.Size = new System.Drawing.Size(122, 13);
            this.linkTroubleshooting.TabIndex = 7;
            this.linkTroubleshooting.TabStop = true;
            this.linkTroubleshooting.Text = "trouble shooting section.";
            this.linkTroubleshooting.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.TroubleshootingClickedEventHandler);
            //
            // ErrorDialog
            //
            this.AcceptButton = this.buttonClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(546, 383);
            this.Controls.Add(this.linkTroubleshooting);
            this.Controls.Add(this.labelTroubleshooting);
            this.Controls.Add(this.labelEmail);
            this.Controls.Add(this.linkEmail);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.labelLog);
            this.Controls.Add(this.labelMessage);
            this.Controls.Add(this.textBoxLog);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ErrorDialog";
            this.Text = "Error";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxLog;
        private System.Windows.Forms.Label labelMessage;
        private System.Windows.Forms.Label labelLog;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.LinkLabel linkEmail;
        private System.Windows.Forms.Label labelEmail;
        private System.Windows.Forms.Label labelTroubleshooting;
        private System.Windows.Forms.LinkLabel linkTroubleshooting;
    }
}
