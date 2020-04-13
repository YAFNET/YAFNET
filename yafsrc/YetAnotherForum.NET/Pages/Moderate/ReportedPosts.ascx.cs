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

namespace YAF.Pages.Moderate
{
    #region Using

    using System;
    using System.Web;
    using System.Web.UI.WebControls;

    using YAF.Core.BasePages;
    using YAF.Core.Context;
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
    ///  Moderating Page for Reported Posts.
    /// </summary>
    public partial class ReportedPosts : ModerateForumPage
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "ReportedPosts" /> class. 
        ///   Default constructor.
        /// </summary>
        public ReportedPosts()
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
            this.PageLinks.AddLink(this.GetText("MODERATE_DEFAULT", "TITLE"), BuildLink.GetLink(ForumPages.Moderate_Index));

            // current page
            this.PageLinks.AddLink(this.PageContext.PageForumName);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            this.List.ItemCommand += this.List_ItemCommand;

            base.OnInit(e);
        }

        /// <summary>
        /// Handles page load event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            // do this just on page load, not post-backs
            if (this.IsPostBack)
            {
                return;
            }

            // bind data
            this.BindData();
        }

        /// <summary>
        /// Bind data for this control.
        /// </summary>
        private void BindData()
        {
            // get reported posts for this forum
            this.List.DataSource = this.GetRepository<Message>().ListReportedAsDataTable(this.PageContext.PageForumID);

            // bind data to controls
            this.DataBind();
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
                    this.GetRepository<Message>().Delete(e.CommandArgument.ToType<int>(), true, string.Empty, 1, true);

                    // Update statistics
                    this.Get<IDataCache>().Remove(Constants.Cache.BoardStats);

                    // re-bind data
                    this.BindData();

                    // tell user message was deleted
                    this.PageContext.AddLoadMessage(this.GetText("DELETED"), MessageTypes.info);
                    break;
                case "view":

                    // go to the message
                    BuildLink.Redirect(ForumPages.Posts, "m={0}#post{0}", e.CommandArgument);
                    break;
                case "copyover":

                    // re-bind data
                    this.BindData();

                    // update message text
                    this.GetRepository<Message>().ReportCopyOver(e.CommandArgument.ToType<int>());
                    break;
                case "viewhistory":

                    // go to history page
                    var ff = e.CommandArgument.ToString().Split(',');
                    BoardContext.Current.Get<HttpResponseBase>().Redirect(
                      BuildLink.GetLinkNotEscaped(ForumPages.MessageHistory, "f={0}&m={1}", ff[0], ff[1]));
                    break;
                case "resolved":

                    // mark message as resolved
                    this.GetRepository<Message>().ReportResolve(7, e.CommandArgument.ToType<int>(), this.PageContext.PageUserID);

                    // re-bind data
                    this.BindData();

                    // tell user message was flagged as resolved
                    this.PageContext.AddLoadMessage(this.GetText("RESOLVEDFEEDBACK"), MessageTypes.success);
                    break;
            }

            // see if there are any items left...
            var dt = this.GetRepository<Message>().ListReportedAsDataTable(this.PageContext.PageForumID);

            if (!dt.HasRows())
            {
                // nope -- redirect back to the moderate main...
                BuildLink.Redirect(ForumPages.Moderate_Index);
            }
        }

        #endregion
    }
}