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
namespace YAF.Controls
{
    #region Using

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
    using YAF.Types.Objects;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The PageLink Extensions
    /// </summary>
    public static class PageLinkExtensions
    {
        /// <summary>
        /// Adds the root.
        /// </summary>
        /// <param name="pageLinks">The page links.</param>
        /// <returns>returns the Page links including the root</returns>
        public static PageLinks AddRoot(this PageLinks pageLinks)
        {
            CodeContracts.VerifyNotNull(pageLinks, "pageLinks");

            pageLinks.AddLink(pageLinks.Get<YafBoardSettings>().Name, YafBuildLink.GetLink(ForumPages.forum));

            return pageLinks;
        }

        /// <summary>
        /// Adds the category.
        /// </summary>
        /// <param name="pageLinks">The page links.</param>
        /// <param name="categoryName">Name of the category.</param>
        /// <param name="categoryId">The category identifier.</param>
        /// <returns>Returns the Page links including the Category</returns>
        public static PageLinks AddCategory(
            this PageLinks pageLinks,
            [NotNull] string categoryName,
            [NotNull] int categoryId)
        {
            CodeContracts.VerifyNotNull(pageLinks, "pageLinks");
            CodeContracts.VerifyNotNull(categoryName, "categoryName");

            pageLinks.AddLink(categoryName, YafBuildLink.GetLink(ForumPages.forum, "c={0}", categoryId));

            return pageLinks;
        }

        /// <summary>
        /// Adds the link.
        /// </summary>
        /// <param name="pageLinks">The page links.</param>
        /// <param name="title">The title.</param>
        /// <param name="url">The URL.</param>
        /// <returns>Returns the page links</returns>
        public static PageLinks AddLink(this PageLinks pageLinks, [NotNull] string title, [CanBeNull] string url = "")
        {
            CodeContracts.VerifyNotNull(pageLinks, "pageLinks");
            CodeContracts.VerifyNotNull(title, "title");

            pageLinks.Add(new PageLink() { Title = title.Trim(), URL = url?.Trim() });

            return pageLinks;
        }

        /// <summary>
        /// Adds the forum links.
        /// </summary>
        /// <param name="pageLinks">The page links.</param>
        /// <param name="forumId">The forum id.</param>
        /// <param name="noForumLink">The no forum link.</param>
        /// <returns>Returns the page links</returns>
        public static PageLinks AddForum(this PageLinks pageLinks, int forumId, bool noForumLink = false)
        {
            CodeContracts.VerifyNotNull(pageLinks, "pageLinks");

            using (var links = LegacyDb.forum_listpath(forumId))
            {
                foreach (DataRow row in links.Rows)
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

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Add([NotNull] PageLink item)
        {
            CodeContracts.VerifyNotNull(item, "item");

            var list = this.PageLinkList ?? new List<PageLink>();

            list.Add(item);

            this.PageLinkList = list;
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

            writer.Write("<div class=\"navbar-header d-none d-md-block\"><ol class=\"breadcrumb\">");

            foreach (var link in linkedPageList)
            {
                var encodedTitle = this.HtmlEncode(link.Title);
                var url = link.URL;

                writer.WriteLine(
                    url.IsNotSet()
                        ? @"<li class=""breadcrumb-item active"">{0}</li>".FormatWith(encodedTitle)
                        : @"<li class=""breadcrumb-item""><a href=""{0}"">{1}</a></li>".FormatWith(url, encodedTitle));
            }

            writer.Write(this.PageContext.CurrentForumPage.IsAdminPage ? "</ol>" : "</ol></div>");
        }

        #endregion
    }
}