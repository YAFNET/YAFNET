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

namespace YAF.Pages.moderate
{
    #region Using

    using System;
    using System.Data;
    using System.Web;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Core.Services.CheckForSpam;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    ///  Moderating Page for Reported Posts.
    /// </summary>
    public partial class reportedposts : ModerateForumPage
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "reportedposts" /> class. 
        ///   Default constructor.
        /// </summary>
        public reportedposts()
            : base("MODERATE_FORUM")
        {
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

            // moderation index
            this.PageLinks.AddLink(this.GetText("MODERATE_DEFAULT", "TITLE"), YafBuildLink.GetLink(ForumPages.moderate_index));

            // current page
            this.PageLinks.AddLink(this.PageContext.PageForumName);
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
        protected string FormatMessage([NotNull] DataRowView row)
        {
            // get message flags
            var messageFlags = new MessageFlags(row["Flags"]);

            // message
            string msg;

            // format message?
            if (messageFlags.NotFormatted)
            {
                // just encode it for HTML output
                msg = this.HtmlEncode(row["OriginalMessage"].ToString());
            }
            else
            {
                // fully format message (YafBBCode, smilies)
                msg = this.Get<IFormatMessage>().FormatMessage(
                  row["OriginalMessage"].ToString(), messageFlags, Convert.ToBoolean(row["IsModeratorChanged"]));
            }

            // return formatted message
            return msg;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            this.List.ItemCommand += this.List_ItemCommand;

            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            this.InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Handles page load event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            // do this just on page load, not postbacks
            if (this.IsPostBack)
            {
                return;
            }

            // create page links
            this.CreatePageLinks();

            // bind data
            this.BindData();
        }

        /// <summary>
        /// Bind data for this control.
        /// </summary>
        private void BindData()
        {
            // get reported posts for this forum
            this.List.DataSource = LegacyDb.message_listreported(this.PageContext.PageForumID);

            // bind data to controls
            this.DataBind();
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        ///   the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }

        /// <summary>
        /// Handles post moderation events/buttons.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.RepeaterCommandEventArgs"/> instance containing the event data.</param>
        private void List_ItemCommand([NotNull] object sender, [NotNull] RepeaterCommandEventArgs e)
        {
            // which command are we handling
            switch (e.CommandName.ToLower())
            {
                case "delete":

                    // delete message
                    LegacyDb.message_delete(e.CommandArgument, true, string.Empty, 1, true);

                    // Update statistics
                    this.Get<IDataCache>().Remove(Constants.Cache.BoardStats);

                    // re-bind data
                    this.BindData();

                    // tell user message was deleted
                    this.PageContext.AddLoadMessage(this.GetText("DELETED"));
                    break;
                case "view":

                    // go to the message
                    YafBuildLink.Redirect(ForumPages.posts, "m={0}#post{0}", e.CommandArgument);
                    break;
                case "copyover":

                    // re-bind data
                    this.BindData();

                    // update message text
                    LegacyDb.message_reportcopyover(e.CommandArgument);
                    break;
                case "viewhistory":

                    // go to history page
                    var ff = e.CommandArgument.ToString().Split(',');
                    YafContext.Current.Get<HttpResponseBase>().Redirect(
                      YafBuildLink.GetLinkNotEscaped(ForumPages.messagehistory, "f={0}&m={1}", ff[0], ff[1]));
                    break;
                case "resolved":

                    // mark message as resolved
                    LegacyDb.message_reportresolve(7, e.CommandArgument, this.PageContext.PageUserID);

                    // re-bind data
                    this.BindData();

                    // tell user message was flagged as resolved
                    this.PageContext.AddLoadMessage(this.GetText("RESOLVEDFEEDBACK"), MessageTypes.success);
                    break;
                case "spam":

                    this.ReportSpam((string)e.CommandArgument);

                    break;
            }

            // see if there are any items left...
            var dt = LegacyDb.message_listreported(this.PageContext.PageForumID);

            if (!dt.HasRows())
            {
                // nope -- redirect back to the moderate main...
                YafBuildLink.Redirect(ForumPages.moderate_index);
            }
        }

        /// <summary>
        /// Report Message as Spam
        /// </summary>
        /// <param name="comment">
        /// The comment.
        /// </param>
        private void ReportSpam(string comment)
        {
            if (!this.Get<YafBoardSettings>().SpamServiceType.Equals(1))
            {
                return;
            }

            var message = BlogSpamNet.ClassifyComment(comment, true);

            this.PageContext.AddLoadMessage(message);
        }

        #endregion
    }
}