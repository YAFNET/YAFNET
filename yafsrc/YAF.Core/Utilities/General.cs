/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2021 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Core.Utilities
{
    #region Using

    using System.Web;

    using YAF.Types;

    #endregion

    /// <summary>
    /// General Utils.
    /// </summary>
    public static class General
    {
        #region Public Methods

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

        #endregion
    }
}