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
    #region Using

    using System;

    using YAF.Controls;
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
            this.PageLinks.AddRoot();

            // information title text
            this.PageLinks.AddLink(this.Title.Text);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            this.InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            // Put user code to initialize the page here
            if (this.IsPostBack)
            {
                return;
            }

            // localize button label
            this.Continue.TextLocalizedTag = "CONTINUE";

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
                switch ((InfoMessage)this.Request.QueryString.GetFirstOrDefaultAs<int>("i"))
                {
                    case InfoMessage.Moderated: // Moderated
                        this.Title.Text = this.GetText("title_moderated");
                        this.Info.Text = this.GetText("moderated");
                        this.RefreshTime = 10;
                        break;
                    case InfoMessage.Suspended: // Suspended
                        this.Title.Text = this.GetText("title_suspended");

                        if (this.PageContext.SuspendedReason.IsSet())
                        {
                            this.Info.Text =
                                "{0}{1}".FormatWith(
                                    this.GetTextFormatted(
                                        "SUSPENDED",
                                        this.Get<IDateTime>().GetUserDateTime(this.PageContext.SuspendedUntil)),
                                    this.GetTextFormatted("SUSPENDED_REASON", this.PageContext.SuspendedReason));
                        }
                        else
                        {
                            this.Info.Text = this.GetTextFormatted(
                                "SUSPENDED",
                                this.Get<IDateTime>().GetUserDateTime(this.PageContext.SuspendedUntil));
                        }
                            
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