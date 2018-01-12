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
    using System.Data;
    using System.Linq;
    using System.Web;
    using System.Web.UI.WebControls;

    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// Post Message History Page.
    /// </summary>
    public partial class messagehistory : ForumPage
    {
        #region Constants and Fields

        /// <summary>
        ///   To save Forum ID value.
        /// </summary>
        private int forumID;

        /// <summary>
        ///   To save Message ID value.
        /// </summary>
        private int messageID;

        /// <summary>
        ///   To save originalRow value.
        /// </summary>
        private DataTable originalRow;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "messagehistory" /> class. 
        /// </summary>
        public messagehistory()
            : base("MESSAGEHISTORY")
        {
        }

        #endregion

        /// <summary>
        /// Gets or sets the revisions count.
        /// </summary>
        /// <value>
        /// The revisions count.
        /// </value>
        protected int RevisionsCount { get; set; }

        #region Methods

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.PageContext.IsGuest)
            {
                YafBuildLink.AccessDenied();
            }

            if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("m").IsSet())
            {
                if (!int.TryParse(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("m"), out this.messageID))
                {
                    this.Response.Redirect(
                        YafBuildLink.GetLink(ForumPages.error, "Incorrect message value: {0}", this.messageID));
                }

                this.ReturnBtn.Visible = true;
            }

            if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("f").IsSet())
            {
                // We check here if the user have access to the option
                if (this.PageContext.IsGuest)
                {
                    this.Response.Redirect(YafBuildLink.GetLinkNotEscaped(ForumPages.info, "i=4"));
                }

                if (!int.TryParse(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("f"), out this.forumID))
                {
                    this.Response.Redirect(
                        YafBuildLink.GetLink(ForumPages.error, "Incorrect forum value: {0}", this.forumID));
                }

                this.ReturnModBtn.Visible = true;
            }

            this.originalRow = LegacyDb.message_secdata(this.messageID, this.PageContext.PageUserID);

            if (this.originalRow.Rows.Count <= 0)
            {
                this.Response.Redirect(
                    YafBuildLink.GetLink(ForumPages.error, "Incorrect message value: {0}", this.messageID));
            }

            if (this.IsPostBack)
            {
                return;
            }

            this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
            this.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);

            this.BindData();
        }

        /// <summary>
        /// Compares the selected Versions
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void CompareVersions_OnClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            //TODO:
        }

        /// <summary>
        /// Redirect to the changed post
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void ReturnBtn_OnClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.Response.Redirect(YafBuildLink.GetLinkNotEscaped(ForumPages.posts, "m={0}#post{0}", this.messageID));
        }

        /// <summary>
        /// Redirect to the changed post
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void ReturnModBtn_OnClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.Response.Redirect(
                YafBuildLink.GetLinkNotEscaped(ForumPages.moderate_reportedposts, "f={0}", this.forumID));
        }

        /// <summary>
        /// Handle Commands for restoring an old Message Version
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="RepeaterCommandEventArgs"/> instance containing the event data.</param>
        protected void RevisionsList_ItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "restore":
                    var currentMessage = LegacyDb.MessageList(this.messageID).FirstOrDefault();

                    DataRow restoreMessage = null;

                    var revisionsTable = LegacyDb.messagehistory_list(
                        this.messageID,
                        this.PageContext.BoardSettings.MessageHistoryDaysToLog).AsEnumerable();

                    foreach (var row in Enumerable.Where(revisionsTable, row => row["Edited"].ToType<string>().Equals(e.CommandArgument.ToType<string>())))
                    {
                        restoreMessage = row;
                    }

                    if (restoreMessage != null)
                    {

                        LegacyDb.message_update(
                            this.messageID,
                            currentMessage.Priority,
                            restoreMessage["Message"],
                            currentMessage.Description,
                            currentMessage.Status,
                            currentMessage.Styles,
                            currentMessage.Topic,
                            currentMessage.Flags.BitValue,
                            restoreMessage["EditReason"],
                            this.PageContext.PageUserID != currentMessage.UserID,
                            this.PageContext.IsAdmin || this.PageContext.ForumModeratorAccess,
                            currentMessage.Message,
                            this.PageContext.PageUserID);

                        this.PageContext.AddLoadMessage(this.GetText("MESSAGE_RESTORED"), MessageTypes.success);
                    }

                    break;
            }
        }

        /// <summary>
        /// Add Confirm Dialog to the Restore Message Button
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void RestoreVersion_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            ((ThemeButton)sender).Attributes["onclick"] =
                "return confirm('{0}')".FormatWith(this.GetText("MESSAGEHISTORY", "CONFIRM_RESTORE"));
        }

        /// <summary>
        /// Format message.
        /// </summary>
        /// <param name="row">
        /// Message data row.
        /// </param>
        /// <returns>
        /// Formatted string with escaped HTML markup and formatted.
        /// </returns>
        protected string FormatMessage([NotNull] DataRow row)
        {
            // get message flags
            var messageFlags = new MessageFlags(row["Flags"]);

            // message
            string msg;

            // format message?
            if (messageFlags.NotFormatted)
            {
                // just encode it for HTML output
                msg = this.HtmlEncode(row["Message"].ToString());
            }
            else
            {
                // fully format message (YafBBCode, smilies)
                msg = this.Get<IFormatMessage>().FormatMessage(
                  row["Message"].ToString(), messageFlags, row["IsModeratorChanged"].ToType<bool>());
            }

            // return formatted message
            return msg;
        }
        
        /// <summary>
        /// Binds data to data source
        /// </summary>
        private void BindData()
        {
            // Fill revisions list repeater.
            var revisionsTable = LegacyDb.messagehistory_list(
                this.messageID,
                this.PageContext.BoardSettings.MessageHistoryDaysToLog);

            revisionsTable.AcceptChanges();

            revisionsTable.Merge(LegacyDb.message_secdata(this.messageID, this.PageContext.PageUserID));

            this.RevisionsCount = revisionsTable.Rows.Count;

            this.RevisionsList.DataSource = revisionsTable.AsEnumerable();

            this.DataBind();
        }

        #endregion
    }
}