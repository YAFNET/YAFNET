/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2012 Jaben Cargman
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

namespace YAF.Pages.moderate
{
    #region Using

    using System;
    using System.Data;
    using System.Web.UI.WebControls;

    using YAF.Classes;
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
    /// Moderating Page for Unapproved Posts.
    /// </summary>
    public partial class unapprovedposts : ModerateForumPage
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "unapprovedposts" /> class. 
        ///   Default constructor.
        /// </summary>
        public unapprovedposts()
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
            this.PageLinks.AddLink(this.Get<YafBoardSettings>().Name, YafBuildLink.GetLink(ForumPages.forum));

            // moderation index
            this.PageLinks.AddLink(this.GetText("MODERATE_DEFAULT", "TITLE"), YafBuildLink.GetLink(ForumPages.moderate_index));

            // current page
            this.PageLinks.AddLink(this.PageContext.PageForumName);
        }

        /// <summary>
        /// Handles load event for delete button, adds confirmation dialog.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Delete_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            var button = sender as ThemeButton;
            if (button != null)
            {
                button.Attributes["onclick"] = "return confirm('{0}');".FormatWith(this.GetText("ASK_DELETE"));
            }
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
                msg = this.HtmlEncode(row["Message"].ToString());
            }
            else
            {
                // fully format message (YafBBCode, smilies)
                msg = this.Get<IFormatMessage>().FormatMessage(
                  row["Message"].ToString(), messageFlags, Convert.ToBoolean(row["IsModeratorChanged"]));
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
            // get unapproved posts for this forum
            this.List.DataSource = LegacyDb.message_unapproved(this.PageContext.PageForumID);

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
                case "approve":

                    // approve post
                    LegacyDb.message_approve(e.CommandArgument);

                    // Update statistics
                    this.Get<IDataCache>().Remove(Constants.Cache.BoardStats);

                    // re-bind data
                    this.BindData();

                    // tell user message was approved
                    this.PageContext.AddLoadMessage(this.GetText("APPROVED"), MessageTypes.Success);

                    // send notification to watching users...
                    this.Get<ISendNotification>().ToWatchingUsers(e.CommandArgument.ToType<int>());
                    break;
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
            }

            // see if there are any items left...
            DataTable dt = LegacyDb.message_unapproved(this.PageContext.PageForumID);

            if (dt.Rows.Count == 0)
            {
                // nope -- redirect back to the moderate main...
                YafBuildLink.Redirect(ForumPages.moderate_index);
            }
        }

        #endregion
    }
}