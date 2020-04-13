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
namespace YAF.Controls
{
    #region Using

    using System;
    using System.Web.UI.WebControls;

    using YAF.Configuration;
    using YAF.Core.BaseControls;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Web.Controls;

    #endregion

    /// <summary>
    /// Display Connect Control
    /// </summary>
    public partial class DisplayConnect : BaseUserControl
    {
        #region Methods

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            var endPoint = new Literal { Text = "." };

            var isLoginAllowed = false;
            var isRegisterAllowed = false;

            if (Config.IsAnyPortal)
            {
                this.Visible = false;
            }
            else
            {
                if (Config.AllowLoginAndLogoff)
                {
                    this.ConnectHolder.Controls.Add(
                        new Literal { Text = $"<strong>{this.GetText("TOOLBAR", "WELCOME_GUEST_CONNECT")}</strong>" });

                    // show login
                    var loginLink = new ThemeButton
                                        {
                                            TextLocalizedTag = "LOGIN_CONNECT",
                                            TextLocalizedPage = "TOOLBAR",
                                            ParamText0 = this.Get<BoardSettings>().Name,
                                            TitleLocalizedTag = "LOGIN",
                                            TitleLocalizedPage = "TOOLBAR",
                                            Type = ButtonStyle.Link,
                                            Icon = "sign-in-alt",
                                            NavigateUrl = "javascript:void(0);",
                                            CssClass = "LoginLink"
                                        };

                    this.ConnectHolder.Controls.Add(loginLink);

                    isLoginAllowed = true;
                }

                if (!this.Get<BoardSettings>().DisableRegistrations)
                {
                    // show register link
                    var registerLink = new ThemeButton
                                           {
                                               TextLocalizedTag = "REGISTER_CONNECT",
                                               TextLocalizedPage = "TOOLBAR",
                                               TitleLocalizedTag = "REGISTER",
                                               TitleLocalizedPage = "TOOLBAR",
                                               Type = ButtonStyle.Link,
                                               Icon = "user-plus",
                                               NavigateUrl =
                                                   this.Get<BoardSettings>().ShowRulesForRegistration
                                                       ? BuildLink.GetLink(ForumPages.Rules)
                                                       : !this.Get<BoardSettings>().UseSSLToRegister
                                                           ? BuildLink.GetLink(ForumPages.Register)
                                                           : BuildLink.GetLink(
                                                               ForumPages.Register,
                                                               true).Replace("http:", "https:")
                                           };

                    this.ConnectHolder.Controls.Add(registerLink);

                    isRegisterAllowed = true;
                }
                else
                {
                    this.ConnectHolder.Controls.Add(endPoint);

                    this.ConnectHolder.Controls.Add(new Literal { Text = this.GetText("TOOLBAR", "DISABLED_REGISTER") });
                }

                // If both disallowed
                if (!isLoginAllowed && !isRegisterAllowed)
                {
                    this.ConnectHolder.Controls.Clear();

                    this.ConnectHolder.Visible = false;

                    return;
                }

                if (this.Get<BoardSettings>().AllowSingleSignOn
                    && (Config.FacebookAPIKey.IsSet() || Config.TwitterConsumerKey.IsSet()
                        || Config.GoogleClientID.IsSet()))
                {
                    this.ConnectHolder.Controls.Add(
                        new Literal { Text = $"&nbsp;{this.GetText("LOGIN", "CONNECT_VIA")}&nbsp;" });

                    if (Config.FacebookAPIKey.IsSet() && Config.FacebookSecretKey.IsSet())
                    {
                        var linkButton = new LinkButton
                                             {
                                                 Text = "Facebook",
                                                 ToolTip =
                                                     this.GetTextFormatted("AUTH_CONNECT_HELP", "Facebook"),
                                                 ID = "FacebookRegister",
                                                 CssClass = "authLogin facebookLogin"
                    };

                        linkButton.Click += this.FacebookFormClick;

                        this.ConnectHolder.Controls.Add(linkButton);
                    }

                    this.ConnectHolder.Controls.Add(new Literal { Text = "&nbsp;" });

                    if (Config.TwitterConsumerKey.IsSet() && Config.TwitterConsumerSecret.IsSet())
                    {
                        var linkButton = new LinkButton
                                             {
                                                 Text = "Twitter",
                                                 ToolTip =
                                                     this.GetTextFormatted("AUTH_CONNECT_HELP", "Twitter"),
                                                 ID = "TwitterRegister",
                                                 CssClass = "authLogin twitterLogin"
                                             };

                        linkButton.Click += this.TwitterFormClick;

                        this.ConnectHolder.Controls.Add(linkButton);
                    }

                    this.ConnectHolder.Controls.Add(new Literal { Text = "&nbsp;" });

                    if (Config.GoogleClientID.IsSet() && Config.GoogleClientSecret.IsSet())
                    {
                        var linkButton = new LinkButton
                                             {
                                                 Text = "Google",
                                                 ToolTip = this.GetTextFormatted("AUTH_CONNECT_HELP", "Google"),
                                                 ID = "GoogleRegister",
                                                 CssClass = "authLogin googleLogin"
                                             };

                        linkButton.Click += this.GoogleFormClick;

                        this.ConnectHolder.Controls.Add(linkButton);
                    }
                }
            }
        }

        /// <summary>
        /// Show the Facebook Login/Register Form
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void FacebookFormClick(object sender, EventArgs e)
        {
            BuildLink.Redirect(ForumPages.Login, "auth={0}", "facebook");
        }

        /// <summary>
        /// Show the Twitter Login/Register Form
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void TwitterFormClick(object sender, EventArgs e)
        {
            BuildLink.Redirect(ForumPages.Login, "auth={0}", "twitter");
        }

        /// <summary>
        /// Show the Google Login/Register Form
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void GoogleFormClick(object sender, EventArgs e)
        {
            BuildLink.Redirect(ForumPages.Login, "auth={0}", "google");
        }

        #endregion
    }
}