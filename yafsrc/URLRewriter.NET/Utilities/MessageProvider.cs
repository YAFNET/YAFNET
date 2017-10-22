// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

using System;
using System.Collections.Generic;
using System.Resources;
using System.Reflection;

namespace Intelligencia.UrlRewriter.Utilities
{
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

            return String.Format(format, args);
        }

        private static IDictionary<Message, string> _messageCache = new Dictionary<Message, string>();
        private static ResourceManager _resources = new ResourceManager(Constants.Messages, Assembly.GetExecutingAssembly());
    }
}
