/* Yet Another Forum.NET
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
namespace YAF.Pages
{
    #region Using

    using System;
    using System.Data;
    using System.Web;

    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
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
        ///   To save single report value.
        /// </summary>
        protected bool singleReport;

        /// <summary>
        ///   To save forumid value.
        /// </summary>
        private int forumID;

        /// <summary>
        ///   To save messageid value.
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
        /// Binds data to data source
        /// </summary>
        private void BindData()
        {
            // Fill revisions list repeater.
            DataTable dt = LegacyDb.messagehistory_list(
                this.messageID,
                this.PageContext.BoardSettings.MessageHistoryDaysToLog);
            this.RevisionsList.DataSource = dt.AsEnumerable();

            this.singleReport = dt.Rows.Count <= 1;

            // Fill current message repeater
            this.CurrentMessageRpt.DataSource =
                LegacyDb.message_secdata(this.messageID, this.PageContext.PageUserID).AsEnumerable();
            this.CurrentMessageRpt.Visible = true;

            this.DataBind();
        }

        #endregion
    }
}