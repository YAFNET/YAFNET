/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2010 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

namespace YAF.Pages
{
  // YAF.Pages
  using System;
  using System.Web.UI.WebControls;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Utils;

  /// <summary>
  /// Summary description for login.
  /// </summary>
  public partial class login : ForumPage
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="login"/> class.
    /// </summary>
    public login()
      : base("LOGIN")
    {
    }

    /// <summary>
    /// Gets a value indicating whether IsProtected.
    /// </summary>
    public override bool IsProtected
    {
      get
      {
        return false;
      }
    }

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
      if (!IsPostBack)
      {
        this.Login1.MembershipProvider = Config.MembershipProvider;

        this.PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink(GetText("title"));

        // Login1.CreateUserText = "Sign up for a new account.";
        // Login1.CreateUserUrl = YafBuildLink.GetLink( ForumPages.register );
        this.Login1.PasswordRecoveryText = GetText("lostpassword");
        this.Login1.PasswordRecoveryUrl = YafBuildLink.GetLink(ForumPages.recoverpassword);
        this.Login1.FailureText = GetText("password_error");

        if (!String.IsNullOrEmpty(Request.QueryString["ReturnUrl"]))
        {
          this.Login1.DestinationPageUrl = Server.UrlDecode(Request.QueryString["ReturnUrl"]);
        }
        else
        {
          this.Login1.DestinationPageUrl = YafBuildLink.GetLink(ForumPages.forum);
        }

        // localize controls
        var rememberMe = ControlHelper.FindControlAs<CheckBox>(this.Login1, "RememberMe");
        var userName = ControlHelper.FindControlAs<TextBox>(this.Login1, "UserName");
        var password = ControlHelper.FindControlAs<TextBox>(this.Login1, "Password");
        var forumLogin = ControlHelper.FindControlAs<Button>(this.Login1, "LoginButton");
        var passwordRecovery = ControlHelper.FindControlAs<Button>(this.Login1, "PasswordRecovery");

        userName.Focus();

          /*
        RequiredFieldValidator usernameRequired = ( RequiredFieldValidator ) Login1.FindControl( "UsernameRequired" );
        RequiredFieldValidator passwordRequired = ( RequiredFieldValidator ) Login1.FindControl( "PasswordRequired" );

        usernameRequired.ToolTip = usernameRequired.ErrorMessage = GetText( "REGISTER", "NEED_USERNAME" );
        passwordRequired.ToolTip = passwordRequired.ErrorMessage = GetText( "REGISTER", "NEED_PASSWORD" );
        */
        if (rememberMe != null)
        {
          rememberMe.Text = GetText("auto");
        }

        if (forumLogin != null)
        {
          forumLogin.Text = GetText("forum_login");
        }

        if (passwordRecovery != null)
        {
          passwordRecovery.Text = GetText("lostpassword");
        }

        if (password != null && forumLogin != null)
        {
            password.Attributes.Add(
              "onkeydown",
              "if(event.which || event.keyCode){if ((event.which == 13) || (event.keyCode == 13)) {document.getElementById('" + forumLogin.ClientID +
              "').click();return false;}} else {return true}; ");
        }    

        DataBind();
      }
    }

    /// <summary>
    /// The login 1_ authenticate.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Login1_Authenticate(object sender, AuthenticateEventArgs e)
    {
      var userName = ControlHelper.FindControlAs<TextBox>(this.Login1, "UserName");
      var password = ControlHelper.FindControlAs<TextBox>(this.Login1, "Password");

      e.Authenticated = PageContext.CurrentMembership.ValidateUser(userName.Text.Trim(), password.Text.Trim());
    }

    /// <summary>
    /// The login 1_ login error.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Login1_LoginError(object sender, EventArgs e)
    {
      bool emptyFields = false;

      var userName = ControlHelper.FindControlAs<TextBox>(this.Login1, "UserName");
      var password = ControlHelper.FindControlAs<TextBox>(this.Login1, "Password");

      if (userName.Text.Trim().Length == 0)
      {
        PageContext.AddLoadMessage(GetText("REGISTER", "NEED_USERNAME"));
        emptyFields = true;
      }

      if (password.Text.Trim().Length == 0)
      {
        PageContext.AddLoadMessage(GetText("REGISTER", "NEED_PASSWORD"));
        emptyFields = true;
      }

      if (!emptyFields)
      {
        PageContext.AddLoadMessage(this.Login1.FailureText);
      }
    }

    /// <summary>
    /// Called when Password Recovery is Clicked
    /// </summary>
    /// <param name="sender">
    /// standard event object sender
    /// </param>
    /// <param name="e">
    /// event args
    /// </param>
    protected void PasswordRecovery_Click(object sender, EventArgs e)
    {
      YafBuildLink.Redirect(ForumPages.recoverpassword);
    }
  }
}