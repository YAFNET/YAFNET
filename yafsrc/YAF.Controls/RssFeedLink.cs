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

    using System.Web.UI;

    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The RSS feed link (with optional icon)
    /// </summary>
    public class RssFeedLink : BaseControl
    {
        #region Properties

        /// <summary>
        ///   Gets or sets the additional rss feed url parameters.
        /// </summary>
        public string AdditionalParameters
        {
            get
            {
                return this.ViewState.ToTypeOrDefault("AdditionalParameters", string.Empty);
            }

            set
            {
                this.ViewState["AdditionalParameters"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of the feed. Defaults to "Forum"
        /// </summary>
        /// <value>
        /// The type of the feed.
        /// </value>
        public YafRssFeeds FeedType
        {
            get
            {
                return this.ViewState["FeedType"] != null
                           ? this.ViewState["FeedType"].ToEnum<YafRssFeeds>()
                           : YafRssFeeds.Forum;
            }

            set
            {
                this.ViewState["FeedType"] = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Renders the specified output.
        /// </summary>
        /// <param name="output">The output.</param>
        protected override void Render([NotNull] HtmlTextWriter output)
        {
            if (!this.Visible)
            {
                return;
            }

            output.BeginRender();

            output.WriteBeginTag("div");
            output.WriteAttribute("class", "dropdown");
            output.Write(HtmlTextWriter.TagRightChar);

            // write button
            output.WriteBeginTag("button");
            output.WriteAttribute("class", "btn btn-warning btn-sm dropdown-toggle");
            output.WriteAttribute("id", this.ClientID);
            output.WriteAttribute("data-toggle", "dropdown");
            output.WriteAttribute("aria-haspopup", "true");
            output.WriteAttribute("aria-expanded", "false");
            output.Write(HtmlTextWriter.TagRightChar);

            // icon
            output.WriteBeginTag("i");
            output.WriteAttribute("class", "fa fa-rss-square");
            output.Write(HtmlTextWriter.TagRightChar);
            output.WriteEndTag("i");

            output.WriteEndTag("button");

            // write dropdown
            output.WriteBeginTag("div");
            output.WriteAttribute("class", "dropdown-menu");
            output.WriteAttribute("aria-labelledby", this.ClientID);
            output.Write(HtmlTextWriter.TagRightChar);

            // Render Rss Menu Item
            if (this.PageContext.BoardSettings.ShowRSSLink)
            {
                output.WriteBeginTag("a");
                output.WriteAttribute("class", "dropdown-item");
                output.WriteAttribute(
                    "href",
                    YafBuildLink.GetLink(
                        ForumPages.rsstopic,
                        "pg={0}&ft={1}{2}",
                        this.FeedType.ToInt(),
                        0,
                        this.AdditionalParameters.IsNotSet()
                            ? string.Empty
                            : "&{0}".FormatWith(this.AdditionalParameters)));
                output.Write(HtmlTextWriter.TagRightChar);
                output.Write(@"<i class=""fa fa-rss""></i>&nbsp;{0}", this.GetText("ATOMFEED"));
                output.WriteEndTag("a");
            }

            // Render Atomn Menu Item
            if (this.PageContext.BoardSettings.ShowRSSLink)
            {
                output.WriteBeginTag("a");
                output.WriteAttribute("class", "dropdown-item");
                output.WriteAttribute(
                    "href",
                    YafBuildLink.GetLink(
                        ForumPages.rsstopic,
                        "pg={0}&ft={1}{2}",
                        this.FeedType.ToInt(),
                        1,
                        this.AdditionalParameters.IsNotSet()
                            ? string.Empty
                            : "&{0}".FormatWith(this.AdditionalParameters)));
                output.Write(HtmlTextWriter.TagRightChar);
                output.Write(@"<i class=""fa fa-rss""></i>&nbsp;{0}", this.GetText("RSSFEED"));
                output.WriteEndTag("a");
            }

            output.WriteEndTag("div");

            // write end tag
            output.WriteEndTag("div");

            output.EndRender();
        }

        #endregion
    }
}