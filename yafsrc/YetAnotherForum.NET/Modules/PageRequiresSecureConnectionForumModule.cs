/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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
namespace YAF.Modules
{
    #region Using

    using System;
    using System.Web;

    using YAF.Types;
    using YAF.Types.Attributes;

    #endregion

    /// <summary>
    /// The page requires secure connection module.
    /// </summary>
    [Module("Page Requires Secure Connection Module", "Tiny Gecko", 1)]
    public class PageRequiresSecureConnectionForumModule : SimpleBaseForumModule
    {
        #region Public Methods

        /// <summary>
        /// The init forum.
        /// </summary>
        public override void InitForum()
        {
            this.ForumControl.Load += this.ForumControl_Load;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The forum control_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ForumControl_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (HttpContext.Current.Request.IsLocal || HttpContext.Current.Request.IsSecureConnection ||
                !this.PageContext.BoardSettings.RequireSSL)
            {
                return;
            }

            var redirectUrl = HttpContext.Current.Request.Url.ToString().Replace("http:", "https:");
            HttpContext.Current.Response.Redirect(redirectUrl, false);
        }

        #endregion
    }
}