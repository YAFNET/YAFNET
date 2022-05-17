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
/// Provides Active Users location info
/// </summary>
public class ActiveLocation : BaseControl
{
    /// <summary>
    ///   Gets or sets the Forum ID of the current location
    /// </summary>
    public int ForumID
    {
        get
        {
            if (this.ViewState["ForumID"] != null)
            {
                return this.ViewState["ForumID"].ToType<int>();
            }

            return -1;
        }

        set => this.ViewState["ForumID"] = value;
    }

    /// <summary>
    ///   Gets or sets the forum name of the current location
    /// </summary>
    public string ForumName
    {
        get => this.ViewState["ForumName"] != null ? this.ViewState["ForumName"].ToString() : string.Empty;

        set => this.ViewState["ForumName"] = value;
    }

    /// <summary>
    /// Gets or sets the location.
    /// </summary>
    public string Location
    {
        get => this.ViewState["Location"] != null ? this.ViewState["Location"].ToString() : string.Empty;

        set => this.ViewState["Location"] = value;
    }

    /// <summary>
    ///   Gets or sets the localization tag for the current location.
    ///   It should be  equal to page name
    /// </summary>
    [NotNull]
    public string ForumPage
    {
        get
        {
            if (this.ViewState["ForumPage"] != null && this.ViewState["ForumPage"] != DBNull.Value)
            {
                return this.ViewState["ForumPage"].ToString();
            }

            return "Board";
        }

        set => this.ViewState["ForumPage"] = value;
    }

    /// <summary>
    ///   Gets or sets a value indicating whether to show only topic link.
    /// </summary>
    public bool HasForumAccess
    {
        get => this.ViewState["HasForumAccess"] == null || Convert.ToBoolean(this.ViewState["HasForumAccess"]);

        set => this.ViewState["HasForumAccess"] = value;
    }

    /// <summary>
    ///   Gets or sets a value indicating whether to show only topic link.
    /// </summary>
    public bool LastLinkOnly
    {
        get => this.ViewState["LastLinkOnly"] != null && Convert.ToBoolean(this.ViewState["LastLinkOnly"]);

        set => this.ViewState["LastLinkOnly"] = value;
    }

    /// <summary>
    ///   Gets or sets the topic id of the current location
    /// </summary>
    public int TopicID
    {
        get
        {
            if (this.ViewState["TopicID"] != null)
            {
                return this.ViewState["TopicID"].ToType<int>();
            }

            return -1;
        }

        set => this.ViewState["TopicID"] = value;
    }

    /// <summary>
    ///   Gets or sets the topic name of the current location
    /// </summary>
    [NotNull]
    public string TopicName
    {
        get => this.ViewState["TopicName"] != null ? this.ViewState["TopicName"].ToString() : string.Empty;

        set => this.ViewState["TopicName"] = value;
    }

    /// <summary>
    ///   Gets or sets the user id of the current user
    /// </summary>
    public int UserID
    {
        get
        {
            if (this.ViewState["UserID"] != null)
            {
                return this.ViewState["UserID"].ToType<int>();
            }

            return -1;
        }

        set => this.ViewState["UserID"] = value;
    }

    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="writer">
    /// The output.
    /// </param>
    protected override void Render([NotNull] HtmlTextWriter writer)
    {
        if (this.ForumPage.IsNotSet())
        {
            this.ForumPage = "Board";
        }

        var forumPageName = this.ForumPage.ToEnum<ForumPages>();
        var forumPageAttributes = this.Location;
        var outText = new StringBuilder();

        writer.BeginRender();

        switch (forumPageName)
        {
            case ForumPages.Board:
                outText.Append(
                    this.ForumPage.Contains("c=")
                        ? this.GetText("ACTIVELOCATION", "FORUMFROMCATEGORY")
                        : this.GetText("ACTIVELOCATION", "MAINPAGE"));
                break;
            case ForumPages.Albums:
                outText.Append(this.RenderAlbumsLocation(forumPageAttributes));
                break;
            case ForumPages.Album:
                outText.Append(this.RenderAlbumLocation(forumPageAttributes));
                break;
            case ForumPages.UserProfile:
                outText.Append(this.RenderProfileLocation(forumPageAttributes));
                break;
            case ForumPages.Topics:
                outText.Append(this.GetText("ACTIVELOCATION", "FORUM"));

                if (this.HasForumAccess)
                {
                    outText.AppendFormat(
                        @"<a href=""{0}"" title=""{1}"" data-bs-toggle=""tooltip"">{2}</a>",
                        this.Get<LinkBuilder>().GetForumLink(this.ForumID, this.ForumName),
                        this.GetText("COMMON", "VIEW_FORUM"),
                        HttpUtility.HtmlEncode(this.ForumName));
                }

                break;
            default:
                if (!BoardContext.Current.IsAdmin && this.ForumPage.ToUpper().Contains("MODERATE_"))
                {
                    // We shouldn't show moderators activity to all users but admins
                    outText.Append(this.GetText("ACTIVELOCATION", "MODERATE"));
                }
                else if (!BoardContext.Current.PageUser.UserFlags.IsHostAdmin &&
                         this.ForumPage.ToUpper().Contains("ADMIN_"))
                {
                    // We shouldn't show admin activity to all users
                    outText.Append(this.GetText("ACTIVELOCATION", "ADMINTASK"));
                }
                else
                {
                    if (this.TopicID > 0 && this.ForumID > 0)
                    {
                        outText.Append(this.RenderTopicsOrForumLocations(forumPageName));
                    }
                    else
                    {
                        // Generic action name based on page name
                        outText.Append(this.GetText("ACTIVELOCATION", this.ForumPage.ToUpper()));
                    }
                }

                break;
        }

        var outputText = outText.ToString();

        writer.Write(outputText);

        writer.EndRender();
    }

    /// <summary>
    /// Gets the User id from query string.
    /// </summary>
    /// <param name="queryString">
    /// The query string.
    /// </param>
    /// <returns>
    /// Returns the User Id
    /// </returns>
    private static int? GetUserIdFromQueryString(string queryString)
    {
        if (ValidationHelper.IsValidInt(queryString))
        {
            return queryString.ToType<int>();
        }

        return null;
    }

    /// <summary>
    /// The render topics or forum locations.
    /// </summary>
    /// <param name="forumPageName">
    /// The forum page name.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    private string RenderTopicsOrForumLocations(ForumPages forumPageName)
    {
        var outText = new StringBuilder();

        // All pages should be processed in call frequency order
        // We are in messages
        switch (forumPageName)
        {
            case ForumPages.Posts:
                outText.Append(this.GetText("ACTIVELOCATION", "POSTS"));
                break;

            case ForumPages.PostMessage:
                outText.Append(this.GetText("ACTIVELOCATION", "POSTMESSAGE_FULL"));
                break;

            case ForumPages.ReportPost:
                outText.Append(this.GetText("ACTIVELOCATION", "REPORTPOST"));
                outText.Append(". ");
                outText.Append(this.GetText("ACTIVELOCATION", "TOPICS"));
                break;

            case ForumPages.MessageHistory:
                outText.Append(this.GetText("ACTIVELOCATION", "MESSAGEHISTORY"));
                outText.Append(". ");
                outText.Append(this.GetText("ACTIVELOCATION", "TOPICS"));
                break;

            default:
                outText.Append(this.GetText("ACTIVELOCATION", "POSTS"));
                break;
        }

        if (!this.HasForumAccess)
        {
            return outText.ToString();
        }

        outText.AppendFormat(
            @"<a href=""{0}"" title=""{1}"" data-bs-toggle=""tooltip"">{2}</a>",
            this.Get<LinkBuilder>().GetTopicLink(this.TopicID, this.TopicName),
            this.GetText("COMMON", "VIEW_TOPIC"),
            HttpUtility.HtmlEncode(this.TopicName));

        if (this.LastLinkOnly)
        {
            return outText.ToString();
        }

        outText.Append(this.GetText("ACTIVELOCATION", "TOPICINFORUM"));
        outText.AppendFormat(
            @"<a href=""{0}"" title=""{1}"" data-bs-toggle=""tooltip"">{2}</a>",
            this.Get<LinkBuilder>().GetForumLink(this.ForumID, this.ForumName),
            this.GetText("COMMON", "VIEW_FORUM"),
            HttpUtility.HtmlEncode(this.ForumName));

        return outText.ToString();
    }

    /// <summary>
    /// A method to get album path string.
    /// </summary>
    /// <param name="forumPageAttributes">
    /// A page query string cleared from page name.
    /// </param>
    /// <returns>
    /// The string
    /// </returns>
    private string RenderAlbumLocation([NotNull] string forumPageAttributes)
    {
        var outString = new StringBuilder();

        var albumID =
            forumPageAttributes.Substring(forumPageAttributes.IndexOf("a=", StringComparison.Ordinal) + 2);

        albumID = albumID.Contains("&")
                      ? albumID.Substring(0, albumID.IndexOf("&", StringComparison.Ordinal)).Trim()
                      : albumID.Substring(0).Trim();

        var userId = GetUserIdFromQueryString(forumPageAttributes);

        if (userId.HasValue && ValidationHelper.IsValidInt(albumID))
        {
            // The DataRow should not be missing in the case
            var userAlbum = this.GetRepository<UserAlbum>().GetById(albumID.Trim().ToType<int>());

            // If album doesn't have a Title, use his ID.
            var albumName = userAlbum.Title.IsNotSet() ? userAlbum.Title : userAlbum.ID.ToString();

            // Render
            if (userId.Value != this.UserID)
            {
                var user = this.GetRepository<User>().GetById(userId.Value);

                outString.Append(this.GetText("ACTIVELOCATION", "ALBUM"));

                outString.AppendFormat(
                    @" <a href=""{0}"">{1}</a> ",
                    this.Get<LinkBuilder>().GetLink(ForumPages.Album, new { a = albumID }),
                    HttpUtility.HtmlEncode(albumName));

                outString.Append(this.GetText("ACTIVELOCATION", "ALBUM_OFUSER"));

                outString.AppendFormat(
                    @" <a href=""{0}"" data-bs-toggle=""tooltip"" title=""{1}"">{2}</a>",
                    this.Get<LinkBuilder>().GetUserProfileLink(userId.Value, user.DisplayOrUserName()),
                    this.GetText("COMMON", "VIEW_USRPROFILE"),
                    HttpUtility.HtmlEncode(user.DisplayOrUserName()));
            }
            else
            {
                outString.Append(this.GetText("ACTIVELOCATION", "ALBUM_OWN"));

                outString.AppendFormat(
                    @"<a href=""{0}"">{1}</a>",
                    this.Get<LinkBuilder>().GetLink(ForumPages.Album, new { a = albumID }),
                    HttpUtility.HtmlEncode(albumName));
            }
        }
        else
        {
            outString.Append(this.GetText("ACTIVELOCATION", "ALBUM"));
        }

        return outString.ToString();
    }

    /// <summary>
    /// A method to get albums path string.
    /// </summary>
    /// <param name="forumPageAttributes">
    /// A page query string cleared from page name.
    /// </param>
    /// <returns>
    /// The string
    /// </returns>
    private string RenderAlbumsLocation([NotNull] string forumPageAttributes)
    {
        var outString = new StringBuilder();

        var userId = GetUserIdFromQueryString(forumPageAttributes);

        if (userId.HasValue)
        {
            if (userId.Value != this.UserID)
            {
                var user = this.GetRepository<User>().GetById(userId.Value);

                outString.Append(this.GetText("ACTIVELOCATION", "ALBUMS_OFUSER"));

                outString.AppendFormat(
                    @" <a href=""{0}"" data-bs-toggle=""tooltip"" title=""{1}"">{2}</a>",
                    this.Get<LinkBuilder>().GetUserProfileLink(userId.Value, user.DisplayOrUserName()),
                    this.GetText("COMMON", "VIEW_USRPROFILE"),
                    HttpUtility.HtmlEncode(user.DisplayOrUserName()));
            }
            else
            {
                outString.Append(this.GetText("ACTIVELOCATION", "ALBUMS_OWN"));
            }
        }
        else
        {
            outString.Append(this.GetTextFormatted("ACTIVELOCATION", "ALBUMS"));
        }

        return outString.ToString();
    }

    /// <summary>
    /// A method to get profile path string.
    /// </summary>
    /// <param name="forumPageAttributes">
    /// The forum page attributes.
    /// </param>
    /// <returns>
    /// The profile.
    /// </returns>
    private string RenderProfileLocation([NotNull] string forumPageAttributes)
    {
        var outString = new StringBuilder();

        var userId = GetUserIdFromQueryString(forumPageAttributes);

        if (userId.HasValue)
        {
            if (userId.Value != this.UserID)
            {
                var user = this.GetRepository<User>().GetById(userId.Value);

                outString.Append(this.GetText("ACTIVELOCATION", "PROFILE_OFUSER"));

                outString.AppendFormat(
                    @" <a href=""{0}"" data-bs-toggle=""tooltip"" title=""{1}"">{2}</a>",
                    this.Get<LinkBuilder>().GetUserProfileLink(userId.Value, user.DisplayOrUserName()),
                    this.GetText("COMMON", "VIEW_USRPROFILE"),
                    HttpUtility.HtmlEncode(user.DisplayOrUserName()));
            }
            else
            {
                outString.Append(this.GetText("ACTIVELOCATION", "PROFILE_OWN"));
            }
        }
        else
        {
            outString.Append(this.GetText("ACTIVELOCATION", "PROFILE"));
        }

        return outString.ToString();
    }
}