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
    using System.Web;
    using System.Web.UI;

    using YAF.Classes;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    ///     Provides a basic "profile link" for a YAF User
    /// </summary>
    public class UserLink : UserLabel
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether
        ///     Make the link target "blank" to open in a new window.
        /// </summary>
        public bool BlankTarget
        {
            get
            {
                return this.ViewState["BlankTarget"] != null && Convert.ToBoolean(this.ViewState["BlankTarget"]);
            }

            set
            {
                this.ViewState["BlankTarget"] = value;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether [enable hover card].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [enable hover card]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableHoverCard
        {
            get
            {
                return this.ViewState["EnableHoverCard"] == null || this.ViewState["EnableHoverCard"].ToType<bool>();
            }

            set
            {
                this.ViewState["EnableHoverCard"] = value;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether
        ///     User Is a Guest.
        /// </summary>
        public bool IsGuest
        {
            get
            {
                return this.ViewState["IsGuest"] != null && Convert.ToBoolean(this.ViewState["IsGuest"]);
            }

            set
            {
                this.ViewState["IsGuest"] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UserLink"/> is suspended.
        /// </summary>
        /// <value>
        ///   <c>true</c> if suspended; otherwise, <c>false</c>.
        /// </value>
        [NotNull]
        public bool Suspended
        {
            get
            {
                return this.ViewState["Suspended"] != null && Convert.ToBoolean(this.ViewState["Suspended"]);
            }

            set
            {
                this.ViewState["Style"] = value;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether the user has Profile View Permission
        /// </summary>
        /// <value>
        /// <c>true</c> if the user can view profiles; otherwise, <c>false</c>.
        /// </value>
        private bool CanViewProfile => this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().ProfileViewPermissions);

        /// <summary>
        /// Gets a value indicating whether is hover card enabled.
        /// </summary>
        private bool IsHoverCardEnabled => this.Get<YafBoardSettings>().EnableUserInfoHoverCards && this.EnableHoverCard;

        #endregion

        #region Methods

        /// <summary>
        /// The On PreRender event.
        /// </summary>
        /// <param name="e">
        /// the Event Arguments
        /// </param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            if (!this.CanViewProfile || !this.IsHoverCardEnabled)
            {
                return;
            }

            // Setup Hover Card JS
            YafContext.Current.PageElements.RegisterJsBlockStartup(
                "yafhovercardtjs",
                "if (typeof(jQuery.fn.hovercard) != 'undefined'){{ {0}('.userHoverCard').hovercard({{showYafCard: true, delay: {1}, width: 350,loadingHTML: '{2}',errorHTML: '{3}'}}); }}"
                    .FormatWith(
                        Config.JQueryAlias,
                        this.Get<YafBoardSettings>().HoverCardOpenDelay,
                        this.GetText("DEFAULT", "LOADING_HOVERCARD").ToJsString(),
                        this.GetText("DEFAULT", "ERROR_HOVERCARD").ToJsString()));
        }

        /// <summary>
        /// The render.
        /// </summary>
        /// <param name="output">
        /// The output.
        /// </param>
        protected override void Render([NotNull] HtmlTextWriter output)
        {
            var displayName = this.ReplaceName.IsNotSet()
                                  ? this.Get<IUserDisplayName>().GetName(this.UserID)
                                  : this.ReplaceName;

            if (this.UserID == -1 || !displayName.IsSet())
            {
                return;
            }

            // is this the guest user? If so, guest's don't have a profile.
            var isGuest = this.IsGuest ? this.IsGuest : UserMembershipHelper.IsGuestUser(this.UserID);

            output.BeginRender();

            if (!isGuest)
            {
                output.WriteBeginTag("a");

                output.WriteAttribute("href", YafBuildLink.GetLink(ForumPages.profile, "u={0}&name={1}", this.UserID, displayName));

                if (this.CanViewProfile && this.IsHoverCardEnabled)
                {
                    if (this.CssClass.IsSet())
                    {
                        this.CssClass += " btn-sm userHoverCard";
                    }
                    else
                    {
                        this.CssClass = " btn-sm userHoverCard";
                    }

                    output.WriteAttribute(
                        "data-hovercard",
                        "{0}resource.ashx?userinfo={1}&boardId={2}&type=json&forumUrl={3}".FormatWith(
                            Config.IsDotNetNuke ? BaseUrlBuilder.GetBaseUrlFromVariables() + BaseUrlBuilder.AppPath : YafForumInfo.ForumClientFileRoot,
                            this.UserID,
                            YafContext.Current.PageBoardID,
                            HttpUtility.UrlEncode(YafBuildLink.GetBasePath())));
                }
                else
                {
                    output.WriteAttribute("title", this.GetText("COMMON", "VIEW_USRPROFILE"));

                    if (this.CssClass.IsSet())
                    {
                        if (this.CssClass.Equals("dropdown-toggle"))
                        {
                            output.WriteAttribute("data-toggle", "dropdown");
                            output.WriteAttribute("aria-haspopup", "true");
                            output.WriteAttribute("aria-expanded", "false");
                        }

                        this.CssClass += " btn-sm";
                    }
                    else
                    {
                        this.CssClass = " btn-sm";
                    }
                }

                if (this.Get<YafBoardSettings>().UseNoFollowLinks)
                {
                    output.WriteAttribute("rel", "nofollow");
                }

                if (this.BlankTarget)
                {
                    output.WriteAttribute("target", "_blank");
                }
            }
            else
            {
                output.WriteBeginTag("span");
            }

            this.RenderMainTagAttributes(output);

            output.Write(HtmlTextWriter.TagRightChar);

            // show online icon
            if (this.Get<YafBoardSettings>().ShowUserOnlineStatus)
            {
                var onlineStatusIcon = new OnlineStatusIcon { UserId = this.UserID };

                onlineStatusIcon.RenderControl(output);

                output.Write("&nbsp;");
            }

            // Replace Name with Crawler Name if Set, otherwise use regular display name or Replace Name if set
            if (this.CrawlerName.IsSet())
            {
                output.WriteEncodedText(this.CrawlerName);
            }
            else if (!this.CrawlerName.IsSet() && this.ReplaceName.IsSet() && isGuest)
            {
                output.WriteEncodedText(this.ReplaceName);
            }
            else
            {
                output.WriteEncodedText(displayName);
            }

            output.WriteEndTag(!isGuest ? "a" : "span");

            if (this.PostfixText.IsSet())
            {
                output.Write(this.PostfixText);
            }

            output.EndRender();
        }

        #endregion
    }
}