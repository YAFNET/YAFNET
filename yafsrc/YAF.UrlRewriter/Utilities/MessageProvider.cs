// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Intelligencia" file="MessageProvider.cs">
//   Copyright (c)2011 Seth Yates
//   //   Author Seth Yates
//   //   Author Stewart Rae
// </copyright>
// <summary>
//   Forked Version for YAF.NET
//   Original can be found at https://github.com/sethyates/urlrewriter
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YAF.UrlRewriter.Utilities
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Resources;

    /// <summary>
    /// Message provider.
    /// </summary>
    internal static class MessageProvider
    {
        /// <summary>
        /// Formats a string.
        /// </summary>
        /// <param name="message">The message ID</param>
        /// <param name="args">The arguments</param>
        /// <returns>The formatted string</returns>
        public static string FormatString(Message message, params object[] args)
        {
            string format;

            lock (_messageCache)
            {
                if (_messageCache.ContainsKey(message))
                {
                    format = _messageCache[message];
                }
                else
                {
                    format = _resources.GetString(message.ToString());
                    _messageCache.Add(message, format);
                }
            }

            return string.Format(format, args);
        }

        private static IDictionary<Message, string> _messageCache = new Dictionary<Message, string>();
        private static ResourceManager _resources = new ResourceManager(Constants.Messages, Assembly.GetExecutingAssembly());
    }
}
