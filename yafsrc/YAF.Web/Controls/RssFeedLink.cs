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
namespace YAF.Web.Controls;

/// <summary>
/// The RSS feed link (with optional icon)
/// </summary>
public class RssFeedLink : BaseControl
{
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

        if (!this.PageBoardContext.BoardSettings.ShowAtomLink)
        {
            return;
        }

        writer.BeginRender();

        string url;

        if (this.PageBoardContext.CurrentForumPage.PageType is ForumPages.Topics or ForumPages.Posts)
        {
            url = this.PageBoardContext.CurrentForumPage.PageType is ForumPages.Topics
                      ? this.Get<LinkBuilder>().GetLink(
                          ForumPages.Feed,
                          new
                              {
                                  feed = RssFeeds.Topics.ToInt(),
                                  f = this.PageBoardContext.PageForumID,
                                  name = UrlRewriteHelper.CleanStringForURL(this.PageBoardContext.PageForum.Name)
                              })
                      : this.Get<LinkBuilder>().GetLink(
                          ForumPages.Feed,
                          new
                              {
                                  feed = RssFeeds.Posts.ToInt(),
                                  t = this.PageBoardContext.PageTopicID,
                                  name = UrlRewriteHelper.CleanStringForURL(this.PageBoardContext.PageTopic.TopicName)
                              });
        }
        else
        {
            url = this.Get<LinkBuilder>().GetLink(ForumPages.Feed, new {feed = RssFeeds.LatestPosts.ToInt()});
        }

        new ThemeButton
            {
                Type = ButtonStyle.Warning,
                Size = ButtonSize.Small,
                Icon = "rss-square",
                DataToggle = "tooltip",
                TitleNonLocalized = this.GetText("ATOMFEED"),
                NavigateUrl = url
            }.RenderControl(writer);

        writer.EndRender();
    }

    #endregion
}