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
        /// The message cache.
        /// </summary>
        private static readonly IDictionary<Message, string> MessageCache = new Dictionary<Message, string>();

        /// <summary>
        /// The resources.
        /// </summary>
        private static readonly ResourceManager Resources = new ResourceManager(Constants.Messages, Assembly.GetExecutingAssembly());

        /// <summary>
        /// Formats a string.
        /// </summary>
        /// <param name="message">The message ID</param>
        /// <param name="args">The arguments</param>
        /// <returns>The formatted string</returns>
        public static string FormatString(Message message, params object[] args)
        {
            string format;

            lock (MessageCache)
            {
                if (MessageCache.ContainsKey(message))
                {
                    format = MessageCache[message];
                }
                else
                {
                    format = Resources.GetString(message.ToString());
                    MessageCache.Add(message, format);
                }
            }

            return string.Format(format ?? string.Empty, args);
        }
    }
}
