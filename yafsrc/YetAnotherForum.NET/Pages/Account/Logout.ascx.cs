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

namespace YAF.Pages.Account;

using System.Web.Security;

using YAF.Types.EventProxies;
using YAF.Types.Interfaces.Events;

/// <summary>
/// The Logout function
/// </summary>
public partial class Logout : AccountPage
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "Logout" /> class.
    /// </summary>
    public Logout()
        : base("LOGOUT", ForumPages.Account_Logout)
    {
        this.PageBoardContext.Globals.IsSuspendCheckEnabled = false;
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        this.Get<IAspNetUsersHelper>().SignOut();

        // Handle legacy ASP.NET Membership logout
        FormsAuthentication.SignOut();

        this.Get<IRaiseEvent>().Raise(new UserLogoutEvent(this.PageBoardContext.PageUserID));

        this.Get<LinkBuilder>().Redirect(ForumPages.Board);
    }
}