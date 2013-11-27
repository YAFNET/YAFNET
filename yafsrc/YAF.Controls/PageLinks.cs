/* Yet Another Forum.NET
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
namespace YAF.Controls
{
    #region Using

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web.UI;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    [Serializable]
    public class PageLink
    {
        public string Title { get; set; }
        public string URL { get; set; }
    }

    public static class PageLinkExtensions
    {
        public static PageLinks AddRoot(this PageLinks pageLinks)
        {
            CodeContracts.VerifyNotNull(pageLinks, "pageLinks");

            pageLinks.AddLink(pageLinks.Get<YafBoardSettings>().Name, YafBuildLink.GetLink(ForumPages.forum));

            return pageLinks;
        }

        public static PageLinks AddCategory(this PageLinks pageLinks, [NotNull] string categoryName, [NotNull] int categoryId)
        {
            CodeContracts.VerifyNotNull(pageLinks, "pageLinks");
            CodeContracts.VerifyNotNull(categoryName, "categoryName");

            pageLinks.AddLink(categoryName, YafBuildLink.GetLink(ForumPages.forum, "c={0}", categoryId));

            return pageLinks;
        }

        public static PageLinks AddLink(this PageLinks pageLinks, [NotNull] string title, [CanBeNull] string url = "")
        {
            CodeContracts.VerifyNotNull(pageLinks, "pageLinks");
            CodeContracts.VerifyNotNull(title, "title");

            pageLinks.Add(new PageLink() { Title = title.Trim(), URL = url.Trim() });

            return pageLinks;
        }

        /// <summary>
        /// Adds the forum links.
        /// </summary>
        /// <param name="forumId">
        /// The forum id.
        /// </param>
        /// <param name="noForumLink">
        /// The no forum link.
        /// </param>
        public static PageLinks AddForum(this PageLinks pageLinks, int forumId, bool noForumLink = false)
        {
            CodeContracts.VerifyNotNull(pageLinks, "pageLinks");

            using (DataTable dtLinks = LegacyDb.forum_listpath(forumId))
            {
                foreach (DataRow row in dtLinks.Rows)
                {
                    if (noForumLink && row["ForumID"].ToType<int>() == forumId)
                    {
                        pageLinks.AddLink(row["Name"].ToString(), string.Empty);
                    }
                    else
                    {
                        pageLinks.AddLink(
                            row["Name"].ToString(),
                            YafBuildLink.GetLink(ForumPages.topics, "f={0}", row["ForumID"]));
                    }
                }
            }

            return pageLinks;
        }
    }

    /// <summary>
    /// Page Links Control.
    /// </summary>
    public class PageLinks : BaseControl, IAdd<PageLink>
    {
        #region Properties

        /// <summary>
        ///   Gets or sets LinkedPageLinkID.
        /// </summary>
        [CanBeNull]
        public string LinkedPageLinkID
        {
            get
            {
                return this.ViewState["LinkedPageLinkID"].ToType<string>();
            }

            set
            {
                this.ViewState["LinkedPageLinkID"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets PageLink List
        /// </summary>
        [CanBeNull]
        public List<PageLink> PageLinkList
        {
            get
            {
                return this.ViewState["PageLinkList"] as List<PageLink>;
            }

            set
            {
                this.ViewState["PageLinkList"] = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Clear all Links
        /// </summary>
        public void Clear()
        {
            this.PageLinkList = null;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sends server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter" /> object, which writes the content to be rendered on the client.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter" /> object that receives the server control content.</param>
        protected override void Render([NotNull] HtmlTextWriter writer)
        {
            List<PageLink> linkedPageList = null;

            if (this.LinkedPageLinkID.IsSet())
            {
                // attempt to get access to the other control...
                var parentControl = this.Parent.FindControl(this.LinkedPageLinkID) as PageLinks;

                if (parentControl != null)
                {
                    // use the other data stream...
                    linkedPageList = parentControl.PageLinkList;
                }
            }
            else
            {
                // use the data table from this control...
                linkedPageList = this.PageLinkList;
            }

            if (linkedPageList == null || !linkedPageList.Any())
            {
                return;
            }

            writer.WriteLine(@"<div id=""{0}"" class=""yafPageLink breadcrumb"">".FormatWith(this.ClientID));

            var first = true;

            foreach (var link in linkedPageList)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.WriteLine(@"<span class=""linkSeperator divider"">&nbsp;&#187;&nbsp;</span>");
                }

                string encodedTitle = this.HtmlEncode(link.Title);
                string url = link.URL;

                writer.WriteLine(
                    url.IsNotSet()
                        ? @"<span class=""currentPageLink active"">{0}</span>".FormatWith(encodedTitle)
                        : @"<a href=""{0}"">{1}</a>".FormatWith(url, encodedTitle));
            }

            writer.WriteLine("</div>");
        }

        #endregion

        public void Add([NotNull] PageLink item)
        {
            CodeContracts.VerifyNotNull(item, "item");

            var list = this.PageLinkList ?? new List<PageLink>();

            list.Add(item);

            this.PageLinkList = list;
        }
    }
}