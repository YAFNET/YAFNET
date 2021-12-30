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
    using System.Web.UI.HtmlControls;

    using YAF.Core.Helpers;
    using YAF.Types;
    using YAF.Types.Attributes;
    using YAF.Web.EventsArgs;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// Page Title Module
    /// </summary>
    [Module("Page Title Module", "Tiny Gecko", 1)]
    public class PageTitleForumModule : SimpleBaseForumModule
    {
        #region Public Methods

        /// <summary>
        /// The initialization after page.
        /// </summary>
        public override void InitAfterPage()
        {
            this.CurrentForumPage.Load += this.ForumPage_Load;
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
        /// Creates this pages title and fires a PageTitleSet event if one is set
        /// </summary>
        private void GeneratePageTitle()
        {
            var head = this.ForumControl.Page.Header ??
                       this.CurrentForumPage.FindControlRecursiveBothAs<HtmlHead>("YafHead");

            if (head == null)
            {
                return;
            }

            // compute page title..
            var forumPageTitle = this.CurrentForumPage.GeneratePageTitle();

            head.Title = forumPageTitle;

            this.ForumControl.FirePageTitleSet(this, new ForumPageTitleArgs(forumPageTitle));
        }

        #endregion
    }
}