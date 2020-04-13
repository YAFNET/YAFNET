/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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

namespace YAF.Core.Extensions
{
    #region Using

    using System;
    using System.Text;
    using System.Web.UI;

    using YAF.Core.Context;
    using YAF.Types.Extensions;
    using YAF.Utils.Helpers.StringUtils;

    #endregion

    /// <summary>
    /// The control extensions.
    /// </summary>
    public static class ControlExtensions
    {
        #region Public Methods

        /// <summary>
        /// Creates a ID Based on the Control Structure
        /// </summary>
        /// <param name="currentControl">The current Control.</param>
        /// <param name="prefix">The prefix.</param>
        /// <returns>
        /// The get extended id.
        /// </returns>
        public static string GetExtendedID(this Control currentControl, string prefix)
        {
            var createdID = new StringBuilder();

            if (currentControl.ID.IsSet())
            {
                createdID.AppendFormat("{0}_", currentControl.ID);
            }

            createdID.Append(prefix.IsSet() ? prefix : Guid.NewGuid().ToString().Substring(0, 5));

            return createdID.ToString();
        }

        /// <summary>
        /// Creates a Unique ID
        /// </summary>
        /// <param name="currentControl">The current Control.</param>
        /// <param name="prefix">The prefix.</param>
        /// <returns>
        /// The get unique id.
        /// </returns>
        public static string GetUniqueID(this Control currentControl, string prefix)
        {
            return prefix.IsSet()
                       ? $"{prefix}{Guid.NewGuid().ToString().Substring(0, 5)}"
                       : Guid.NewGuid().ToString().Substring(0, 10);
        }

        /// <summary>
        /// XSS Encode input String.
        /// </summary>
        /// <param name="currentControl">The current Control.</param>
        /// <param name="data">The data.</param>
        /// <returns>
        /// The encoded string.
        /// </returns>
        public static string HtmlEncode(this Control currentControl, object data)
        {
            if (data == null)
            {
                return null;
            }

            return BoardContext.Current != null
                       ? new UnicodeEncoder().XSSEncode(data.ToString())
                       : BoardContext.Current.CurrentForumPage.HtmlEncode(data.ToString());
        }

        /// <summary>
        /// Gets PageContext.
        /// </summary>
        /// <param name="currentControl">
        /// The current Control.
        /// </param>
        /// <returns>
        /// The <see cref="BoardContext"/>.
        /// </returns>
        public static BoardContext PageContext(this Control currentControl)
        {
            if (currentControl.Site != null && currentControl.Site.DesignMode)
            {
                // design-time, return null...
                return null;
            }

            return BoardContext.Current;
        }

        #endregion
    }
}