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
namespace YAF.Pages
{
    #region Using

    using System;
    using System.Web;

    using YAF.Core.BasePages;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// Information control displaying feedback information to users.
    /// </summary>
    public partial class Info : ForumPage
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Info" /> class. 
        ///   Default constructor.
        /// </summary>
        public Info()
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
            if (this.Get<HttpRequestBase>().QueryString.Exists("url"))
            {
                // un-escape ampersands
                this.RefreshURL = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("url")
                    .Replace("&amp;", "&");
            }

            // try to get info message code from parameter
            try
            {
                // compare it converted to enumeration
                switch ((InfoMessage)this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAs<int>("i"))
                {
                    case InfoMessage.Moderated: // Moderated
                        this.Title.Text = this.GetText("title_moderated");
                        this.InfoLabel.Text = this.GetText("moderated");
                        this.RefreshTime = 10;
                        break;
                    case InfoMessage.Suspended: // Suspended
                        this.Title.Text = this.GetText("title_suspended");

                        if (this.PageContext.SuspendedReason.IsSet())
                        {
                            this.InfoLabel.Text =
                                $"{this.GetTextFormatted("SUSPENDED", this.Get<IDateTime>().GetUserDateTime(this.PageContext.SuspendedUntil))}{this.GetTextFormatted("SUSPENDED_REASON", this.PageContext.SuspendedReason)}";
                        }
                        else
                        {
                            this.InfoLabel.Text = this.GetTextFormatted(
                                "SUSPENDED",
                                this.Get<IDateTime>().GetUserDateTime(this.PageContext.SuspendedUntil));
                        }

                        break;
                    case InfoMessage.RegistrationEmail: // Registration email
                        this.Title.Text = this.GetText("title_registration");
                        this.InfoLabel.Text = this.GetText("registration");
                        this.RefreshTime = 10;
                        this.RefreshURL = BuildLink.GetLink(ForumPages.Login);
                        break;
                    case InfoMessage.AccessDenied: // Access Denied
                        this.Title.Text = this.GetText("title_accessdenied");
                        this.InfoLabel.Text = this.GetText("accessdenied");
                        this.RefreshTime = 10;
                        this.RefreshURL = BuildLink.GetLink(ForumPages.forum);
                        break;
                    case InfoMessage.Disabled: // Disabled feature
                        this.Title.Text = this.GetText("TITLE_ACCESSDENIED");
                        this.InfoLabel.Text = this.GetText("DISABLED");
                        this.RefreshTime = 10;
                        this.RefreshURL = BuildLink.GetLink(ForumPages.forum);
                        break;
                    case InfoMessage.Invalid: // Invalid argument!
                        this.Title.Text = this.GetText("TITLE_INVALID");
                        this.InfoLabel.Text = this.GetText("INVALID");
                        this.RefreshTime = 10;
                        this.RefreshURL = BuildLink.GetLink(ForumPages.forum);
                        break;
                    case InfoMessage.Failure: // some sort of failure
                        this.Title.Text = this.GetText("TITLE_FAILURE");
                        this.InfoLabel.Text = this.GetText("FAILURE");
                        this.RefreshTime = 10;
                        this.RefreshURL = BuildLink.GetLink(ForumPages.forum);
                        break;
                    case InfoMessage.HostAdminPermissionsAreRequired: // some sort of failure
                        this.Title.Text = this.GetText("TITLE_HOSTADMINPERMISSIONSREQUIRED");
                        this.InfoLabel.Text = this.GetText("HOSTADMINPERMISSIONSREQUIRED");
                        this.RefreshTime = 10;
                        this.RefreshURL = BuildLink.GetLink(ForumPages.forum);
                        break;
                }
            }
            catch (Exception)
            {
                // get title for exception message
                this.Title.Text = this.GetText("title_exception");

                // exception message
                this.InfoLabel.Text = string.Format(
                    "{1} <strong>{0}</strong>.",
                    this.PageContext.PageUserName,
                    this.GetText("exception"));

                // redirect to forum main after 2 seconds
                this.RefreshTime = 2;
                this.RefreshURL = BuildLink.GetLink(ForumPages.forum);
            }

            // set continue button URL and visibility
            this.Continue.NavigateUrl = this.RefreshURL;
            this.Continue.Visible = this.RefreshURL != null;
        }

        #endregion
    }
}