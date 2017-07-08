// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2007 Intelligencia
// Copyright 2007 Seth Yates
// 

namespace Intelligencia.UrlRewriter.Utilities
{
    using System.Collections;
    using System.Reflection;
    using System.Resources;

    /// <summary>
    /// Message provider.
    /// </summary>
    internal sealed class MessageProvider
    {
        private MessageProvider()
        {
        }

        public static string FormatString(Message message, params object[] args)
        {
            string format;

            lock (_messageCache.SyncRoot)
            {
                if (_messageCache.ContainsKey(message))
                {
                    format = (string)_messageCache[message];
                }
                else
                {
                    format = _resources.GetString(message.ToString());
                    _messageCache.Add(message, format);
                }
            }

            return string.Format(format, args);
        }

        private static Hashtable _messageCache = new Hashtable();

        private static ResourceManager _resources =
            new ResourceManager(Constants.Messages, Assembly.GetExecutingAssembly());
    }
}