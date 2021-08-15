/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2021 Ingo Herbote
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
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

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
            get => this.ViewState["FeedType"]?.ToString().ToEnum<RssFeeds>() ?? RssFeeds.LatestPosts;

            set => this.ViewState["FeedType"] = value;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Renders the specified output.
        /// </summary>
        /// <param name="writer">The output.</param>
        protected override void Render([NotNull] HtmlTextWriter writer)
        {
            if (!this.Visible)
            {
                return;
            }

            if (!this.PageContext.BoardSettings.ShowAtomLink)
            {
                return;
            }

            writer.BeginRender();

            new ThemeButton
            {
                Type = ButtonStyle.Warning,
                Size = ButtonSize.Small,
                Icon = "rss-square",
                DataToggle = "tooltip",
                TitleNonLocalized = this.GetText("ATOMFEED"),
                NavigateUrl = this.Get<LinkBuilder>().GetLink(
                    ForumPages.Feed,
                    "feed={0}{1}",
                    this.FeedType.ToInt(),
                    this.AdditionalParameters.IsNotSet() ? string.Empty : $"&{this.AdditionalParameters}")
            }.RenderControl(writer);

            writer.EndRender();
        }

        #endregion
    }
}