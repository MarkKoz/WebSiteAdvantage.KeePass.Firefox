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
