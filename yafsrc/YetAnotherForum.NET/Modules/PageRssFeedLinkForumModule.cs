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

    using System.Web.UI.HtmlControls;
    using YAF.Types.Attributes;

    #endregion

    /// <summary>
    /// Summary description for PageRssFeedLinkModule
    /// </summary>
    [Module("Page Rss Feed Link Module", "Tiny Gecko", 1)]
    public class PageRssFeedLinkForumModule : SimpleBaseForumModule
    {
        #region Public Methods

        /// <summary>
        /// The init after page.
        /// </summary>
        public override void InitAfterPage()
        {
            this.CurrentForumPage.PreRender += this.ForumPage_PreRender;
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
            var head = this.ForumControl.Page.Header ??
                            this.CurrentForumPage.FindControlRecursiveBothAs<HtmlHead>("YafHead");

            if (head == null)
            {
                return;
            }

            var groupAccess =
                this.Get<IPermissions>().Check(this.PageBoardContext.BoardSettings.PostLatestFeedAccess);

            if (!this.PageBoardContext.BoardSettings.ShowAtomLink || !groupAccess)
            {
                return;
            }

            // setup the rss link...
            var atomLink = new HtmlLink
            {
                Href =
                                       this.Get<LinkBuilder>().GetAbsoluteLink(
                                           ForumPages.Feed,
                                           new { feed = RssFeeds.LatestPosts.ToInt() })
            };

            // defaults to the "Active" rss.
            atomLink.Attributes.Add("rel", "alternate");
            atomLink.Attributes.Add("type", "application/atom+xml");
            atomLink.Attributes.Add(
                "title",
                $"{this.GetText("ATOMFEED")} - {this.PageBoardContext.BoardSettings.Name}");

            head.Controls.Add(atomLink);
        }

        #endregion
    }
}