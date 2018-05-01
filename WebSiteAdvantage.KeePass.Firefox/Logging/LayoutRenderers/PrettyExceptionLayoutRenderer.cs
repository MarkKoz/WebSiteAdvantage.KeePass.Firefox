using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using NLog;
using NLog.LayoutRenderers;

namespace WebSiteAdvantage.KeePass.Firefox.Logging.LayoutRenderers
{
    [LayoutRenderer("pretty-exception")]
    public class PrettyExceptionLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        /// A line's indentation. Nested lines (e.g. those of a stack trace) double this value.
        /// </summary>
        public string Indent { get; set; } = new string(' ', 4);

        /// <summary>
        /// Denotes if the exceptions' full type should be used.
        /// </summary>
        public bool FullTypeName { get; set; } = true;

        /// <summary>
        /// String before the exceptions' type.
        /// </summary>
        public string BeforeType { get; set; } = "[";

        /// <summary>
        /// String after the exceptions' type.
        /// </summary>
        public string AfterType { get; set; } = "] ";

        /// <summary>
        /// Denotes if the top-level exception's stack trace is logged.
        /// </summary>
        public bool LogStack { get; set; } = true;

        /// <summary>
        /// Maximum number of inner exceptions to log.
        /// </summary>
        public int MaxInnerExceptions { get; set; } = 10;

        /// <summary>
        /// Denotes if the inner exceptions' stack trace are logged.
        /// </summary>
        public bool LogInnerStacks { get; set; } = true;

        /// <summary>
        /// Contents of the line between an inner exception and the previous exception.
        /// </summary>
        public string InnerSeperator { get; set; } = string.Empty;

        /// <inheritdoc />
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            int loggedInner = 0;
            Exception eTop = logEvent.Exception;
            Exception e = eTop;

            while (e != null)
            {
                if (e != eTop)
                {
                    ++loggedInner;
                    builder.Append(InnerSeperator);
                }

                string type = FullTypeName ? e.GetType().FullName : e.GetType().Name;
                builder.AppendFormat("{0}{1}{2}{3}{4}\n", Indent, BeforeType, type, AfterType, FormatMessage(e.Message));

                if ((e == eTop && LogStack) || (e != eTop && LogInnerStacks))
                    foreach (string line in ReadLines(e.StackTrace))
                        builder.AppendFormat("{0}{0}{1}\n", Indent, line);

                // Moves on to the inner exception if the limit hasn't been reached.
                e = loggedInner < MaxInnerExceptions ? e.InnerException : null;
            }

            RemoveFinalNewline(ref builder);
        }

        /// <summary>
        /// Properly formats an exception's message if it contains multiple lines.
        /// </summary>
        /// <param name="message">The message to format.</param>
        /// <returns>The formatted message.</returns>
        private string FormatMessage(string message)
        {
            string[] lines = ReadLines(message).ToArray();

            if (lines.Length < 2)
                return message;

            // First line should not be indented because it will be on the same line as the exception's type.
            var builder = new StringBuilder(lines.First() + '\n', lines.Length);

            foreach (string line in lines.Skip(1))
                builder.AppendFormat("{0}{1}\n", Indent, line);

            RemoveFinalNewline(ref builder);

            return builder.ToString();
        }

        /// <summary>
        /// Yields lines read from a string.
        /// </summary>
        /// <param name="source">The string to read.</param>
        /// <returns>The lines contained by <paramref name="source"/>.</returns>
        private static IEnumerable<string> ReadLines(string source)
        {
            using (var reader = new StringReader(source))
            {
                string line;

                while ((line = reader.ReadLine()?.Trim()) != null)
                    yield return line;
            }
        }

        /// <summary>
        /// If it exists, removes the final newline character from a <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="builder">The string builder from which to remove the final newline.</param>
        private static void RemoveFinalNewline(ref StringBuilder builder)
        {
            if (builder.Length != 0 && builder[builder.Length - 1] == '\n')
                --builder.Length;
        }
    }
}
