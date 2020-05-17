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
namespace YAF.Modules
{
    #region Using

    using System;
    using System.Web.UI.WebControls;

    using YAF.Configuration;
    using YAF.Core.Context;
    using YAF.Types;
    using YAF.Types.Attributes;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// Page Logo Handler Module
    /// </summary>
    [Module("Page Logo Handler Module", "Tiny Gecko", 1)]
    public class PageLogoHandlerForumModule : SimpleBaseForumModule
    {
        #region Public Methods

        /// <summary>
        /// The init after page.
        /// </summary>
        public override void InitAfterPage()
        {
            this.CurrentForumPage.PreRender += this.ForumPage_PreRender;
        }

        /// <summary>
        /// The init before page.
        /// </summary>
        public override void InitBeforePage()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// The forum page_ pre render.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ForumPage_PreRender([NotNull] object sender, [NotNull] EventArgs e)
        {
            var bannerLink = this.CurrentForumPage.FindControlRecursiveBothAs<HyperLink>("BannerLink");

            if (bannerLink == null)
            {
                return;
            }

            bannerLink.NavigateUrl = BuildLink.GetLink(ForumPages.Board);
            bannerLink.ToolTip = this.GetText("TOOLBAR", "FORUM_TITLE");

            var logoUrl = $"{BoardInfo.ForumClientFileRoot}{BoardFolders.Current.Logos}/{BoardContext.Current.BoardSettings.ForumLogo}";

            bannerLink.Attributes.Add("style", $"background: url('{logoUrl}') no-repeat");
            
            if (!this.CurrentForumPage.ShowToolBar)
            {
                bannerLink.Visible = false;
            }
        }

        #endregion
    }
}