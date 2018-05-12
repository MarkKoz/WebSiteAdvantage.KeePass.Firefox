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
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using WebSiteAdvantage.KeePass.Firefox.Logging.LayoutRenderers;

namespace WebSiteAdvantage.KeePass.Firefox.Utilities
{
    /// <summary>
    /// A form which displays errors in a user-friendly manner.
    /// </summary>
    public partial class ErrorDialog : Form
    {
        private static readonly PrettyExceptionLayoutRenderer _Renderer =
            new PrettyExceptionLayoutRenderer { Indent = string.Empty, NewLine = Environment.NewLine };

        internal ErrorDialog()
        {
            InitializeComponent();
            Icon = SystemIcons.Error;
        }

        /// <summary>
        /// The message to display as a header.
        /// </summary>
        public string Message
        {
            get => labelMessage.Text;
            internal set => labelMessage.Text = value;
        }

        /// <summary>
        /// The error log (e.g. stack trace) to display in the text box.
        /// </summary>
        public string Log
        {
            get => textBoxLog.Text;
            internal set => textBoxLog.Text = value;
        }

        /// <summary>
        /// Displays a message, diagnostic info, and an exception stack trace in a dialog box.
        /// </summary>
        /// <param name="message">The error message to display.</param>
        /// <param name="exception">The exception caught.</param>
        public static void Show(string message, Exception exception)
        {
            var dialog = new ErrorDialog { Message = message };
            var sb = new StringBuilder();
            AssemblyName assembly = Assembly.GetCallingAssembly().GetName();

            sb.AppendLine("Message: " + message);
            sb.AppendLine("Program: " + assembly.Name);
            sb.AppendLine("Version: " + assembly.Version);
            sb.AppendLine("Library Version: " + Assembly.GetExecutingAssembly().GetName().Version);
            sb.AppendLine("Process: " + (Environment.Is64BitProcess ? "64-bit" : "not 64-bit"));
            sb.AppendLine("Processor Architecture: " + assembly.ProcessorArchitecture);
            sb.AppendLine();
            _Renderer.Append(sb, exception);

            dialog.Log = sb.ToString();
            dialog.ShowDialog();
        }

        private void EmailClickedEventHandler(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("mailto:tony@websiteadvantage.com.au");
        }

        private void TroubleshootingClickedEventHandler(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://websiteadvantage.com.au/Firefox-KeePass-Password-Import#heading-trouble");
        }
    }
}
