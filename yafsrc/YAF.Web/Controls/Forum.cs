/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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

namespace YAF.Web.Controls
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using YAF.Configuration;
    using YAF.Core;
    using YAF.Core.BaseControls;
    using YAF.Core.BasePages;
    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Constants;
#if DEBUG
    using YAF.Types.Exceptions;
#endif
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Web.EventsArgs;

    #endregion

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
            // validate TaskModule is running...
            TaskModuleRunning();

            // init the modules and run them immediately...
            var baseModules = this.Get<IModuleManager<IBaseForumModule>>();

            baseModules.GetAll().ForEach(
                module =>
                    {
                        module.ForumControlObj = this;
                        module.Init();
                    });
        }

        #endregion

        #region Public Events

        /// <summary>
        ///   The after forum page load.
        /// </summary>
        public event EventHandler<AfterForumPageLoad> AfterForumPageLoad;

        /// <summary>
        ///   The before forum page load.
        /// </summary>
        public event EventHandler<BeforeForumPageLoad> BeforeForumPageLoad;

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
            get => ControlSettings.Current.BoardID;

            set => ControlSettings.Current.BoardID = value;
        }

        /// <summary>
        ///   Gets or sets the CategoryID for this instance of the forum control
        /// </summary>
        public int CategoryID
        {
            get => ControlSettings.Current.CategoryID;

            set => ControlSettings.Current.CategoryID = value;
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
            get => ControlSettings.Current.LockedForum;

            set => ControlSettings.Current.LockedForum = value;
        }

        /// <summary>
        ///   Gets or sets The forum header control
        /// </summary>
        public BaseUserControl NotificationBox { get; set; }

        /// <summary>
        ///   Gets UserID for the current User (Read Only)
        /// </summary>
        public int PageUserID => BoardContext.Current.PageUserID;

        /// <summary>
        ///   Gets UserName for the current User (Read Only)
        /// </summary>
        public string PageUserName => BoardContext.Current.User == null ? "Guest" : BoardContext.Current.User.UserName;

        /// <summary>
        ///   Gets ServiceLocator.
        /// </summary>
        public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;

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
            this.PageTitleSet?.Invoke(this, e);
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

            // make sure the BoardContext is disposed of...
            BoardContext.Current.Dispose();
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

            writer.Write(@"<div id=""{0}"" class=""yafnet"">", this.ClientID);

            writer.Write(@"<div class=""page-{0}"">", this.page.PageName.ToLower().Replace("_", "-"));

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
                yafScriptManager.EnableCdn = this.Get<BoardSettings>().ScriptManagerScriptsCDNHosted;
                yafScriptManager.EnableScriptLocalization = !this.Get<BoardSettings>().ScriptManagerScriptsCDNHosted;
                yafScriptManager.EnableCdnFallback = this.Get<BoardSettings>().ScriptManagerScriptsCDNHosted;

                return;
            }

            // add a script manager since one doesn't exist...
            yafScriptManager = new ScriptManager
                                   {
                                       ID = "YafScriptManager",
                                       EnablePartialRendering = true,
                                       EnableCdn = this.Get<BoardSettings>().ScriptManagerScriptsCDNHosted,
                                       EnableCdnFallback = this.Get<BoardSettings>().ScriptManagerScriptsCDNHosted,
                                       EnableScriptLocalization =
                                           !this.Get<BoardSettings>().ScriptManagerScriptsCDNHosted
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
            this.BeforeForumPageLoad?.Invoke(this, new BeforeForumPageLoad());

            // add the forum header control...
            this.topControl = new PlaceHolder();
            this.Controls.AddAt(0, this.topControl);

            // get the current page...
            var src = this.GetPageSource();

            try
            {
                this.currentForumPage = (ForumPage)this.LoadControl(src);

                this.Header = this.LoadControl($"{BoardInfo.ForumServerFileRoot}controls/Header.ascx");

                this.Footer = new Footer();
            }
            catch (FileNotFoundException)
            {
                throw new ApplicationException($"Failed to load {src}.");
            }

            this.currentForumPage.ForumTopControl = this.topControl;
            this.currentForumPage.ForumFooter = this.Footer;

            this.currentForumPage.ForumHeader = this.Header;

            // only show header if show toolbar is enabled
            this.currentForumPage.ForumHeader.Visible = this.currentForumPage.ShowToolBar;

            // set the BoardContext ForumPage...
            BoardContext.Current.CurrentForumPage = this.currentForumPage;

            // add the header control before the page rendering...
            if (BoardContext.Current.Settings.LockedForum == 0)
            {
                this.Controls.AddAt(1, this.Header);
            }

            // Add the LoginBox to Control, if used and User is Guest
            if (BoardContext.Current.IsGuest && !Config.IsAnyPortal && Config.AllowLoginAndLogoff)
            {
                this.Controls.Add(this.LoadControl($"{BoardInfo.ForumServerFileRoot}Dialogs/LoginBox.ascx"));
            }

            this.NotificationBox = (BaseUserControl)this.LoadControl(
                $"{BoardInfo.ForumServerFileRoot}Dialogs/DialogBox.ascx");

            this.currentForumPage.Notification = this.NotificationBox;

            this.Controls.Add(this.NotificationBox);

            this.Controls.Add(this.currentForumPage);

            // add the footer control after the page...
            if (BoardContext.Current.Settings.LockedForum == 0)
            {
                this.Controls.Add(this.Footer);
            }

            // Add image gallery dialog
            this.Controls.Add(this.LoadControl($"{BoardInfo.ForumServerFileRoot}Dialogs/ImageGallery.ascx"));

            var cookieName = "YAF-AcceptCookies";

            if (BoardContext.Current.Get<HttpRequestBase>().Cookies[cookieName] == null
                && this.Get<BoardSettings>().ShowCookieConsent && !Config.IsAnyPortal)
            {
                // Add cookie consent
                this.Controls.Add(this.LoadControl($"{BoardInfo.ForumServerFileRoot}controls/CookieConsent.ascx"));
            }

            // Add smart Scroll
            /*if (Config.IsAnyPortal)
            {
                this.Controls.Add(new SmartScroller());
            }*/

            if (this.Get<BoardSettings>().ShowScrollBackToTopButton)
            {
                // Add Scroll top button
                this.Controls.Add(this.LoadControl($"{BoardInfo.ForumServerFileRoot}controls/ScrollTop.ascx"));
            }

            // load plugins/functionality modules
            this.AfterForumPageLoad?.Invoke(this, new AfterForumPageLoad());

            base.OnLoad(e);
        }

        /// <summary>
        /// The task module running.
        /// </summary>
        private static void TaskModuleRunning()
        {
            if (HttpContext.Current.Application[Constants.Cache.TaskModule] != null)
            {
                return;
            }

#if DEBUG
            throw new TaskModuleNotRegisteredException(
                @"YAF.NET is not setup properly. Please add the <add name=""TaskModule"" type=""YAF.Core.TaskModule, YAF.Core"" /> to the <modules> section of your web.config file.");
#else

            // YAF is not setup properly...
            HttpContext.Current.Session["StartupException"] =
                @"YAF.NET is not setup properly. Please add the <add name=""TaskModule"" type=""YAF.Core.TaskModule, YAF.Core"" /> to the <modules> section of your web.config file.";

            // go immediately to the error page.
            HttpContext.Current.Response.Redirect($"{BoardInfo.ForumClientFileRoot}error.aspx");

#endif
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

            if (this.Get<HttpRequestBase>().QueryString.Exists("g"))
            {
                var pageQuery = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("g");

                this.page = pages.GetPage(pageQuery);
            }

            if (this.page == null)
            {
                this.page = pages.GetPage("forum");
            }

            var src = $"{BoardInfo.ForumServerFileRoot}pages/{this.page.PageName}.ascx";

            var replacementPaths = new List<string> { "moderate", "admin", "help" };

            replacementPaths.Where(path => src.IndexOf($"/{path}_", StringComparison.Ordinal) >= 0)
                .ForEach(path => { src = src.Replace($"/{path}_", $"/{path}/"); });

            return src;
        }

        #endregion
    }
}