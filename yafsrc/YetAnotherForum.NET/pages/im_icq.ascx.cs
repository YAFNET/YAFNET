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

namespace YAF.Pages
{
  // YAF.Pages
  #region Using

  using System;
  using System.Web.Security;

  using YAF.Controls;
  using YAF.Core;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Extensions;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// Summary description for active.
  /// </summary>
  public partial class im_icq : ForumPage
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="im_icq"/> class.
    /// </summary>
    public im_icq()
      : base("IM_ICQ")
    {
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets UserID.
    /// </summary>
    public int UserID
    {
      get
      {
        return (int)Security.StringToLongOrRedirect(this.Request.QueryString["u"]);
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      if (this.User == null)
      {
        YafBuildLink.AccessDenied();
      }

      if (!this.IsPostBack)
      {
        this.Send.Text = this.GetText("SEND");
        this.From.Text = this.PageContext.User.UserName;
        this.Email.Text = this.PageContext.User.Email;

        // get user data...
        var user = UserMembershipHelper.GetMembershipUserById(this.UserID);

        if (user == null)
        {
          YafBuildLink.AccessDenied( /*No such user exists*/);
        }

        var displayName = UserMembershipHelper.GetDisplayNameFromID(this.UserID);

        this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
          this.PageLinks.AddLink(
              this.PageContext.BoardSettings.EnableDisplayName ? displayName : user.UserName,
              YafBuildLink.GetLink(
                  ForumPages.profile,
                  "u={0}&name={1}",
                  this.UserID,
                  this.PageContext.BoardSettings.EnableDisplayName ? displayName : user.UserName));
        this.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);

        // get full user data...
        var userData = new CombinedUserDataHelper(user, this.UserID);

        this.ViewState["to"] = userData.Profile.ICQ;
        this.Status.Src = "http://web.icq.com/whitepages/online?icq={0}&img=5".FormatWith(userData.Profile.ICQ);
      }
    }

    /// <summary>
    /// The send_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Send_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      var html =
        "http://wwp.icq.com/scripts/WWPMsg.dll?from={0}&fromemail={1}&subject={2}&to={3}&body={4}".FormatWith(
          this.Server.UrlEncode(this.From.Text), 
          this.Server.UrlEncode(this.Email.Text), 
          this.Server.UrlEncode("From WebPager Panel"), 
          this.ViewState["to"], 
          this.Server.UrlEncode(this.Body.Text));
      this.Response.Redirect(html);
    }

    #endregion
  }
}