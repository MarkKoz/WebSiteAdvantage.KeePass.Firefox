/* WebSiteAdvantage KeePass to Firefox
 * Copyright (C) 2008 - 2012  Anthony James McCreath
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

using System;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace WebSiteAdvantage.KeePass.Firefox
{
    public partial class ErrorMessage : Form
    {
        public ErrorMessage()
        {
            InitializeComponent();
        }

        public string Message {
            get
            {
            	return this.labelMessage.Text;
            }
            set {
                this.labelMessage.Text = value;
            }
        }

        public string Log
        {
            get
            {
                return this.textBoxLog.Text;
            }
            set
            {
                this.textBoxLog.Text = value;
            }
        }

        public static void ShowErrorMessage(string toolName, string message, Exception ex)
        {

            ErrorMessage dialog = new ErrorMessage();

            dialog.Message = message;

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Message: " + message);
            sb.AppendLine("Program: " + toolName);
             sb.AppendLine("Version: " + KeePassUtilities.Version);


             sb.AppendLine("Assembly Version: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

             if (Environment.Is64BitProcess)
                 sb.AppendLine("Process: 64bit");
             else
                 sb.AppendLine("Process: not 64bit");

             sb.AppendLine("Processor Architecture: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().ProcessorArchitecture.ToString());


            if (ex !=null)
            {
                sb.AppendLine();
                sb.AppendLine("Exception");
                sb.AppendLine(ex.Message);
                sb.AppendLine("Source: "+ex.Source);
                sb.AppendLine(ex.StackTrace);

                if (ex.InnerException!=null)
                {
                sb.AppendLine();

                sb.AppendLine("Inner Exception");
                sb.AppendLine(ex.InnerException.Message);
                sb.AppendLine("Source: "+ex.InnerException.Source);
                sb.AppendLine(ex.InnerException.StackTrace);
                }
            }
            dialog.Log = sb.ToString();

            dialog.ShowDialog();
        }


        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("mailto:tony@websiteadvantage.com.au");

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://websiteadvantage.com.au/Firefox-KeePass-Password-Import#heading-trouble");
        }
    }
}
