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
    using YAF.Dialogs;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Exceptions;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// EventArgs class for the PageTitleSet event
    /// </summary>
    public class ForumPageTitleArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ForumPageTitleArgs"/> class.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        public ForumPageTitleArgs([NotNull] string title)
        {
            this.Title = title;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets Title.
        /// </summary>
        public string Title { get; }

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
        private ForumPage currentForumPage;

        /// <summary>
        ///   The _page.
        /// </summary>
        private ILocatablePage page;

        /// <summary>
        ///   The _topControl.
        /// </summary>
        private PlaceHolder topControl;

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
        public Control Footer { get; set; }

        /// <summary>
        ///   Gets or sets The forum header control
        /// </summary>
        public Control Header { get; set; }

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
        public DialogBox NotificationBox { get; set; }

        /// <summary>
        ///   Gets UserID for the current User (Read Only)
        /// </summary>
        public int PageUserID => YafContext.Current.PageUserID;

        /// <summary>
        ///   Gets UserName for the current User (Read Only)
        /// </summary>
        public string PageUserName => YafContext.Current.User == null ? "Guest" : YafContext.Current.User.UserName;

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
        public IServiceLocator ServiceLocator => YafContext.Current.ServiceLocator;

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

            writer.Write(
                 @"<div class=""yaf-net"" id=""{0}"">".FormatWith(this.ClientID));

            writer.Write(
                @"<div id=""yafpage_{0}"" class=""{1}"">".FormatWith(
                    this.page.ToString(),
                    this.page.ToString().Replace(".", "_")));

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
            var yafScriptManager = ScriptManager.GetCurrent(this.Page);

            if (yafScriptManager != null)
            {
                yafScriptManager.EnableCdn = this.Get<YafBoardSettings>().ScriptManagerScriptsCDNHosted;
                yafScriptManager.EnableScriptLocalization = !this.Get<YafBoardSettings>().ScriptManagerScriptsCDNHosted;
                return;
            }

            // add a script manager since one doesn't exist...
            yafScriptManager = new ScriptManager
                                   {
                                       ID = "YafScriptManager",
                                       EnablePartialRendering = true,
                                       EnableCdn =
                                           this.Get<YafBoardSettings>().ScriptManagerScriptsCDNHosted,
                                       EnableScriptLocalization =
                                           !this.Get<YafBoardSettings>().ScriptManagerScriptsCDNHosted
                                   };

            this.Controls.Add(yafScriptManager);

            base.OnInit(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            // context is ready to be loaded, call the before page load event...
            this.BeforeForumPageLoad?.Invoke(this, new YafBeforeForumPageLoad());

            // add the forum header control...
            this.topControl = new PlaceHolder();
            this.Controls.AddAt(0, this.topControl);

            // get the current page...
            var src = this.GetPageSource();

            try
            {
                this.currentForumPage = (ForumPage)this.LoadControl(src);

                this.Header = this.LoadControl(
                    "{0}controls/{1}.ascx".FormatWith(
                        YafForumInfo.ForumServerFileRoot,
                        this.currentForumPage.IsAdminPage ? "AdminHeader" : "YafHeader"));

                this.Footer = new Footer();
            }
            catch (FileNotFoundException)
            {
                throw new ApplicationException("Failed to load {0}.".FormatWith(src));
            }

            this.currentForumPage.ForumTopControl = this.topControl;
            this.currentForumPage.ForumFooter = this.Footer;

            this.currentForumPage.ForumHeader = this.Header;

            // only show header if showtoolbar is enabled
            this.currentForumPage.ForumHeader.Visible = this.currentForumPage.ShowToolBar;

            // don't allow as a popup if it's not allowed by the page...
            if (!this.currentForumPage.AllowAsPopup && this.Popup)
            {
                this.Popup = false;
            }

            // set the YafContext ForumPage...
            YafContext.Current.CurrentForumPage = this.currentForumPage;

            // add the header control before the page rendering...
            if (!this.Popup && YafContext.Current.Settings.LockedForum == 0)
            {
                this.Controls.AddAt(1, this.Header);
            }

            // Add the LoginBox to Control, if used and User is Guest
            if (this.Get<YafBoardSettings>().UseLoginBox && YafContext.Current.IsGuest && !Config.IsAnyPortal
                && Config.AllowLoginAndLogoff)
            {
                this.Controls.Add(
                    this.LoadControl("{0}Dialogs/{1}.ascx".FormatWith(YafForumInfo.ForumServerFileRoot, "LoginBox")));
            }

            this.NotificationBox = (DialogBox)this.LoadControl(
                "{0}Dialogs/{1}.ascx".FormatWith(YafForumInfo.ForumServerFileRoot, "DialogBox"));

            this.currentForumPage.Notification = this.NotificationBox;

            this.Controls.Add(this.NotificationBox);

            this.Controls.Add(this.currentForumPage);

            // add the footer control after the page...
            if (!this.Popup && YafContext.Current.Settings.LockedForum == 0)
            {
                this.Controls.Add(this.Footer);
            }

            // TODO Move to Dialogs
            // Add image gallery dialog
            this.Controls.Add(
                this.LoadControl("{0}controls/ImageGallery.ascx".FormatWith(YafForumInfo.ForumServerFileRoot)));

            // Add cookie consent
            this.Controls.Add(
                this.LoadControl("{0}controls/CookieConsent.ascx".FormatWith(YafForumInfo.ForumServerFileRoot)));

            // load plugins/functionality modules
            this.AfterForumPageLoad?.Invoke(this, new YafAfterForumPageLoad());

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
            var pages = this.Get<IEnumerable<ILocatablePage>>().ToList();

            if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("g") != null)
            {
                var pageQuery = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("g");

                this.page = pages.GetPage(pageQuery);
            }

            if (this.page == null)
            {
                this.page = pages.GetPage("forum");
            }

            /*if (!this.IsValidForLockedForum(this._page))
            {
            /  YafBuildLink.Redirect(ForumPages.topics, "f={0}", this.LockedForum);
            }*/
            string[] src = { "{0}pages/{1}.ascx".FormatWith(YafForumInfo.ForumServerFileRoot, this.page.PageName) };

            var controlOverride = this.Get<ITheme>().GetItem("PAGE_OVERRIDE", this.page.PageName.ToLower(), null);

            if (controlOverride.IsSet())
            {
                src[0] = controlOverride;
            }

            var replacementPaths = new List<string> { "moderate", "admin", "help" };

            foreach (var path in replacementPaths.Where(
                path => src[0].IndexOf("/{0}_".FormatWith(path), StringComparison.Ordinal) >= 0))
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
            if (HttpContext.Current.Application[Constants.Cache.TaskModule] != null)
            {
                return;
            }

#if DEBUG
            throw new YafTaskModuleNotRegisteredException(
                @"YAF.NET is not setup properly. Please add the <add name=""YafTaskModule"" type=""YAF.Core.YafTaskModule, YAF.Core"" /> to the <modules> section of your web.config file.");
#else
// YAF is not setup properly...
            HttpContext.Current.Session["StartupException"] =
                @"YAF.NET is not setup properly. Please add the <add name=""YafTaskModule"" type=""YAF.Core.YafTaskModule, YAF.Core"" /> to the <modules> section of your web.config file.";

            // go immediately to the error page.
            HttpContext.Current.Response.Redirect(string.Format("{0}error.aspx", YafForumInfo.ForumClientFileRoot));

#endif
        }

        #endregion
    }
}