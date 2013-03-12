/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj�rnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
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

    using YAF.Classes;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// Information control displaying feedback information to users.
    /// </summary>
    public partial class info : ForumPage
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "info" /> class. 
        ///   Default constructor.
        /// </summary>
        public info()
            : base("INFO")
        {
            this.PageContext.Globals.IsSuspendCheckEnabled = false;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates page links for this page.
        /// </summary>
        protected override void CreatePageLinks()
        {
            // forum index
            this.PageLinks.AddLink(this.Get<YafBoardSettings>().Name, YafBuildLink.GetLink(ForumPages.forum));

            // information title text
            this.PageLinks.AddLink(this.Title.Text);
        }

        /// <summary>
        /// The on init.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            this.InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            // Put user code to initialize the page here
            if (this.IsPostBack)
            {
                return;
            }

            // localize button label
            this.Continue.Text = this.GetText("continue");

            // get redirect URL from parameter
            if (this.Request.QueryString.GetFirstOrDefault("url") != null)
            {
                // unescape ampersands
                this.RefreshURL = this.Request.QueryString.GetFirstOrDefault("url").Replace("&amp;", "&");
            }

            // try to get infomessage code from parameter
            try
            {
                // compare it converted to enumeration
                switch ((InfoMessage)int.Parse(this.Request.QueryString.GetFirstOrDefault("i")))
                {
                    case InfoMessage.Moderated: // Moderated
                        this.Title.Text = this.GetText("title_moderated");
                        this.Info.Text = this.GetText("moderated");
                        this.RefreshTime = 10;
                        break;
                    case InfoMessage.Suspended: // Suspended
                        this.Title.Text = this.GetText("title_suspended");
                        this.Info.Text = this.GetTextFormatted(
                            "suspended", this.Get<IDateTime>().FormatDateTime(this.PageContext.SuspendedUntil));
                        break;
                    case InfoMessage.RegistrationEmail: // Registration email
                        this.Title.Text = this.GetText("title_registration");
                        this.Info.Text = this.GetText("registration");
                        this.RefreshTime = 10;
                        this.RefreshURL = YafBuildLink.GetLink(ForumPages.login);
                        break;
                    case InfoMessage.AccessDenied: // Access Denied
                        this.Title.Text = this.GetText("title_accessdenied");
                        this.Info.Text = this.GetText("accessdenied");
                        this.RefreshTime = 10;
                        this.RefreshURL = YafBuildLink.GetLink(ForumPages.forum);
                        break;
                    case InfoMessage.Disabled: // Disabled feature
                        this.Title.Text = this.GetText("TITLE_ACCESSDENIED");
                        this.Info.Text = this.GetText("DISABLED");
                        this.RefreshTime = 10;
                        this.RefreshURL = YafBuildLink.GetLink(ForumPages.forum);
                        break;
                    case InfoMessage.Invalid: // Invalid argument!
                        this.Title.Text = this.GetText("TITLE_INVALID");
                        this.Info.Text = this.GetText("INVALID");
                        this.RefreshTime = 10;
                        this.RefreshURL = YafBuildLink.GetLink(ForumPages.forum);
                        break;
                    case InfoMessage.Failure: // some sort of failure
                        this.Title.Text = this.GetText("TITLE_FAILURE");
                        this.Info.Text = this.GetText("FAILURE");
                        this.RefreshTime = 10;
                        this.RefreshURL = YafBuildLink.GetLink(ForumPages.forum);
                        break;
                    case InfoMessage.RequiresCookies: // some sort of failure
                        this.Title.Text = this.GetText("TITLE_COOKIES");
                        this.Info.Text = this.GetText("COOKIES");
                        this.RefreshTime = 10;
                        this.RefreshURL = YafBuildLink.GetLink(ForumPages.forum);
                        break;
                    case InfoMessage.RequiresEcmaScript: // some sort of failure
                        this.Title.Text = this.GetText("TITLE_ECMAREQUIRED");
                        this.Info.Text = this.GetText("ECMAREQUIRED");
                        this.RefreshTime = 10;
                        this.RefreshURL = YafBuildLink.GetLink(ForumPages.forum);
                        break;
                    case InfoMessage.EcmaScriptVersionUnsupported: // some sort of failure
                        this.Title.Text = this.GetText("TITLE_ECMAVERSION");
                        this.Info.Text = this.GetText("ECMAVERSION");
                        this.RefreshTime = 10;
                        this.RefreshURL = YafBuildLink.GetLink(ForumPages.forum);
                        break;
                    case InfoMessage.HostAdminPermissionsAreRequired: // some sort of failure
                        this.Title.Text = this.GetText("TITLE_HOSTADMINPERMISSIONSREQUIRED");
                        this.Info.Text = this.GetText("HOSTADMINPERMISSIONSREQUIRED");
                        this.RefreshTime = 10;
                        this.RefreshURL = YafBuildLink.GetLink(ForumPages.forum);
                        break;
                }
            }
            catch (Exception)
            {
                // get title for exception message
                this.Title.Text = this.GetText("title_exception");

                // exception message
                this.Info.Text = "{1} <strong>{0}</strong>.".FormatWith(
                    this.PageContext.PageUserName, this.GetText("exception"));

                // redirect to forum main after 2 seconds
                this.RefreshTime = 2;
                this.RefreshURL = YafBuildLink.GetLink(ForumPages.forum);
            }

            // set continue button URL and visibility
            this.Continue.NavigateUrl = this.RefreshURL;
            this.Continue.Visible = this.RefreshURL != null;

            // create page links - must be placed after switch to display correct title (last breadcrumb trail)
            this.CreatePageLinks();
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        ///   the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }

        #endregion
    }
}