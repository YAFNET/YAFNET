/* YetAnotherForum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
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