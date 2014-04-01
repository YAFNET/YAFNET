/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014 Ingo Herbote
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
  using System.Net.Mail;
  using System.Web;
  using System.Web.Security;

  using YAF.Classes;
  using YAF.Classes.Data;
  using YAF.Controls;
  using YAF.Core;
  using YAF.Core.Extensions;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// Summary description for active.
  /// </summary>
  public partial class im_email : ForumPage
  {
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "im_email" /> class.
    /// </summary>
    public im_email()
      : base("IM_EMAIL")
    {
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets UserID.
    /// </summary>
    public int UserID
    {
      get
      {
        return (int)Security.StringToLongOrRedirect(this.Get<HttpRequestBase>().QueryString["u"]);
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
      if (this.User == null || !this.Get<YafBoardSettings>().AllowEmailSending)
      {
        YafBuildLink.AccessDenied();
      }

        if (this.IsPostBack)
        {
            return;
        }

        // get user data...
        MembershipUser user = UserMembershipHelper.GetMembershipUserById(this.UserID);

        if (user == null)
        {
            YafBuildLink.AccessDenied(/*No such user exists*/);
        }

        string displayName = UserMembershipHelper.GetDisplayNameFromID(this.UserID);

        this.PageLinks.AddRoot();
        this.PageLinks.AddLink(
            this.PageContext.BoardSettings.EnableDisplayName ? displayName : user.UserName,
            YafBuildLink.GetLink(
                ForumPages.profile,
                "u={0}&name={1}",
                this.UserID,
                this.PageContext.BoardSettings.EnableDisplayName ? displayName : user.UserName));
        this.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);

        this.Send.Text = this.GetText("SEND");
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
      try
      {
        // get "to" user...
        MembershipUser toUser = UserMembershipHelper.GetMembershipUserById(this.UserID);

        // send it...
        this.Get<ISendMail>().Send(
          new MailAddress(this.PageContext.User.Email, this.PageContext.User.UserName), 
          new MailAddress(toUser.Email.Trim(), toUser.UserName.Trim()), 
          this.Subject.Text.Trim(), 
          this.Body.Text.Trim());

        // redirect to profile page...
        YafBuildLink.Redirect(ForumPages.profile, false, "u={0}", this.UserID);
      }
      catch (Exception x)
      {
          this.Logger.Log(this.PageContext.PageUserID, this, x);

          this.PageContext.AddLoadMessage(this.PageContext.IsAdmin ? x.Message : this.GetText("ERROR"));
      }
    }

    #endregion
  }
}