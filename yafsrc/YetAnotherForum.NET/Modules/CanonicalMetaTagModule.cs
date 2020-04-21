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
    using System;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;

    using YAF.Types;
    using YAF.Types.Attributes;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    /// <summary>
    ///     Generates a canonical meta tag to fight the dreaded duplicate content SEO warning
    /// </summary>
    [Module("Canonical Meta Tag Module", "BonzoFestoon", 1)]
    public class CanonicalMetaTagModule : SimpleBaseForumModule
    {
        /// <summary>
        /// The initialization after page.
        /// </summary>
        public override void InitAfterPage()
        {
            this.CurrentForumPage.PreRender += this.CurrentForumPage_PreRender;
        }

        /// <summary>
        ///     Handles the PreRender event of the ForumPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void CurrentForumPage_PreRender([NotNull] object sender, [NotNull] EventArgs e)
        {
            const string TopicLinkParams = "t={0}";

            var head = this.ForumControl.Page.Header
                       ?? this.CurrentForumPage.FindControlRecursiveBothAs<HtmlHead>("YafHead");

            if (head == null)
            {
                return;
            }

            // in cases where we are not going to index, but follow, we will not add a canonical tag.
            if (this.ForumPageType == ForumPages.Posts)
            {
                if (this.Get<HttpRequestBase>().QueryString.Exists("m") ||
                    this.Get<HttpRequestBase>().QueryString.Exists("find"))
                {
                    // add no-index tag
                    head.Controls.Add(ControlHelper.MakeMetaNoIndexControl()); 
                }
                else
                {
                    var topicId = this.PageContext.PageTopicID;
                    var topicUrl = BuildLink.GetLink(ForumPages.Posts, true, TopicLinkParams, topicId);

                    head.Controls.Add(new LiteralControl($"<link rel=\"canonical\" href=\"{topicUrl}\" />"));
                }
            }
            else if (this.ForumPageType != ForumPages.Board && this.ForumPageType != ForumPages.Topics)
            {
                // there is not much SEO value to having lists indexed
                // because they change as soon as some adds a new topic
                // or post so don't index them, but follow the links
                // add no-index tag
                head.Controls.Add(ControlHelper.MakeMetaNoIndexControl());
            }
        }
    }
}