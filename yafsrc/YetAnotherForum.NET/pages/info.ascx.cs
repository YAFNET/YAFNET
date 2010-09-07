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
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Utils;

  /// <summary>
  /// Information control displaying feedback information to users.
  /// </summary>
  public partial class info : ForumPage
  {
    #region Constructors & Overridden Methods

    /// <summary>
    /// Initializes a new instance of the <see cref="info"/> class. 
    /// Default constructor.
    /// </summary>
    public info()
      : base("INFO")
    {
      PageContext.Globals.IsSuspendCheckEnabled = false;
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    protected override void CreatePageLinks()
    {
      // forum index
      this.PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));

      // information title text
      this.PageLinks.AddLink(this.Title.Text);
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">
    /// </param>
    /// <param name="e">
    /// </param>
    protected void Page_Load(object sender, EventArgs e)
    {
      // Put user code to initialize the page here
      if (!IsPostBack)
      {
        // localize button label
        this.Continue.Text = GetText("continue");

        // get redirect URL from parameter
        if (Request.QueryString.GetFirstOrDefault("url") != null)
        {
          // unescape ampersands
          RefreshURL = Request.QueryString.GetFirstOrDefault("url").Replace("&amp;", "&");
        }

        // try to get infomessage code from parameter
        try
        {
          // compare it converted to enumeration
          switch ((InfoMessage) int.Parse(Request.QueryString.GetFirstOrDefault("i")))
          {
            case InfoMessage.Moderated: // Moderated
              this.Title.Text = GetText("title_moderated");
              this.Info.Text = GetText("moderated");
              break;
            case InfoMessage.Suspended: // Suspended
              this.Title.Text = GetText("title_suspended");
              this.Info.Text = GetTextFormatted("suspended", YafServices.DateTime.FormatDateTime(PageContext.SuspendedUntil));
              break;
            case InfoMessage.RegistrationEmail: // Registration email
              this.Title.Text = GetText("title_registration");
              this.Info.Text = GetText("registration");
              RefreshTime = 10;
              RefreshURL = YafBuildLink.GetLink(ForumPages.login);
              break;
            case InfoMessage.AccessDenied: // Access Denied
              this.Title.Text = GetText("title_accessdenied");
              this.Info.Text = GetText("accessdenied");
              RefreshTime = 10;
              RefreshURL = YafBuildLink.GetLink(ForumPages.forum);
              break;
            case InfoMessage.Disabled: // Disabled feature
              this.Title.Text = GetText("TITLE_ACCESSDENIED");
              this.Info.Text = GetText("DISABLED");
              RefreshTime = 10;
              RefreshURL = YafBuildLink.GetLink(ForumPages.forum);
              break;
            case InfoMessage.Invalid: // Invalid argument!
              this.Title.Text = GetText("TITLE_INVALID");
              this.Info.Text = GetText("INVALID");
              RefreshTime = 10;
              RefreshURL = YafBuildLink.GetLink(ForumPages.forum);
              break;
            case InfoMessage.Failure: // some sort of failure
              this.Title.Text = GetText("TITLE_FAILURE");
              this.Info.Text = GetText("FAILURE");
              RefreshTime = 10;
              RefreshURL = YafBuildLink.GetLink(ForumPages.forum);
              break;
            case InfoMessage.RequiresCookies: // some sort of failure
              this.Title.Text = GetText("TITLE_COOKIES");
              this.Info.Text = GetText("COOKIES");
              RefreshTime = 10;
              RefreshURL = YafBuildLink.GetLink(ForumPages.forum);
              break;
            case InfoMessage.RequiresEcmaScript: // some sort of failure
              this.Title.Text = GetText("TITLE_ECMAREQUIRED");
              this.Info.Text = GetText("ECMAREQUIRED");
              RefreshTime = 10;
              RefreshURL = YafBuildLink.GetLink(ForumPages.forum);
              break;
            case InfoMessage.EcmaScriptVersionUnsupported: // some sort of failure
              this.Title.Text = GetText("TITLE_ECMAVERSION");
              this.Info.Text = GetText("ECMAVERSION");
              RefreshTime = 10;
              RefreshURL = YafBuildLink.GetLink(ForumPages.forum);
              break;
          }
        }
          
          // exception was thrown
        catch (Exception)
        {
          // get title for exception message
          this.Title.Text = GetText("title_exception");

          // exception message
          this.Info.Text = string.Format("{1} <strong>{0}</strong>.", PageContext.PageUserName, GetText("exception"));

          // redirect to forum main after 2 seconds
          RefreshTime = 2;
          RefreshURL = YafBuildLink.GetLink(ForumPages.forum);
        }

        // set continue button URL and visibility
        this.Continue.NavigateUrl = RefreshURL;
        this.Continue.Visible = RefreshURL != null;

        // create page links - must be placed after switch to display correct title (last breadcrumb trail)
        CreatePageLinks();
      }
    }

    #endregion

    #region Web Form Designer generated code

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit(EventArgs e)
    {
      // CODEGEN: This call is required by the ASP.NET Web Form Designer.
      InitializeComponent();
      base.OnInit(e);
    }

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
    }

    #endregion
  }
}