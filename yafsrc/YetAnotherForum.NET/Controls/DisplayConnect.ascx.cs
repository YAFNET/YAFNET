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

using YAF.Web.Controls;

/// <summary>
/// Display Connect Control
/// </summary>
public partial class DisplayConnect : BaseUserControl
{
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
            return;
        }

        if (Config.AllowLoginAndLogoff)
        {
            this.ConnectHolder.Controls.Add(
                new Literal { Text = $"<strong>{this.GetText("TOOLBAR", "WELCOME_GUEST_CONNECT")}</strong>" });

            // show login
            var loginLink = new ThemeButton
                                {
                                    TextLocalizedTag = "LOGIN_CONNECT",
                                    TextLocalizedPage = "TOOLBAR",
                                    ParamText0 = this.PageBoardContext.BoardSettings.Name,
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

        if (!this.PageBoardContext.BoardSettings.DisableRegistrations)
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
                                       NavigateUrl = this.PageBoardContext.BoardSettings.ShowRulesForRegistration
                                                         ?
                                                         this.Get<LinkBuilder>().GetLink(ForumPages.RulesAndPrivacy)
                                                         : this.Get<LinkBuilder>().GetLink(ForumPages.Account_Register)
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
        if (isLoginAllowed || isRegisterAllowed)
        {
            return;
        }

        this.ConnectHolder.Controls.Clear();

        this.ConnectHolder.Visible = false;
    }
}