/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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

namespace YAF.Pages;

using YAF.Types.Models;

/// <summary>
/// Class to communicate in Jabber.
/// </summary>
public partial class Jabber : ForumPage
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "Jabber" /> class.
    /// </summary>
    public Jabber()
        : base("IM_XMPP", ForumPages.Jabber)
    {
    }

    /// <summary>
    ///   Gets UserID.
    /// </summary>
    public int UserID =>
        this.Get<LinkBuilder>().StringToIntOrRedirect(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("u"));

    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.User == null)
        {
            this.Get<LinkBuilder>().AccessDenied();
        }

        if (this.IsPostBack)
        {
            return;
        }

        // get user data...
        var userHe = this.GetRepository<User>().GetById(this.UserID);

        // only approved members of the board
        if (userHe == null || userHe.BoardID != this.PageBoardContext.PageBoardID || userHe.UserFlags.IsGuest
            || !userHe.UserFlags.IsApproved)
        {
            this.Get<LinkBuilder>().AccessDenied();
        }

        this.PageBoardContext.PageLinks.AddRoot();
        this.PageBoardContext.PageLinks.AddUser(
            this.UserID,
            userHe.DisplayOrUserName());
        this.PageBoardContext.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);

        if (this.UserID == this.PageBoardContext.PageUserID)
        {
            this.NotifyLabel.Text = this.GetText("SERVERYOU");
            this.Alert.Type = MessageTypes.warning;
        }
        else
        {
            // get full user data...
            var userDataHe = this.Get<IAspNetUsersHelper>().GetUser(userHe.ProviderUserKey);

            // the member has no XMPP address
            if (userDataHe == null || userDataHe.Profile_XMPP.IsNotSet())
            {
                this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
                return;
            }

            var serverHe = GetServer(userDataHe.Profile_XMPP);

            // empty if the current user has no XMPP address
            var serverMe = GetServer(this.PageBoardContext.MembershipUser.Profile_XMPP);

            this.NotifyLabel.Text = serverMe == serverHe
                                        ? this.GetTextFormatted("SERVERSAME", userDataHe.Profile_XMPP)
                                        : this.GetTextFormatted("SERVEROTHER", $"http://{serverHe}");

            this.Alert.Type = MessageTypes.info;
        }
    }

    /// <summary>
    /// Gets the server of a XMPP address.
    /// </summary>
    /// <param name="address">The XMPP address.</param>
    /// <returns>Returns the server or an empty string if there is no address.</returns>
    private static string GetServer(string address)
    {
        return address.IsSet()
                   ? address.Substring(address.IndexOf("@", StringComparison.Ordinal) + 1).Trim()
                   : string.Empty;
    }
}