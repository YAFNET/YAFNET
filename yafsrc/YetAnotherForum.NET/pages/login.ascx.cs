/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj�rnar Henden
 * Copyright (C) 2006-2011 Jaben Cargman
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
  #region Using

  using System;
  using System.Web;
  using System.Web.Security;
  using System.Web.UI.WebControls;

  using YAF.Classes;
  using YAF.Core;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.EventProxies;
  using YAF.Types.Interfaces;
  using YAF.Utils;
  using YAF.Utils.Helpers;

  #endregion

  /// <summary>
  /// Summary description for login.
  /// </summary>
  public partial class login : ForumPage
  {
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "login" /> class.
    /// </summary>
    public login()
      : base("LOGIN")
    {
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets a value indicating whether IsProtected.
    /// </summary>
    public override bool IsProtected
    {
      get
      {
        return false;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Inits the page.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// Event Args.
    /// </param>
    protected void Init([NotNull] object sender, [NotNull] EventArgs e)
    {
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
    protected void Login1_Authenticate([NotNull] object sender, [NotNull] AuthenticateEventArgs e)
    {
      var userName = this.Login1.FindControlAs<TextBox>("UserName").Text.Trim();
      var password = this.Login1.FindControlAs<TextBox>("Password").Text.Trim();

      e.Authenticated = false;

      // validate userName and password...
      if (this.Get<MembershipProvider>().ValidateUser(userName, password))
      {
        e.Authenticated = true;
      }
      else if (this.Get<YafBoardSettings>().EnableDisplayName)
      {
        var id = this.Get<IUserDisplayName>().GetId(userName);

        if (id.HasValue)
        {
          // get the username associated with this id...
          userName = UserMembershipHelper.GetUserNameFromID(id.Value);

          // validate again...
           if (this.Get<MembershipProvider>().ValidateUser(userName, password))
           {
             e.Authenticated = true;

             // update the username
             this.Login1.UserName = userName;
           }
        }
      }
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
    protected void Login1_LoginError([NotNull] object sender, [NotNull] EventArgs e)
    {
      bool emptyFields = false;

      var userName = this.Login1.FindControlAs<TextBox>("UserName");
      var password = this.Login1.FindControlAs<TextBox>("Password");

      if (userName.Text.Trim().Length == 0)
      {
        this.PageContext.AddLoadMessage(this.GetText("REGISTER", "NEED_USERNAME"));
        emptyFields = true;
      }

      if (password.Text.Trim().Length == 0)
      {
        this.PageContext.AddLoadMessage(this.GetText("REGISTER", "NEED_PASSWORD"));
        emptyFields = true;
      }

      if (!emptyFields)
      {
        this.PageContext.AddLoadMessage(this.Login1.FailureText);
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
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
        if (this.IsPostBack)
        {
            return;
        }

        this.Login1.MembershipProvider = Config.MembershipProvider;

        this.PageLinks.AddLink(this.Get<YafBoardSettings>().Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink(this.GetText("title"));

        // Login1.CreateUserText = "Sign up for a new account.";
        // Login1.CreateUserUrl = YafBuildLink.GetLink( ForumPages.register );
        this.Login1.PasswordRecoveryText = this.GetText("lostpassword");
        this.Login1.PasswordRecoveryUrl = YafBuildLink.GetLink(ForumPages.recoverpassword);
        this.Login1.FailureText = this.GetText("password_error");

        if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("ReturnUrl").IsSet())
        {
            this.Login1.DestinationPageUrl = this.Server.UrlDecode(
                this.Request.QueryString.GetFirstOrDefault("ReturnUrl"));
        }
        else
        {
            this.Login1.DestinationPageUrl = YafBuildLink.GetLink(ForumPages.forum);
        }

        // localize controls
        var rememberMe = this.Login1.FindControlAs<CheckBox>("RememberMe");
        var userName = this.Login1.FindControlAs<TextBox>("UserName");
        var password = this.Login1.FindControlAs<TextBox>("Password");
        var forumLogin = this.Login1.FindControlAs<Button>("LoginButton");
        var passwordRecovery = this.Login1.FindControlAs<Button>("PasswordRecovery");

        userName.Focus();

        /*
        RequiredFieldValidator usernameRequired = ( RequiredFieldValidator ) Login1.FindControl( "UsernameRequired" );
        RequiredFieldValidator passwordRequired = ( RequiredFieldValidator ) Login1.FindControl( "PasswordRequired" );

        usernameRequired.ToolTip = usernameRequired.ErrorMessage = GetText( "REGISTER", "NEED_USERNAME" );
        passwordRequired.ToolTip = passwordRequired.ErrorMessage = GetText( "REGISTER", "NEED_PASSWORD" );
        */
        if (rememberMe != null)
        {
            rememberMe.Text = this.GetText("auto");
        }

        if (forumLogin != null)
        {
            forumLogin.Text = this.GetText("FORUM_LOGIN");
        }

        if (passwordRecovery != null)
        {
            passwordRecovery.Text = this.GetText("LOSTPASSWORD");
        }

        if (password != null && forumLogin != null)
        {
            password.Attributes.Add(
                "onkeydown", 
                "if(event.which || event.keyCode){if ((event.which == 13) || (event.keyCode == 13)) {document.getElementById('" +
                forumLogin.ClientID + "').click();return false;}} else {return true}; ");
        }

        this.DataBind();
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
    protected void PasswordRecovery_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      YafBuildLink.Redirect(ForumPages.recoverpassword);
    }

    #endregion

      /// <summary>
      /// The Logged In Event
      /// </summary>
      /// <param name="sender">
      /// The sender.
      /// </param>
      /// <param name="e">
      /// The e.
      /// </param>
      protected void Login1_LoggedIn(object sender, EventArgs e)
    {
      this.Get<IRaiseEvent>().Raise(new SuccessfulUserLoginEvent(this.PageContext.PageUserID));
    }
  }
}