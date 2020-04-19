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
    using System.Net.Mail;
    using System.Web;

    using YAF.Core.BasePages;
    using YAF.Core.Extensions;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// The Share Topic via email
    /// </summary>
    public partial class EmailTopic : ForumPage
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "EmailTopic" /> class.
        /// </summary>
        public EmailTopic()
            : base("EMAILTOPIC")
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.Get<HttpRequestBase>().QueryString.Exists("t") || !this.PageContext.ForumReadAccess
                || !this.PageContext.BoardSettings.AllowEmailTopic)
            {
                BuildLink.AccessDenied();
            }

            if (this.IsPostBack)
            {
                return;
            }

            if (this.PageContext.Settings.LockedForum == 0)
            {
                this.PageLinks.AddRoot();

                this.PageLinks.AddCategory(this.PageContext.PageCategoryName, this.PageContext.PageCategoryID);
            }

            this.PageLinks.AddForum(this.PageContext.PageForumID);

            this.PageLinks.AddLink(
                this.PageContext.PageTopicName,
                BuildLink.GetLink(ForumPages.Posts, "t={0}", this.PageContext.PageTopicID));

            this.Subject.Text = this.PageContext.PageTopicName;

            var emailTopic = new TemplateEmail
                                 {
                                     TemplateParams =
                                         {
                                             ["{link}"] =
                                             BuildLink.GetLinkNotEscaped(
                                                 ForumPages.Posts,
                                                 true,
                                                 "t={0}",
                                                 this.PageContext.PageTopicID),
                                             ["{user}"] = this.PageContext.PageUserName
                                         }
                                 };

            this.Message.Text = emailTopic.ProcessTemplate("EMAILTOPIC");
        }

        /// <summary>
        /// Handles the Click event of the SendEmail control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void SendEmail_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.EmailAddress.Text.Length == 0)
            {
                this.PageContext.AddLoadMessage(this.GetText("need_email"), MessageTypes.warning);
                return;
            }

            try
            {
                var emailTopic = new TemplateEmail("EMAILTOPIC")
                                     {
                                         TemplateParams = { ["{message}"] = this.Message.Text.Trim() }
                                     };

                // send a change email message...
                emailTopic.SendEmail(new MailAddress(this.EmailAddress.Text.Trim()), this.Subject.Text.Trim());

                BuildLink.Redirect(ForumPages.Posts, "t={0}", this.PageContext.PageTopicID);
            }
            catch (Exception x)
            {
                this.Logger.Log(this.PageContext.PageUserID, this, x);
                this.PageContext.AddLoadMessage(this.GetTextFormatted("failed", x.Message), MessageTypes.danger);
            }
        }

        #endregion
    }
}