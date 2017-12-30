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
namespace YAF.Modules
{
    #region Using

    using System;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Types;
    using YAF.Types.Attributes;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// Page Logo Handler Module
    /// </summary>
    [YafModule("Page Logo Handler Module", "Tiny Gecko", 1)]
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
            var htmlImgBanner = this.CurrentForumPage.FindControlRecursiveBothAs<HtmlImage>("imgBanner");
            var imgBanner = this.CurrentForumPage.FindControlRecursiveBothAs<Image>("imgBanner");
            var bannerLink = this.CurrentForumPage.FindControlRecursiveBothAs<HyperLink>("BannerLink");

            if (bannerLink != null)
            {
                bannerLink.NavigateUrl = YafBuildLink.GetLink(ForumPages.forum);
                bannerLink.ToolTip = this.GetText("TOOLBAR", "FORUM_TITLE");
            }

            if (!this.CurrentForumPage.ShowToolBar)
            {
                if (htmlImgBanner != null)
                {
                    htmlImgBanner.Visible = false;
                }
                else if (imgBanner != null)
                {
                    imgBanner.Visible = false;
                }
            }

            if (!this.Get<YafBoardSettings>().AllowThemedLogo || Config.IsAnyPortal)
            {
                return;
            }

            string graphicSrc = this.Get<ITheme>().GetItem("FORUM", "BANNER", null);

            if (!graphicSrc.IsSet())
            {
                return;
            }

            if (htmlImgBanner != null)
            {
                htmlImgBanner.Src = graphicSrc;
            }
            else if (imgBanner != null)
            {
                imgBanner.ImageUrl = graphicSrc;
            }
        }

        #endregion
    }
}