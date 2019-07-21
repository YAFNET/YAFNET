/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
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
    using System.Linq;
    using System.Text;
    using System.Web.UI.HtmlControls;

   using YAF.Web;
    using YAF.Types;
    using YAF.Types.Attributes;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils.Helpers;
    using YAF.Web.Controls;

    #endregion

    /// <summary>
    /// Page Title Module
    /// </summary>
    [YafModule(moduleName: "Page Title Module", moduleAuthor: "Tiny Gecko", moduleVersion: 1)]
    public class PageTitleForumModule : SimpleBaseForumModule
    {
        #region Constants and Fields

        /// <summary>
        ///   The _forum page title.
        /// </summary>
        protected string _forumPageTitle;

        #endregion

        #region Public Methods

        /// <summary>
        /// The init after page.
        /// </summary>
        public override void InitAfterPage()
        {
            this.CurrentForumPage.PreRender += this.ForumPage_PreRender;
            this.CurrentForumPage.Load += this.ForumPage_Load;
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
        /// Handles the Load event of the ForumPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ForumPage_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.GeneratePageTitle();
        }

        /// <summary>
        /// Handles the PreRender event of the ForumPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ForumPage_PreRender([NotNull] object sender, [NotNull] EventArgs e)
        {
            var head = this.ForumControl.Page.Header
                            ?? this.CurrentForumPage.FindControlRecursiveBothAs<HtmlHead>(id: "YafHead");

            if (head != null)
            {
                // setup the title...
                var addition = string.Empty;

                if (head.Title.IsSet())
                {
                    addition = $" - {head.Title.Trim()}";
                }

                head.Title = this._forumPageTitle + addition;
            }
            else
            {
                // old style
                var title = this.CurrentForumPage.FindControlRecursiveBothAs<HtmlTitle>(id: "ForumTitle");

                if (title != null)
                {
                    title.Text = this._forumPageTitle;
                }
            }
        }

        /// <summary>
        /// Creates this pages title and fires a PageTitleSet event if one is set
        /// </summary>
        private void GeneratePageTitle()
        {
            // compute page title..
            var title = new StringBuilder();

            var pageString = string.Empty;

            if (this.ForumPageType == ForumPages.posts || this.ForumPageType == ForumPages.topics)
            {
                // get current page...
                var currentPager = this.CurrentForumPage.FindControlAs<Pager>(id: "Pager");

                if (currentPager != null && currentPager.CurrentPageIndex != 0)
                {
                    pageString = $"- Page {currentPager.CurrentPageIndex + 1}";
                }
            }

            var addBoardName = true;

            if (!this.PageContext.CurrentForumPage.IsAdminPage)
            {
                switch (this.ForumPageType)
                {
                    case ForumPages.posts:
                        if (this.PageContext.PageTopicID != 0)
                        {
                            // Tack on the topic we're viewing
                            title.Append(
                                value: this.Get<IBadWordReplace>().Replace(searchText: this.PageContext.PageTopicName.Truncate(inputLimit: 80)));
                        }

                        addBoardName = false;

                        // Append Current Page
                        title.Append(value: pageString);

                        break;
                    case ForumPages.topics:
                        if (this.PageContext.PageForumName != string.Empty)
                        {
                            // Tack on the forum we're viewing
                            title.Append(value: this.CurrentForumPage.HtmlEncode(data: this.PageContext.PageForumName.Truncate(inputLimit: 80)));
                        }

                        addBoardName = false;

                        // Append Current Page
                        title.Append(value: pageString);

                        break;
                    case ForumPages.forum:
                        if (this.PageContext.PageCategoryName != string.Empty)
                        {
                            addBoardName = false;

                            // Tack on the forum we're viewing
                            title.Append(
                                value: this.CurrentForumPage.HtmlEncode(data: this.PageContext.PageCategoryName.Truncate(inputLimit: 80)));
                        }

                        break;
                    default:
                        var pageLinks = this.CurrentForumPage.FindControlAs<PageLinks>(id: "PageLinks");

                        var activePageLink = pageLinks?.PageLinkList?.FirstOrDefault(predicate: link => link.URL.IsNotSet());

                        if (activePageLink != null)
                        {
                            addBoardName = false;

                            // Tack on the forum we're viewing
                            title.Append(value: this.CurrentForumPage.HtmlEncode(data: activePageLink.Title.Truncate(inputLimit: 80)));
                        }

                        break;
                }
            }

            if (addBoardName)
            {
                // and lastly, tack on the board's name
                title.Append(value: this.CurrentForumPage.HtmlEncode(data: this.PageContext.BoardSettings.Name));
            }

            this._forumPageTitle = title.ToString();

            this.ForumControl.FirePageTitleSet(sender: this, e: new ForumPageTitleArgs(title: this._forumPageTitle));
        }

        #endregion
    }
}