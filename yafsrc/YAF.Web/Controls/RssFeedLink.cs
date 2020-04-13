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
namespace YAF.Web.Controls
{
    #region Using

    using System.Web.UI;

    using YAF.Core.BaseControls;
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
        ///   Gets or sets the additional RSS feed url parameters.
        /// </summary>
        public string AdditionalParameters
        {
            get => this.ViewState.ToTypeOrDefault("AdditionalParameters", string.Empty);

            set => this.ViewState["AdditionalParameters"] = value;
        }

        /// <summary>
        /// Gets or sets the type of the feed. Defaults to "Forum"
        /// </summary>
        /// <value>
        /// The type of the feed.
        /// </value>
        public RssFeeds FeedType
        {
            get => this.ViewState["FeedType"]?.ToEnum<RssFeeds>() ?? RssFeeds.Forum;

            set => this.ViewState["FeedType"] = value;
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

            if (!this.PageContext.BoardSettings.ShowAtomLink && !this.PageContext.BoardSettings.ShowAtomLink)
            {
                return;
            }

            output.BeginRender();

            // write button
            output.WriteBeginTag("button");
            output.WriteAttribute("class", "btn btn-warning btn-sm dropdown-toggle mb-1");
            output.WriteAttribute("id", this.ClientID);
            output.WriteAttribute("data-toggle", "dropdown");
            output.WriteAttribute("aria-haspopup", "true");
            output.WriteAttribute("aria-expanded", "false");
            output.WriteAttribute("aria-label", "RSS Feed");
            output.Write(HtmlTextWriter.TagRightChar);

            // icon
            new Icon { IconName = "rss-square" }.RenderControl(output);

            output.WriteEndTag("button");

            // write dropdown
            output.WriteBeginTag("div");
            output.WriteAttribute(HtmlTextWriterAttribute.Class.ToString(), "dropdown-menu dropdown-menu-right");
            output.WriteAttribute("aria-labelledby", this.ClientID);
            output.Write(HtmlTextWriter.TagRightChar);

            // Render Rss Menu Item
            if (this.PageContext.BoardSettings.ShowAtomLink)
            {
                new ThemeButton
                    {
                        CssClass = "dropdown-item",
                        Type = ButtonStyle.None,
                        Icon = "rss",
                        Text = this.GetText("ATOMFEED"),
                        NavigateUrl = BuildLink.GetLink(
                            ForumPages.RssTopic,
                            "feed={0}&type={1}{2}",
                            this.FeedType.ToInt(),
                            0,
                            this.AdditionalParameters.IsNotSet() ? string.Empty : $"&{this.AdditionalParameters}")
                    }.RenderControl(output);
            }

            // Render Atom Menu Item
            if (this.PageContext.BoardSettings.ShowRSSLink)
            {
                new ThemeButton
                    {
                        CssClass = "dropdown-item",
                        Type = ButtonStyle.None,
                        Icon = "rss-square",
                        Text = this.GetText("RSSFEED"),
                        NavigateUrl = BuildLink.GetLink(
                            ForumPages.RssTopic,
                            "feed={0}&type={1}{2}",
                            this.FeedType.ToInt(),
                            1,
                            this.AdditionalParameters.IsNotSet()
                                ? string.Empty
                                : $"&{this.AdditionalParameters}")
                }.RenderControl(output);
            }

            output.WriteEndTag("div");

            output.EndRender();
        }

        #endregion
    }
}