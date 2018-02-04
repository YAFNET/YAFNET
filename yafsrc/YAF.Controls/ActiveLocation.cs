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

    using System;
    using System.Text;
    using System.Web;
    using System.Web.UI;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// Provides Active Users location info
    /// </summary>
    public class ActiveLocation : BaseControl
    {
        #region Properties

        /// <summary>
        ///   Gets or sets the forumid of the current location
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

            set
            {
                this.ViewState["ForumID"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets the forumname of the current location
        /// </summary>
        [NotNull]
        public string ForumName
        {
            get
            {
                return this.ViewState["ForumName"] != null ? this.ViewState["ForumName"].ToString() : string.Empty;
            }

            set
            {
                this.ViewState["ForumName"] = value;
            }
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
                    // string localizedPage = ViewState["ForumPage"].ToString().Substring(ViewState["ForumPage"].ToString().IndexOf("default.aspx?") - 14, ViewState["ForumPage"].ToString().IndexOf("&"));
                    return this.ViewState["ForumPage"].ToString();
                }

                return "MAINPAGE";
            }

            set
            {
                this.ViewState["ForumPage"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether to show only topic link.
        /// </summary>
        public bool HasForumAccess
        {
            get
            {
                return this.ViewState["HasForumAccess"] == null || Convert.ToBoolean(this.ViewState["HasForumAccess"]);
            }

            set
            {
                this.ViewState["HasForumAccess"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether to show only topic link.
        /// </summary>
        public bool LastLinkOnly
        {
            get
            {
                return this.ViewState["LastLinkOnly"] != null && Convert.ToBoolean(this.ViewState["LastLinkOnly"]);
            }

            set
            {
                this.ViewState["LastLinkOnly"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets the topicid of the current location
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

            set
            {
                this.ViewState["TopicID"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets the topicname of the current location
        /// </summary>
        [NotNull]
        public string TopicName
        {
            get
            {
                return this.ViewState["TopicName"] != null ? this.ViewState["TopicName"].ToString() : string.Empty;
            }

            set
            {
                this.ViewState["TopicName"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets the userid of the current user
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

            set
            {
                this.ViewState["UserID"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets the UserName of the current user
        /// </summary>
        [NotNull]
        public string UserName
        {
            get
            {
                return this.ViewState["UserName"] != null ? this.ViewState["UserName"].ToString() : string.Empty;
            }

            set
            {
                this.ViewState["UserName"] = value;
            }
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
            if (string.IsNullOrEmpty(forumPageName))
            {
                forumPageName = "MAINPAGE";
            }
            else
            {
                // We find here a page name start position
                if (forumPageName.Contains("g="))
                {
                    forumPageName = forumPageName.Substring(forumPageName.IndexOf("g=") + 2);

                    // We find here a page name end position
                    if (forumPageName.Contains("&"))
                    {
                        forumPageAttributes = forumPageName.Substring(forumPageName.IndexOf("&") + 1);
                        forumPageName = forumPageName.Substring(0, forumPageName.IndexOf("&"));
                    }
                }
                else
                {
                    if (Config.IsDotNetNuke)
                    {
                        var idxfrst = forumPageName.IndexOf("&");
                        forumPageName = forumPageName.Substring(idxfrst + 1);
                    }

                    var idx = forumPageName.IndexOf("=");
                    if (idx > 0)
                    {
                        forumPageAttributes = forumPageName.Substring(
                            0, forumPageName.IndexOf("&") > 0 ? forumPageName.IndexOf("&") : forumPageName.Length - 1);
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
                    outText.Append(
                        @"<a href=""{0}"" id=""topicid_{1}""  title=""{2}"" runat=""server""> {3} </a>".FormatWith(
                            YafBuildLink.GetLink(ForumPages.posts, "t={0}", this.TopicID),
                            this.UserID,
                            this.GetText("COMMON", "VIEW_TOPIC"),
                            HttpUtility.HtmlEncode(this.TopicName)));
                    if (!this.LastLinkOnly)
                    {
                        outText.Append(this.GetText("ACTIVELOCATION", "TOPICINFORUM"));
                        outText.Append(
                            @"<a href=""{0}"" id=""forumidtopic_{1}"" title=""{2}"" runat=""server""> {3} </a>".FormatWith(
                                    YafBuildLink.GetLink(ForumPages.topics, "f={0}", this.ForumID),
                                    this.UserID,
                                    this.GetText("COMMON", "VIEW_FORUM"),
                                    HttpUtility.HtmlEncode(this.ForumName)));
                    }
                }
            }
            else if (this.ForumID > 0 && this.TopicID <= 0)
            {
                // User views a forum
                if (forumPageName == "topics")
                {
                    outText.Append(this.GetText("ACTIVELOCATION", "FORUM"));

                    if (this.HasForumAccess)
                    {
                        outText.Append(
                            @"<a href=""{0}"" id=""forumid_{1}"" title=""{2}"" runat=""server""> {3} </a>".FormatWith(
                                YafBuildLink.GetLink(ForumPages.topics, "f={0}", this.ForumID),
                                this.UserID,
                                this.GetText("COMMON", "VIEW_FORUM"),
                                HttpUtility.HtmlEncode(this.ForumName)));
                    }
                }
            }
            else
            {
                // First specially treated pages where we can render
                // an info about user name, etc. 
                switch (forumPageName)
                {
                    case "profile":
                        outText.Append(this.Profile(forumPageAttributes));
                        break;
                    case "albums":
                        outText.Append(this.Albums(forumPageAttributes));
                        break;
                    case "album":
                        outText.Append(this.Album(forumPageAttributes));
                        break;
                    default:
                        if (forumPageName == "forum" && this.TopicID <= 0 && this.ForumID <= 0)
                        {
                            outText.Append(
                                this.ForumPage.Contains("c=")
                                    ? this.GetText("ACTIVELOCATION", "FORUMFROMCATEGORY")
                                    : this.GetText("ACTIVELOCATION", "MAINPAGE"));
                        }
                        else if (!YafContext.Current.IsAdmin && forumPageName.ToUpper().Contains("MODERATE_"))
                        {
                            // We shouldn't show moderators activity to all users but admins
                            outText.Append(this.GetText("ACTIVELOCATION", "MODERATE"));
                        }
                        else if (!YafContext.Current.IsHostAdmin && forumPageName.ToUpper().Contains("ADMIN_"))
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

            if (outputText.Contains("ACTIVELOCATION") || string.IsNullOrEmpty(outputText.Trim())
                || forumPageName.IndexOf("p=", StringComparison.Ordinal) == 0)
            {
                if (forumPageName.Contains("p="))
                {
                    outText.AppendFormat("{0}.", this.GetText("ACTIVELOCATION", "NODATA"));
                }
                else
                {
                    if (this.Get<YafBoardSettings>().EnableActiveLocationErrorsLog)
                    {
                        this.Logger.Log(
                            this.UserID,
                            this,
                            "Incorrect active location string: ForumID = {0}; ForumName= {1}; ForumPage={2}; TopicID={3}; TopicName={4}; UserID={5}; UserName={6}; Attributes={7}; ForumPageName={8}; URL={9}"
                                .FormatWith(
                                    this.ForumID,
                                    this.ForumName,
                                    this.ForumPage,
                                    this.TopicID,
                                    this.TopicName,
                                    this.UserID,
                                    this.UserName,
                                    forumPageAttributes,
                                    forumPageName,
                                    this.Get<HttpRequestBase>().Url.AbsoluteUri));
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
        /// <returns>
        /// The string
        /// </returns>
        private string Album([NotNull] string forumPageAttributes)
        {
            var outstring = string.Empty;
            var userID = forumPageAttributes.Substring(forumPageAttributes.IndexOf("u=", StringComparison.Ordinal) + 2).Trim();

            if (userID.Contains("&"))
            {
                userID = userID.Substring(0, userID.IndexOf("&", StringComparison.Ordinal)).Trim();
            }

            var albumID = forumPageAttributes.Substring(forumPageAttributes.IndexOf("a=", StringComparison.Ordinal) + 2);

            albumID = albumID.Contains("&")
                          ? albumID.Substring(0, albumID.IndexOf("&", StringComparison.Ordinal)).Trim()
                          : albumID.Substring(0).Trim();

            if (ValidationHelper.IsValidInt(userID) && ValidationHelper.IsValidInt(albumID))
            {
                // The DataRow should not be missing in the case
                var dr = LegacyDb.album_list(null, albumID.Trim().ToType<int>()).Rows[0];

                // If album doesn't have a Title, use his ID.
                var albumName = !string.IsNullOrEmpty(dr["Title"].ToString())
                                       ? dr["Title"].ToString()
                                       : dr["AlbumID"].ToString();

                // Render
                if (userID.ToType<int>() != this.UserID)
                {
                    var displayName =
                        HttpUtility.HtmlEncode(UserMembershipHelper.GetDisplayNameFromID(userID.ToType<long>()));

                    if (displayName.IsNotSet())
                    {
                        displayName = HttpUtility.HtmlEncode(UserMembershipHelper.GetUserNameFromID(userID.ToType<long>()));
                    } 
                    
                    outstring += this.GetText("ACTIVELOCATION", "ALBUM").FormatWith();
                    outstring +=
                        @"<a href=""{0}"" id=""uiseralbumid_{1}"" runat=""server""> {2} </a>".FormatWith(
                            YafBuildLink.GetLink(ForumPages.album, "a={0}", albumID),
                            userID + this.PageContext.PageUserID,
                            HttpUtility.HtmlEncode(albumName));
                    outstring += this.GetText("ACTIVELOCATION", "ALBUM_OFUSER").FormatWith();
                    outstring +=
                        @"<a href=""{0}"" id=""albumuserid_{1}"" runat=""server""> {2} </a>".FormatWith(
                            YafBuildLink.GetLink(ForumPages.profile, "u={0}&name={1}", userID, displayName),
                            userID,
                            HttpUtility.HtmlEncode(displayName));
                }
                else
                {
                    outstring += this.GetText("ACTIVELOCATION", "ALBUM_OWN").FormatWith();
                    outstring +=
                        @"<a href=""{0}"" id=""uiseralbumid_{1}"" runat=""server""> {2} </a>".FormatWith(
                            YafBuildLink.GetLink(ForumPages.album, "a={0}", albumID),
                            userID + this.PageContext.PageUserID,
                            HttpUtility.HtmlEncode(albumName));
                }
            }
            else
            {
                outstring += this.GetText("ACTIVELOCATION", "ALBUM").FormatWith();
            }

            return outstring;
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
        private string Albums([NotNull] string forumPageAttributes)
        {
            string outstring;

            var userId = forumPageAttributes.Substring(forumPageAttributes.IndexOf("u=", StringComparison.Ordinal) + 2).Substring(0).Trim();

            if (ValidationHelper.IsValidInt(userId))
            {
                if (userId.ToType<int>() == this.UserID)
                {
                    outstring = this.GetText("ACTIVELOCATION", "ALBUMS_OWN").FormatWith();
                }
                else
                {
                    var displayName =
                        HttpUtility.HtmlEncode(UserMembershipHelper.GetDisplayNameFromID(userId.ToType<long>()));

                    if (displayName.IsNotSet())
                    {
                        displayName = HttpUtility.HtmlEncode(UserMembershipHelper.GetUserNameFromID(userId.ToType<long>()));
                    }

                    outstring =
                        @"{3}<a href=""{0}"" id=""albumsuserid_{1}"" runat=""server""> {2} </a>".FormatWith(
                            YafBuildLink.GetLink(ForumPages.profile, "u={0}&name={1}", userId, displayName),
                            userId + this.PageContext.PageUserID,
                            HttpUtility.HtmlEncode(displayName),
                            this.GetText("ACTIVELOCATION", "ALBUMS_OFUSER"));
                }
            }
            else
            {
                outstring = this.GetTextFormatted("ACTIVELOCATION", "ALBUMS").FormatWith();
            }

            return outstring;
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
        private string Profile([NotNull] string forumPageAttributes)
        {
            var outstring = string.Empty;
            var userId = forumPageAttributes.Substring(forumPageAttributes.IndexOf("u=", StringComparison.Ordinal) + 2);

            userId = userId.Contains("&") ? userId.Substring(0, userId.IndexOf("&", StringComparison.Ordinal)).Trim() : userId.Substring(0).Trim();

            if (ValidationHelper.IsValidInt(userId.Trim()))
            {
                if (userId.ToType<int>() != this.UserID)
                {
                    var displayName =
                        HttpUtility.HtmlEncode(UserMembershipHelper.GetDisplayNameFromID(userId.ToType<long>()));
                    
                    if (displayName.IsNotSet())
                    {
                        displayName = HttpUtility.HtmlEncode(UserMembershipHelper.GetUserNameFromID(userId.ToType<long>()));
                    }

                    outstring += this.GetText("ACTIVELOCATION", "PROFILE_OFUSER").FormatWith();
                    outstring +=
                        @"<a href=""{0}""  id=""profileuserid_{1}"" title=""{2}"" alt=""{2}"" runat=""server""> {3} </a>"
                            .FormatWith(
                                YafBuildLink.GetLink(ForumPages.profile, "u={0}&name={1}", userId, displayName),
                                userId + this.PageContext.PageUserID,
                                this.GetText("COMMON", "VIEW_USRPROFILE"),
                                HttpUtility.HtmlEncode(displayName));
                }
                else
                {
                    outstring += this.GetText("ACTIVELOCATION", "PROFILE_OWN").FormatWith();
                }
            }
            else
            {
                outstring += this.GetText("ACTIVELOCATION", "PROFILE").FormatWith();
            }

            return outstring;
        }

        #endregion
    }
}