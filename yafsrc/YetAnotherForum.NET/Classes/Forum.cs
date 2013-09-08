﻿/* YetAnotherForum.NET
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

namespace YAF
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// EventArgs class for the PageTitleSet event
    /// </summary>
    public class ForumPageTitleArgs : EventArgs
    {
        #region Constants and Fields

        /// <summary>
        ///   The _title.
        /// </summary>
        private readonly string _title;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ForumPageTitleArgs"/> class.
        /// </summary>
        /// <param name="title">
        /// The title. 
        /// </param>
        public ForumPageTitleArgs([NotNull] string title)
        {
            this._title = title;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets Title.
        /// </summary>
        public string Title
        {
            get
            {
                return this._title;
            }
        }

        #endregion
    }

    /// <summary>
    /// EventArgs class for the YafBeforeForumPageLoad event
    /// </summary>
    public class YafBeforeForumPageLoad : EventArgs
    {
    }

    /// <summary>
    /// EventArgs class for the YafForumPageReady event -- created for future options
    /// </summary>
    public class YafAfterForumPageLoad : EventArgs
    {
    }

    /// <summary>
    /// The YAF Forum Control
    /// </summary>
    [ToolboxData("<{0}:Forum runat=\"server\"></{0}:Forum>")]
    public class Forum : UserControl, IHaveServiceLocator
    {
        #region Constants and Fields

        /// <summary>
        ///   The _current forum page.
        /// </summary>
        private ForumPage _currentForumPage;

        /// <summary>
        ///   The _footer.
        /// </summary>
        private Control _footer;

        /// <summary>
        ///   The _header
        /// </summary>
        private Control _header;

        /// <summary>
        ///   The Notification PopUp
        /// </summary>
        private DialogBox _notificationBox;

        /// <summary>
        ///   The _page.
        /// </summary>
        private ILocatablePage _page;

        /// <summary>
        ///   The _topControl.
        /// </summary>
        private PlaceHolder _topControl;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="Forum" /> class.
        /// </summary>
        public Forum()
        {
            // validate YafTaskModule is running...
            this.TaskModuleRunning();

            // init the modules and run them immediately...
            var baseModules = this.Get<IModuleManager<IBaseForumModule>>();

            foreach (var module in baseModules.GetAll())
            {
                module.ForumControlObj = this;
                module.Init();
            }
        }

        #endregion

        #region Public Events

        /// <summary>
        ///   The after forum page load.
        /// </summary>
        public event EventHandler<YafAfterForumPageLoad> AfterForumPageLoad;

        /// <summary>
        ///   The before forum page load.
        /// </summary>
        public event EventHandler<YafBeforeForumPageLoad> BeforeForumPageLoad;

        /// <summary>
        ///   The page title set.
        /// </summary>
        public event EventHandler<ForumPageTitleArgs> PageTitleSet;

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets the Board ID for this instance of the forum control, overriding the value defined in app.config.
        /// </summary>
        public int BoardID
        {
            get
            {
                return YafControlSettings.Current.BoardID;
            }

            set
            {
                YafControlSettings.Current.BoardID = value;
            }
        }

        /// <summary>
        ///   Gets or sets the CategoryID for this instance of the forum control
        /// </summary>
        public int CategoryID
        {
            get
            {
                return YafControlSettings.Current.CategoryID;
            }

            set
            {
                YafControlSettings.Current.CategoryID = value;
            }
        }

        /// <summary>
        ///   Gets or sets The forum footer control
        /// </summary>
        public Control Footer
        {
            get
            {
                return this._footer;
            }

            set
            {
                this._footer = value;
            }
        }

        /// <summary>
        ///   Gets or sets The forum header control
        /// </summary>
        public Control Header
        {
            get
            {
                return this._header;
            }

            set
            {
                this._header = value;
            }
        }

        /// <summary>
        ///   Gets or sets LockedForum.
        /// </summary>
        public int LockedForum
        {
            get
            {
                return YafControlSettings.Current.LockedForum;
            }

            set
            {
                YafControlSettings.Current.LockedForum = value;
            }
        }

        /// <summary>
        ///   Gets or sets The forum header control
        /// </summary>
        public DialogBox NotificationBox
        {
            get
            {
                return this._notificationBox;
            }

            set
            {
                this._notificationBox = value;
            }
        }

        /// <summary>
        ///   Gets UserID for the current User (Read Only)
        /// </summary>
        public int PageUserID
        {
            get
            {
                return YafContext.Current.PageUserID;
            }
        }

        /// <summary>
        ///   Gets UserName for the current User (Read Only)
        /// </summary>
        public string PageUserName
        {
            get
            {
                return YafContext.Current.User == null ? "Guest" : YafContext.Current.User.UserName;
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether Popup.
        /// </summary>
        public bool Popup
        {
            get
            {
                return YafControlSettings.Current.Popup;
            }

            set
            {
                YafControlSettings.Current.Popup = value;
            }
        }

        /// <summary>
        ///   Gets ServiceLocator.
        /// </summary>
        public IServiceLocator ServiceLocator
        {
            get
            {
                return YafContext.Current.ServiceLocator;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Called when the forum control sets it's Page Title
        /// </summary>
        /// <param name="sender">
        /// The sender. 
        /// </param>
        /// <param name="e">
        /// The e. 
        /// </param>
        public void FirePageTitleSet([NotNull] object sender, [NotNull] ForumPageTitleArgs e)
        {
            if (this.PageTitleSet != null)
            {
                this.PageTitleSet(this, e);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The on unload.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);

            // make sure the YafContext is disposed of...
            YafContext.Current.Dispose();
        }

        /// <summary>
        /// The render.
        /// </summary>
        /// <param name="writer">
        /// The writer. 
        /// </param>
        protected override void Render([NotNull] HtmlTextWriter writer)
        {
            // wrap the forum in one main div and then a page div for better CSS selection
            writer.WriteLine();
            writer.Write(@"<div class=""yafnet"" id=""{0}"">".FormatWith(this.ClientID));
            writer.Write(
                @"<div id=""yafpage_{0}"" class=""{1}"">".FormatWith(
                    this._page.ToString(), this._page.ToString().Replace(".", "_")));

            // render the forum
            base.Render(writer);

            writer.WriteLine("</div></div>");
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            // run startup services -- should already be called except when running inside a CMS.
            this.RunStartupServices();

            // handle script manager first...
            if (ScriptManager.GetCurrent(this.Page) != null)
            {
                return;
            }

            // add a script manager since one doesn't exist...
            var yafScriptManager = new ScriptManager { ID = "YafScriptManager", EnablePartialRendering = true };
            this.Controls.Add(yafScriptManager);

            base.OnInit(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        /// <exception cref="ApplicationException"></exception>
        protected override void OnLoad(EventArgs e)
        {
            // context is ready to be loaded, call the before page load event...
            if (this.BeforeForumPageLoad != null)
            {
                this.BeforeForumPageLoad(this, new YafBeforeForumPageLoad());
            }

            // "forum load" should be done by now, load the user and page...
            var userId = YafContext.Current.PageUserID;

            // add the forum header control...
            this._topControl = new PlaceHolder();
            this.Controls.AddAt(0, this._topControl);

            // get the current page...
            string src = this.GetPageSource();

            try
            {
                this._currentForumPage = (ForumPage)this.LoadControl(src);

                this._header =
                    this.LoadControl("{0}controls/{1}.ascx".FormatWith(YafForumInfo.ForumServerFileRoot, "YafHeader"));

                this._footer = new Footer();
            }
            catch (FileNotFoundException)
            {
                throw new ApplicationException("Failed to load {0}.".FormatWith(src));
            }

            this._currentForumPage.ForumTopControl = this._topControl;
            this._currentForumPage.ForumFooter = this._footer;

            this._currentForumPage.ForumHeader = this._header;

            // only show header if showtoolbar is enabled
            this._currentForumPage.ForumHeader.Visible = this._currentForumPage.ShowToolBar;

            // don't allow as a popup if it's not allowed by the page...
            if (!this._currentForumPage.AllowAsPopup && this.Popup)
            {
                this.Popup = false;
            }

            // set the YafContext ForumPage...
            YafContext.Current.CurrentForumPage = this._currentForumPage;

            // add the header control before the page rendering...
            if (!this.Popup && YafContext.Current.Settings.LockedForum == 0)
            {
                this.Controls.AddAt(1, this._header);
            }

            // Add the LoginBox to Control, if used and User is Guest
            if (this.Get<YafBoardSettings>().UseLoginBox && YafContext.Current.IsGuest && !Config.IsAnyPortal
                && Config.AllowLoginAndLogoff)
            {
                this.Controls.Add(
                    this.LoadControl("{0}controls/{1}.ascx".FormatWith(YafForumInfo.ForumServerFileRoot, "LoginBox")));
            }

            this._notificationBox =
                (DialogBox)
                this.LoadControl("{0}controls/{1}.ascx".FormatWith(YafForumInfo.ForumServerFileRoot, "DialogBox"));

            this._currentForumPage.Notification = this._notificationBox;

            this.Controls.Add(this._notificationBox);

            this.Controls.Add(this._currentForumPage);

            // add the footer control after the page...
            if (!this.Popup && YafContext.Current.Settings.LockedForum == 0)
            {
                this.Controls.Add(this._footer);
            }

            // load plugins/functionality modules
            if (this.AfterForumPageLoad != null)
            {
                this.AfterForumPageLoad(this, new YafAfterForumPageLoad());
            }

            base.OnLoad(e);
        }

        /// <summary>
        /// Gets the page source.
        /// </summary>
        /// <returns>
        /// The get page source.
        /// </returns>
        [NotNull]
        private string GetPageSource()
        {
            var pages = this.Get<IEnumerable<ILocatablePage>>();

            if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("g") != null)
            {
                var pageQuery = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("g");

                this._page = pages.GetPage(pageQuery);
            }

            if (this._page == null)
            {
                this._page = pages.GetPage("forum");
            }

            /*if (!this.IsValidForLockedForum(this._page))
            {
            /  YafBuildLink.Redirect(ForumPages.topics, "f={0}", this.LockedForum);
            }*/
            string[] src = { "{0}pages/{1}.ascx".FormatWith(YafForumInfo.ForumServerFileRoot, this._page.PageName) };

            string controlOverride = this.Get<ITheme>().GetItem("PAGE_OVERRIDE", this._page.PageName.ToLowerInvariant(), null);

            if (controlOverride.IsSet())
            {
                src[0] = controlOverride;
            }

            var replacementPaths = new List<string> { "moderate", "admin", "help" };

            foreach (var path in replacementPaths.Where(path => src[0].IndexOf("/{0}_".FormatWith(path)) >= 0))
            {
                src[0] = src[0].Replace("/{0}_".FormatWith(path), "/{0}/".FormatWith(path));
            }

            return src[0];
        }

        /// <summary>
        /// The task module running.
        /// </summary>
        private void TaskModuleRunning()
        {
#if DEBUG
       const bool Debugging = true;
#else
       const bool Debugging = false;
#endif

            if (HttpContext.Current.Application[Constants.Cache.TaskModule] != null)
            {
                return;
            }

            if (!Debugging)
            {
                // YAF is not setup properly...
                HttpContext.Current.Session["StartupException"] =
                    @"YAF.NET is not setup properly. Please add the <add name=""YafTaskModule"" type=""YAF.Core.YafTaskModule, YAF.Core"" /> to the <modules> section of your web.config file.";

                // go immediately to the error page.
                HttpContext.Current.Response.Redirect(YafForumInfo.ForumClientFileRoot + "error.aspx");
            }
            else
            {
                throw new YafTaskModuleNotRegisteredException(
                    @"YAF.NET is not setup properly. Please add the <add name=""YafTaskModule"" type=""YAF.Core.YafTaskModule, YAF.Core"" /> to the <modules> section of your web.config file.");
            }
        }

        #endregion
    }
}