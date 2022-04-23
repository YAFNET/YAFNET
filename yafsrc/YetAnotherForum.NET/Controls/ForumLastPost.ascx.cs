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
namespace YAF.Controls;

#region Using

using System.Globalization;

using YAF.Web.Controls;

#endregion

/// <summary>
/// Renders the "Last Post" part of the Forum Topics
/// </summary>
public partial class ForumLastPost : BaseUserControl
{
    #region Properties

    /// <summary>
    /// Gets or sets the data source.
    /// </summary>
    public ForumRead DataSource { get; set; }

    #endregion

    #region Methods

    /// <summary>
    /// Handles the PreRender event
    /// </summary>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (this.DataSource == null)
        {
            return;
        }

        this.PageBoardContext.PageElements.RegisterJsBlockStartup(
            "TopicLinkPopoverJs",
            JavaScriptBlocks.TopicLinkPopoverJs(
                $"{this.GetText("LASTPOST")}&nbsp;{this.GetText("SEARCH", "BY")} ...",
                ".topic-link-popover",
                "focus hover"));

        if (!this.DataSource.ReadAccess)
        {
            this.TopicInPlaceHolder.Visible = false;

            // show "no posts"
            this.LastPostedHolder.Visible = false;
            this.NoPostsPlaceHolder.Visible = true;

            return;
        }

        if (this.DataSource.LastPosted.HasValue)
        {
            this.topicLink.Text = this.Get<IBadWordReplace>()
                .Replace(this.HtmlEncode(this.DataSource.LastTopicName)).Truncate(50);

            // Last Post Date
            var lastPostedDateTime = this.DataSource.LastPosted.Value;

            // Topic Link
            this.topicLink.NavigateUrl = this.Get<LinkBuilder>().GetLink(
                ForumPages.Posts,
                new { t = this.DataSource.LastTopicID, name = this.topicLink.Text });

            var styles = this.PageBoardContext.BoardSettings.UseStyledTopicTitles
                             ? this.Get<IStyleTransform>().Decode(
                                 this.DataSource.LastTopicStyles)
                             : string.Empty;

            if (styles.IsSet())
            {
                this.topicLink.Attributes.Add("style", styles);
            }

            // Last Topic User
            var lastUserLink = new UserLink
                                   {
                                       Suspended = this.DataSource.LastUserSuspended,
                                       UserID = this.DataSource.LastUserID.Value,
                                       IsGuest = true,
                                       Style = this.PageBoardContext.BoardSettings.UseStyledNicks && this.DataSource.Style.IsSet()
                                                   ? this.Get<IStyleTransform>().Decode(
                                                       this.DataSource.Style)
                                                   : string.Empty,
                                       ReplaceName = this.PageBoardContext.BoardSettings.EnableDisplayName
                                                         ? this.DataSource.LastUserDisplayName
                                                         : this.DataSource.LastUser
                                   };

            var lastRead = this.Get<IReadTrackCurrentUser>().GetForumTopicRead(
                this.DataSource.ForumID,
                this.DataSource.LastTopicID,
                this.DataSource.LastForumAccess ?? DateTimeHelper.SqlDbMinTime(),
                this.DataSource.LastTopicAccess ?? DateTimeHelper.SqlDbMinTime());

            var formattedDatetime = this.PageBoardContext.BoardSettings.ShowRelativeTime
                                        ? lastPostedDateTime.ToRelativeTime()
                                        : this.Get<IDateTimeService>().Format(
                                            DateTimeFormat.BothTopic,
                                            lastPostedDateTime);

            this.Info.DataContent = $@"
                          {lastUserLink.RenderToString()}
                          <span class=""fa-stack"">
                                                    <i class=""fa fa-calendar-day fa-stack-1x text-secondary""></i>
                                                    <i class=""fa fa-circle fa-badge-bg fa-inverse fa-outline-inverse""></i>
                                                    <i class=""fa fa-clock fa-badge text-secondary""></i>
                                                </span>&nbsp;{formattedDatetime}
                         ";

            this.Info.Text = string.Format(
                this.GetText("Default", "BY"),
                this.PageBoardContext.BoardSettings.EnableDisplayName ? this.DataSource.LastUserDisplayName : this.DataSource.LastUser);

            if (this.DataSource.LastPosted.Value > lastRead)
            {
                this.NewMessage.Visible = true;
                this.NewMessage.Text = $" <span class=\"badge bg-success\">{this.GetText("NEW_POSTS")}</span>";
            }
            else
            {
                this.NewMessage.Visible = false;
            }

            this.LastPostedHolder.Visible = true;
            this.NoPostsPlaceHolder.Visible = false;
        }
        else
        {
            // show "no posts"
            this.LastPostedHolder.Visible = false;
            this.NoPostsPlaceHolder.Visible = true;
        }
    }
    #endregion
}