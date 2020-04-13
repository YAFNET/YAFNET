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
namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.Data;
    using System.Linq;
    using System.Net.Mail;
    using System.Threading.Tasks;
    using System.Web.UI.WebControls;

    using YAF.Configuration;
    using YAF.Core;
    using YAF.Core.BasePages;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    ///     Admin Interface to send Mass email's to user groups.
    /// </summary>
    public partial class Mail : AdminPage
    {
        #region Methods

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.IsPostBack)
            {
                return;
            }

            this.BindData();
        }

        /// <summary>
        /// Creates page links for this page.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, BuildLink.GetLink(ForumPages.forum));
            this.PageLinks.AddLink(
                this.GetText("ADMIN_ADMIN", "Administration"),
                BuildLink.GetLink(ForumPages.Admin_Admin));
            this.PageLinks.AddLink(this.GetText("ADMIN_MAIL", "TITLE"), string.Empty);

            this.Page.Header.Title =
                $"{this.GetText("ADMIN_ADMIN", "Administration")} - {this.GetText("ADMIN_MAIL", "TITLE")}";
        }

        /// <summary>
        /// Handles the Click event of the Send control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void SendClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            int? groupId = null;

            if (this.ToList.SelectedItem.Value != "0")
            {
                groupId = this.ToList.SelectedValue.ToType<int>();
            }

            var subject = this.Subject.Text.Trim();

            if (subject.IsNotSet())
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_MAIL", "MSG_SUBJECT"), MessageTypes.danger);
            }
            else
            {
                using (var dt = this.GetRepository<User>().EmailsAsDataTable(this.PageContext.PageBoardID, groupId))
                {
                    Parallel.ForEach(
                        dt.Rows.Cast<DataRow>(),
                        row =>
                            {
                                var from = new MailAddress(
                                    this.Get<BoardSettings>().ForumEmail,
                                    this.Get<BoardSettings>().Name);

                                var to = new MailAddress(row.Field<string>("Email"));

                                this.Get<ISendMail>().Send(
                                    from,
                                    to,
                                    from,
                                    this.Subject.Text.Trim(),
                                    this.Body.Text.Trim(),
                                    null);
                            });
                }

                this.Subject.Text = string.Empty;
                this.Body.Text = string.Empty;
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_MAIL", "MSG_QUEUED"), MessageTypes.success);
            }
        }

        /// <summary>
        /// Send a test email
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        protected void TestSmtpClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            try
            {
                this.Get<ISendMail>().Send(
                    this.TestFromEmail.Text.Trim(),
                    this.TestToEmail.Text.Trim(),
                    this.TestFromEmail.Text.Trim(),
                    this.TestSubject.Text,
                    this.TestBody.Text);

                this.PageContext.AddLoadMessage(this.GetText("TEST_SUCCESS"), MessageTypes.success);
            }
            catch (Exception x)
            {
                this.PageContext.AddLoadMessage(x.Message, MessageTypes.danger);
            }
        }

        /// <summary>
        ///     The bind data.
        /// </summary>
        private void BindData()
        {
            this.ToList.DataSource = this.GetRepository<Group>().List(boardId: this.PageContext.PageBoardID);
            this.DataBind();

            var item = new ListItem(this.GetText("ADMIN_MAIL", "ALL_USERS"), "0");
            this.ToList.Items.Insert(0, item);

            this.TestSubject.Text = this.GetText("TEST_SUBJECT");
            this.TestBody.Text = this.GetText("TEST_BODY");
            this.TestFromEmail.Text = this.Get<BoardSettings>().ForumEmail;
        }

        #endregion
    }
}