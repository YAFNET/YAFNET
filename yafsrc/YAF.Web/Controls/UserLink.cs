﻿/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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
///     Provides a basic "profile link" for a YAF User
/// </summary>
public class UserLink : UserLabel
{
    /// <summary>
    ///     Gets or sets a value indicating whether
    ///     Make the link target "blank" to open in a new window.
    /// </summary>
    public bool BlankTarget
    {
        get => this.ViewState["BlankTarget"] != null && Convert.ToBoolean(this.ViewState["BlankTarget"]);

        set => this.ViewState["BlankTarget"] = value;
    }

    /// <summary>
    ///     Gets or sets a value indicating whether [enable hover card].
    /// </summary>
    /// <value>
    ///     <c>true</c> if [enable hover card]; otherwise, <c>false</c>.
    /// </value>
    public bool EnableHoverCard
    {
        get => this.ViewState["EnableHoverCard"] == null || this.ViewState["EnableHoverCard"].ToType<bool>();

        set => this.ViewState["EnableHoverCard"] = value;
    }

    /// <summary>
    ///     Gets or sets a value indicating whether
    ///     User Is a Guest.
    /// </summary>
    public bool IsGuest
    {
        get => this.ViewState["IsGuest"] != null && Convert.ToBoolean(this.ViewState["IsGuest"]);

        set => this.ViewState["IsGuest"] = value;
    }

    /// <summary>
    /// Gets or sets the suspended.
    /// </summary>
    public DateTime? Suspended
    {
        get => this.ViewState["Suspended"] as DateTime?;

        set => this.ViewState["Suspended"] = value;
    }

    /// <summary>
    /// Gets a value indicating whether the user has Profile View Permission
    /// </summary>
    /// <value>
    /// <c>true</c> if the user can view profiles; otherwise, <c>false</c>.
    /// </value>
    private bool CanViewProfile => this.Get<IPermissions>().Check(this.PageBoardContext.BoardSettings.ProfileViewPermissions);

    /// <summary>
    /// Gets a value indicating whether is hover card enabled.
    /// </summary>
    private bool IsHoverCardEnabled =>
        this.PageBoardContext.BoardSettings.EnableUserInfoHoverCards && this.EnableHoverCard
                                                                     && BoardContext.Current.CurrentForumPage != null;

    /// <summary>
    /// The On PreRender event.
    /// </summary>
    /// <param name="e">
    /// the Event Arguments
    /// </param>
    override protected void OnPreRender(EventArgs e)
    {
        if (!this.CanViewProfile || !this.IsHoverCardEnabled)
        {
            return;
        }

        // Setup Hover Card JS
        BoardContext.Current.PageElements.RegisterJsBlockStartup(
            nameof(JavaScriptBlocks.HoverCardJs),
            JavaScriptBlocks.HoverCardJs());
    }

    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    override protected void Render(HtmlTextWriter writer)
    {
        var displayName = this.ReplaceName;

        if (this.UserID == -1)
        {
            return;
        }

        var userSuspended = this.Suspended;

        writer.BeginRender();

        var isCrawler = this.CrawlerName.IsSet();

        if (isCrawler)
        {
            this.IsGuest = true;
        }

        if (!this.IsGuest)
        {
            writer.WriteBeginTag("a");

            writer.WriteAttribute("href", this.Get<LinkBuilder>().GetUserProfileLink(this.UserID, displayName));

            var cssClass = new StringBuilder();

            cssClass.Append("btn-sm");

            if (this.CanViewProfile && this.IsHoverCardEnabled)
            {
                cssClass.Append(" hc-user");

                writer.WriteAttribute(
                    "data-hovercard",
                    $"{(Config.IsDotNetNuke ? $"{BaseUrlBuilder.GetBaseUrlFromVariables()}{BaseUrlBuilder.AppPath}" : BoardInfo.ForumClientFileRoot)}resource.ashx?userinfo={this.UserID}&boardId={BoardContext.Current.PageBoardID}&type=json");
            }
            else
            {
                writer.WriteAttribute("title", this.GetText("COMMON", "VIEW_USRPROFILE"));
            }

            this.CssClass = cssClass.ToString();

            if (this.PageBoardContext.BoardSettings.UseNoFollowLinks)
            {
                writer.WriteAttribute("rel", "nofollow");
            }

            if (this.BlankTarget)
            {
                writer.WriteAttribute("target", "_blank");
            }
        }
        else
        {
            writer.WriteBeginTag("span");
        }

        this.RenderMainTagAttributes(writer);

        writer.Write(HtmlTextWriter.TagRightChar);

        // show online icon
        if (/*this.PageBoardContext.BoardSettings.ShowUserOnlineStatus &&*/ !isCrawler)
        {
            var onlineStatusIcon = new OnlineStatusIcon { UserId = this.UserID, Suspended = userSuspended };

            onlineStatusIcon.RenderControl(writer);
        }

        if (isCrawler)
        {
            var icon = new Icon { IconName = "robot" };

            icon.RenderControl(writer);
        }

        // Replace Name with Crawler Name if Set, otherwise use regular display name or Replace Name if set
        if (this.CrawlerName.IsSet())
        {
            writer.WriteEncodedText(this.CrawlerName);
        }
        else if (this.CrawlerName.IsNotSet() && this.ReplaceName.IsSet() && this.IsGuest)
        {
            writer.WriteEncodedText(this.ReplaceName);
        }
        else
        {
            writer.WriteEncodedText(displayName);
        }

        if (this.PostfixText.IsSet())
        {
            writer.Write(this.PostfixText);
        }

        writer.WriteEndTag(!this.IsGuest ? "a" : "span");

        writer.EndRender();
    }
}