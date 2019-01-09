/*
 * WebSiteAdvantage KeePass to Firefox
 *
 * Copyright (C) 2018-2019 Mark Kozlov
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

using System.Diagnostics;
using System.Text;

using NLog;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Targets;
using NLog.Targets.Wrappers;

using WebSiteAdvantage.KeePass.Firefox.Logging.LayoutRenderers;

namespace WebSiteAdvantage.KeePass.Firefox.Logging
{
    public static class LoggerService
    {
        /// <summary>
        /// Configures the logger.
        /// </summary>
        [Conditional("DEBUG")]
        public static void Initialise()
        {
            LayoutRenderer.Register<PrettyExceptionLayoutRenderer>("pretty-exception");
            LayoutRenderer.Register("linefeed", _ => '\n'); // Workaround for newline using Environment.NewLine.

            var config = new LoggingConfiguration();

            var fileTarget = new FileTarget
            {
                Name = "file",
                FileName = "${basedir}/WebSiteAdvantage.KeePass.Firefox.log",
                LineEnding = LineEndingMode.LF,
                Encoding = Encoding.UTF8,
                Layout = @"${date:format=yyyy-MM-ddTHH\:mm\:ss,fffK} - [${level:uppercase=true}] ${logger}: ${message}${onexception:${linefeed}${pretty-exception}}",
                KeepFileOpen = true,
                ConcurrentWrites = false,
                OpenFileCacheTimeout = 20
            };

            var bufferedFileTarget = new BufferingTargetWrapper(fileTarget, 5);
            config.AddTarget("buffFile", bufferedFileTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, bufferedFileTarget));

            LogManager.Configuration = config;
        }
    }
}
