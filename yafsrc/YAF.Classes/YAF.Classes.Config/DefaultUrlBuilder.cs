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
namespace YAF.Classes
{
    #region Using

    using YAF.Types.Extensions;

    #endregion

    /// <summary>
    /// Implements URL Builder.
    /// </summary>
    public class DefaultUrlBuilder : BaseUrlBuilder
    {
        #region Public Methods

        /// <summary>
        /// Builds path for calling page with parameter URL as page's escaped parameter.
        /// </summary>
        /// <param name="url">
        /// URL to put into parameter.
        /// </param>
        /// <returns>
        /// URL to calling page with URL argument as page's parameter with escaped characters to make it valid parameter.
        /// </returns>
        public override string BuildUrl(string url)
        {
            // escape & to &amp;
            url = url.Replace("&", "&amp;");

            // return URL to current script with URL from parameter as script's parameter
            return "{0}{1}?{2}".FormatWith(AppPath, Config.ForceScriptName ?? ScriptName, url);
        }

        /// <summary>
        /// Builds path for calling page with parameter URL as page's escaped parameter.
        /// </summary>
        /// <param name="boardSettings">The board settings.</param>
        /// <param name="url">URL to put into parameter.</param>
        /// <returns>
        /// URL to calling page with URL argument as page's parameter with escaped characters to make it valid parameter.
        /// </returns>
        public override string BuildUrl(object boardSettings, string url)
        {
            return this.BuildUrl(url);
        }

        #endregion
    }
}