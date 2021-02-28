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

    using System;
    using System.Text;
    using System.Web;
    using System.Web.UI;

    using YAF.Configuration;

    using YAF.Core.BaseControls;
    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Core.Services;
    using YAF.Core.Utilities.Helpers;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;

    #endregion

    /// <summary>
    /// Provides Active Users location info
    /// </summary>
    public class ActiveLocation : BaseControl
    {
        #region Properties

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
        [NotNull]
        public string ForumName
        {
            get => this.ViewState["ForumName"] != null ? this.ViewState["ForumName"].ToString() : string.Empty;

            set => this.ViewState["ForumName"] = value;
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
                if (this.ViewState["ForumPage"] != null || this.ViewState["ForumPage"] != DBNull.Value)
                {
                    return this.ViewState["ForumPage"].ToString();
                }

                return "MAINPAGE";
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
        ///   Gets or sets the UserName of the current user
        /// </summary>
        [NotNull]
        public string UserName
        {
            get => this.ViewState["UserName"] != null ? this.ViewState["UserName"].ToString() : string.Empty;

            set => this.ViewState["UserName"] = value;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The render.
        /// </summary>
        /// <param name="output">
        /// The output.
        /// </param>
        protected override void Render([NotNull] HtmlTextWriter output)
        {
            var forumPageName = this.ForumPage;
            string forumPageAttributes = null;
            var outText = new StringBuilder();

            // Find a user page name. If it's missing we are very probably on the start page 
            if (forumPageName.IsNotSet())
            {
                forumPageName = "MAINPAGE";
            }
            else
            {
                // We find here a page name start position
                if (forumPageName.Contains("g="))
                {
                    forumPageName = forumPageName.Substring(forumPageName.IndexOf("g=", StringComparison.Ordinal) + 2);

                    // We find here a page name end position
                    if (forumPageName.Contains("&"))
                    {
                        forumPageAttributes =
                            forumPageName.Substring(forumPageName.IndexOf("&", StringComparison.Ordinal) + 1);
                        forumPageName = forumPageName.Substring(
                            0,
                            forumPageName.IndexOf("&", StringComparison.Ordinal));
                    }
                }
                else
                {
                    if (Config.IsDotNetNuke)
                    {
                        var indexOf = forumPageName.IndexOf("&", StringComparison.Ordinal);
                        forumPageName = forumPageName.Substring(indexOf + 1);
                    }

                    var idx = forumPageName.IndexOf("=", StringComparison.Ordinal);
                    if (idx > 0)
                    {
                        forumPageAttributes = forumPageName.Substring(
                            0,
                            forumPageName.IndexOf("&", StringComparison.Ordinal) > 0
                                ? forumPageName.IndexOf("&", StringComparison.Ordinal)
                                : forumPageName.Length - 1);

                        forumPageName = forumPageName.Substring(0, idx);
                    }
                }
            }

            output.BeginRender();

            // All pages should be processed in call frequency order 
            // We are in messages
            if (this.TopicID > 0 && this.ForumID > 0)
            {
                switch (forumPageName)
                {
                    case "topics":
                        outText.Append(this.GetText("ACTIVELOCATION", "TOPICS"));
                        break;

                    case "posts":
                        outText.Append(this.GetText("ACTIVELOCATION", "POSTS"));
                        break;

                    case "postmessage":
                        outText.Append(this.GetText("ACTIVELOCATION", "POSTMESSAGE_FULL"));
                        break;

                    case "reportpost":
                        outText.Append(this.GetText("ACTIVELOCATION", "REPORTPOST"));
                        outText.Append(". ");
                        outText.Append(this.GetText("ACTIVELOCATION", "TOPICS"));
                        break;

                    case "messagehistory":
                        outText.Append(this.GetText("ACTIVELOCATION", "MESSAGEHISTORY"));
                        outText.Append(". ");
                        outText.Append(this.GetText("ACTIVELOCATION", "TOPICS"));
                        break;

                    default:
                        outText.Append(this.GetText("ACTIVELOCATION", "POSTS"));
                        break;
                }

                if (this.HasForumAccess)
                {
                    outText.AppendFormat(
                        @"<a href=""{0}"" title=""{1}"" data-bs-toggle=""tooltip"">{2}</a>",
                        this.Get<LinkBuilder>().GetTopicLink(this.TopicID, this.TopicName),
                        this.GetText("COMMON", "VIEW_TOPIC"),
                        HttpUtility.HtmlEncode(this.TopicName));

                    if (!this.LastLinkOnly)
                    {
                        outText.Append(this.GetText("ACTIVELOCATION", "TOPICINFORUM"));
                        outText.AppendFormat(
                            @"<a href=""{0}"" title=""{1}"" data-bs-toggle=""tooltip"">{2}</a>",
                            this.Get<LinkBuilder>().GetForumLink(this.ForumID, this.ForumName),
                            this.GetText("COMMON", "VIEW_FORUM"),
                            HttpUtility.HtmlEncode(this.ForumName));
                    }
                }
            }
            else if (this.ForumID > 0 && this.TopicID <= 0)
            {
                // User views a forum
                if (forumPageName == "Topics")
                {
                    outText.Append(this.GetText("ACTIVELOCATION", "FORUM"));

                    if (this.HasForumAccess)
                    {
                        outText.AppendFormat(
                            @"<a href=""{0}"" title=""{1}"" data-bs-toggle=""tooltip"">{2}</a>",
                            this.Get<LinkBuilder>().GetForumLink(this.ForumID, this.ForumName),
                            this.GetText("COMMON", "VIEW_FORUM"),
                            HttpUtility.HtmlEncode(this.ForumName));
                    }
                }
            }
            else
            {
                // First specially treated pages where we can render
                // an info about user name, etc. 
                switch (forumPageName)
                {
                    case "Profile":
                        outText.Append(this.Profile(forumPageAttributes, this.UserName));
                        break;
                    case "Albums":
                        outText.Append(this.Albums(forumPageAttributes, this.UserName));
                        break;
                    case "Album":
                        outText.Append(this.Album(forumPageAttributes, this.UserName));
                        break;
                    default:
                        if (forumPageName == "Board" && this.TopicID <= 0 && this.ForumID <= 0)
                        {
                            outText.Append(
                                this.ForumPage.Contains("c=")
                                    ? this.GetText("ACTIVELOCATION", "FORUMFROMCATEGORY")
                                    : this.GetText("ACTIVELOCATION", "MAINPAGE"));
                        }
                        else if (!BoardContext.Current.IsAdmin && forumPageName.ToUpper().Contains("MODERATE_"))
                        {
                            // We shouldn't show moderators activity to all users but admins
                            outText.Append(this.GetText("ACTIVELOCATION", "MODERATE"));
                        }
                        else if (!BoardContext.Current.User.UserFlags.IsHostAdmin &&
                                 forumPageName.ToUpper().Contains("ADMIN_"))
                        {
                            // We shouldn't show admin activity to all users 
                            outText.Append(this.GetText("ACTIVELOCATION", "ADMINTASK"));
                        }
                        else
                        {
                            // Generic action name based on page name
                            outText.Append(this.GetText("ACTIVELOCATION", forumPageName.ToUpper()));
                        }

                        break;
                }
            }

            var outputText = outText.ToString();

            if (outputText.Contains("ACTIVELOCATION") || outputText.Trim().IsNotSet() ||
                forumPageName.IndexOf("p=", StringComparison.Ordinal) == 0)
            {
                if (forumPageName.Contains("p="))
                {
                    outText.AppendFormat("{0}.", this.GetText("ACTIVELOCATION", "NODATA"));
                }
                else
                {
                    if (this.PageContext.BoardSettings.EnableActiveLocationErrorsLog)
                    {
                        this.Logger.Log(
                            this.UserID,
                            this,
                            $"Incorrect active location string: ForumID = {this.ForumID};ForumName= {this.ForumName}; ForumPage={this.ForumPage}; TopicID={this.TopicID}; TopicName={this.TopicName}; UserID={this.UserID}; UserName={this.UserName}; Attributes={forumPageAttributes}; ForumPageName={forumPageName}; URL={this.Get<HttpRequestBase>().Url.AbsoluteUri}");
                    }

                    outputText = this.GetText("ACTIVELOCATION", "NODATA");
                }
            }

            output.Write(outputText);

            output.EndRender();
        }

        /// <summary>
        /// A method to get album path string.
        /// </summary>
        /// <param name="forumPageAttributes">
        /// A page query string cleared from page name.
        /// </param>
        /// <param name="userOrDisplayName">
        /// The user Or Display Name.
        /// </param>
        /// <returns>
        /// The string
        /// </returns>
        private string Album([NotNull] string forumPageAttributes, [NotNull] string userOrDisplayName)
        {
            var outString = new StringBuilder();
            var userID = forumPageAttributes.Substring(forumPageAttributes.IndexOf("u=", StringComparison.Ordinal) + 2)
                .Trim();

            if (userID.Contains("&"))
            {
                userID = userID.Substring(0, userID.IndexOf("&", StringComparison.Ordinal)).Trim();
            }

            var albumID =
                forumPageAttributes.Substring(forumPageAttributes.IndexOf("a=", StringComparison.Ordinal) + 2);

            albumID = albumID.Contains("&")
                ? albumID.Substring(0, albumID.IndexOf("&", StringComparison.Ordinal)).Trim()
                : albumID.Substring(0).Trim();

            if (ValidationHelper.IsValidInt(userID) && ValidationHelper.IsValidInt(albumID))
            {
                var userId = userID.ToType<int>();

                // The DataRow should not be missing in the case
                var userAlbum = this.GetRepository<UserAlbum>().GetById(albumID.Trim().ToType<int>());

                // If album doesn't have a Title, use his ID.
                var albumName = userAlbum.Title.IsNotSet() ? userAlbum.Title : userAlbum.ID.ToString();

                // Render
                if (userId != this.UserID)
                {
                    outString.Append(this.GetText("ACTIVELOCATION", "ALBUM"));

                    outString.AppendFormat(
                        @"<a href=""{0}"">{1}</a>",
                        this.Get<LinkBuilder>().GetLink(ForumPages.Album, "a={0}", albumID),
                        HttpUtility.HtmlEncode(albumName));
                    outString.Append(this.GetText("ACTIVELOCATION", "ALBUM_OFUSER"));

                    outString.AppendFormat(
                        @"<a href=""{0}"" data-bs-toggle=""tooltip"" title=""{1}"">{2}</a>",
                        this.Get<LinkBuilder>().GetUserProfileLink(userId, userOrDisplayName),
                        this.GetText("COMMON", "VIEW_USRPROFILE"),
                        HttpUtility.HtmlEncode(userOrDisplayName));
                }
                else
                {
                    outString.Append(this.GetText("ACTIVELOCATION", "ALBUM_OWN"));

                    outString.AppendFormat(
                        @"<a href=""{0}"">{1}</a>",
                        this.Get<LinkBuilder>().GetLink(ForumPages.Album, "a={0}", albumID),
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
        /// <param name="userOrDisplayName">
        /// The user Or Display Name.
        /// </param>
        /// <returns>
        /// The string
        /// </returns>
        private string Albums([NotNull] string forumPageAttributes, [NotNull] string userOrDisplayName)
        {
            var outString = new StringBuilder();

            var userID = forumPageAttributes.Substring(forumPageAttributes.IndexOf("u=", StringComparison.Ordinal) + 2)
                .Substring(0).Trim();

            if (ValidationHelper.IsValidInt(userID))
            {
                var userId = userID.ToType<int>();

                if (userId.ToType<int>() == this.UserID)
                {
                    outString.Append(this.GetText("ACTIVELOCATION", "ALBUMS_OWN"));
                }
                else
                {
                    outString.Append(this.GetText("ACTIVELOCATION", "ALBUMS_OFUSER"));

                    outString.AppendFormat(
                        @"<a href=""{0}"" data-bs-toggle=""tooltip"" title=""{1}"">{2}</a>",
                        this.Get<LinkBuilder>().GetUserProfileLink(userId, userOrDisplayName),
                        this.GetText("COMMON", "VIEW_USRPROFILE"),
                        HttpUtility.HtmlEncode(userOrDisplayName));
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
        /// <param name="userOrDisplayName">
        /// The user Or Display Name.
        /// </param>
        /// <returns>
        /// The profile.
        /// </returns>
        private string Profile([NotNull] string forumPageAttributes, [NotNull] string userOrDisplayName)
        {
            var outString = new StringBuilder();
            var userID = forumPageAttributes.Substring(forumPageAttributes.IndexOf("u=", StringComparison.Ordinal) + 2);

            userID = userID.Contains("&")
                ? userID.Substring(0, userID.IndexOf("&", StringComparison.Ordinal)).Trim()
                : userID.Substring(0).Trim();

            if (ValidationHelper.IsValidInt(userID.Trim()))
            {
                var userId = userID.ToType<int>();

                if (userId != this.UserID)
                {
                    outString.Append(this.GetText("ACTIVELOCATION", "PROFILE_OFUSER"));

                    outString.AppendFormat(
                        @"<a href=""{0}"" data-bs-toggle=""tooltip"" title=""{1}"">{2}</a>",
                        this.Get<LinkBuilder>().GetUserProfileLink(userId, userOrDisplayName),
                        this.GetText("COMMON", "VIEW_USRPROFILE"),
                        HttpUtility.HtmlEncode(userOrDisplayName));
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

        #endregion
    }
}