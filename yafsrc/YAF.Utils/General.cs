/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Utils
{
    #region Using

    using System.Linq;
    using System.Reflection;
    using System.Web;

    using YAF.Types;
    using YAF.Types.Extensions;

    #endregion

    /// <summary>
    /// General Utils.
    /// </summary>
    public static class General
    {
        /* Ederon : 9/12/2007 */
        #region Public Methods

        /// <summary>
        /// Compares two messages.
        /// </summary>
        /// <param name="originalMessage">
        /// Original message text.
        /// </param>
        /// <param name="newMessage">
        /// New message text.
        /// </param>
        /// <returns>
        /// True if messages differ, <see langword="false"/> if they are identical.
        /// </returns>
        public static bool CompareMessage([NotNull] object originalMessage, [NotNull] object newMessage)
        {
            CodeContracts.VerifyNotNull(originalMessage, "originalMessage");
            CodeContracts.VerifyNotNull(newMessage, "newMessage");

            return originalMessage.ToString() != newMessage.ToString();
        }

        /// <summary>
        /// Encodes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>Returns the encoded message</returns>
        public static string EncodeMessage([NotNull] string message)
        {
            CodeContracts.VerifyNotNull(message, "message");

            return message.IndexOf('<') >= 0 ? HttpUtility.HtmlEncode(message) : message;
        }

        /// <summary>
        /// Gets the safe raw URL.
        /// </summary>
        /// <returns>Returns the safe raw URL</returns>
        public static string GetSafeRawUrl()
        {
            return GetSafeRawUrl(HttpContext.Current.Request.RawUrl);
        }

        /// <summary>
        /// Cleans up a URL so that it doesn't contain any problem characters.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>
        /// The get safe raw URL.
        /// </returns>
        [NotNull]
        public static string GetSafeRawUrl([NotNull] string url)
        {
            CodeContracts.VerifyNotNull(url, "url");

            var processedRaw = url;
            processedRaw = processedRaw.Replace("\"", string.Empty);
            processedRaw = processedRaw.Replace("<", "%3C");
            processedRaw = processedRaw.Replace(">", "%3E");
            processedRaw = processedRaw.Replace("&", "%26");
            return processedRaw.Replace("'", string.Empty);
        }

        /// <summary>
        /// Traces the resources.
        /// </summary>
        /// <returns>Returns the founded Resources</returns>
        public static string TraceResources()
        {
            var a = Assembly.GetExecutingAssembly();

            // get a list of resource names from the manifest
            var resNames = a.GetManifestResourceNames();

            // populate the textbox with information about our resources
            // also look for images and put them in our arraylist
            var txtInfo = string.Empty;

            txtInfo += "Found {0} resources\r\n".FormatWith(resNames.Length);
            txtInfo += "----------\r\n";

            txtInfo = resNames.Aggregate(txtInfo, (current, s) => current + (s + "\r\n"));

            txtInfo += "----------\r\n";

            return txtInfo;
        }

        #endregion
    }
}