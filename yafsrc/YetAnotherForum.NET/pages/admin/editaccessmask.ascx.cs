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

namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.Data;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// Admin Page for Editing or Creating an Forum Access Mask
    /// </summary>
    public partial class editaccessmask : AdminPage
    {
        #region Methods

        /// <summary>
        /// Cancel Edit and Return Back To Access Mask List Page.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Cancel_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            // get back to access masks administration
            YafBuildLink.Redirect(ForumPages.admin_accessmasks);
        }

        /// <summary>
        /// Creates navigation page links on top of forum (breadcrumbs).
        /// </summary>
        protected override void CreatePageLinks()
        {
            // beard index
            this.PageLinks.AddLink(this.Get<YafBoardSettings>().Name, YafBuildLink.GetLink(ForumPages.forum));

            // administration index
            this.PageLinks.AddLink(
                this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));

            this.PageLinks.AddLink(
                this.GetText("ADMIN_ACCESSMASKS", "TITLE"), YafBuildLink.GetLink(ForumPages.admin_accessmasks));

            // current page label (no link)
            this.PageLinks.AddLink(this.GetText("ADMIN_EDITACCESSMASKS", "TITLE"), string.Empty);

            this.Page.Header.Title = "{0} - {1} - {2}".FormatWith(
                this.GetText("ADMIN_ADMIN", "Administration"),
                this.GetText("ADMIN_ACCESSMASKS", "TITLE"),
                this.GetText("ADMIN_EDITACCESSMASKS", "TITLE"));
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.IsPostBack)
            {
                return;
            }

            this.Save.Text = this.GetText("COMMON", "SAVE");
            this.Cancel.Text = this.GetText("COMMON", "CANCEL");

            // create page links
            this.CreatePageLinks();

            // bind data
            this.BindData();
        }

        /// <summary>
        /// Saves The Access Mask
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Save_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            // retrieve access mask ID from parameter (if applicable)
            int? accessMaskID = null;

            if (this.Request.QueryString.GetFirstOrDefault("i") != null)
            {
                accessMaskID = this.Request.QueryString.GetFirstOrDefaultAs<int>("i");
            }

            if (this.Name.Text.Trim().Length <= 0)
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITACCESSMASKS", "MSG_MASK_NAME"));
                return;
            }

            short sortOrder;

            if (!ValidationHelper.IsValidPosShort(this.SortOrder.Text.Trim()))
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITACCESSMASKS", "MSG_POSITIVE_SORT"));
                return;
            }

            if (!short.TryParse(this.SortOrder.Text.Trim(), out sortOrder))
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITACCESSMASKS", "MSG_NUMBER_SORT"));
                return;
            }

            // save it
            this.GetRepository<AccessMask>().Save(
                accessMaskID,
                this.Name.Text,
                this.ReadAccess.Checked,
                this.PostAccess.Checked,
                this.ReplyAccess.Checked,
                this.PriorityAccess.Checked,
                this.PollAccess.Checked,
                this.VoteAccess.Checked,
                this.ModeratorAccess.Checked,
                this.EditAccess.Checked,
                this.DeleteAccess.Checked,
                this.UploadAccess.Checked,
                this.DownloadAccess.Checked,
                sortOrder);

            // empty out access table
            this.Get<IDbFunction>().Query.activeaccess_reset();

            // clear cache
            this.Get<IDataCache>().Remove(Constants.Cache.ForumModerators);

            // get back to access masks administration
            YafBuildLink.Redirect(ForumPages.admin_accessmasks);
        }

        /* Methods */

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            if (this.Request.QueryString.GetFirstOrDefault("i") != null)
            {
                // load access mask
                using (DataTable dataTable = this.GetRepository<AccessMask>().List(this.Request.QueryString.GetFirstOrDefaultAs<int>("i")))
                {
                    // we need just one
                    DataRow row = dataTable.Rows[0];

                    // get access mask properties
                    this.Name.Text = (string)row["Name"];
                    this.SortOrder.Text = row["SortOrder"].ToString();

                    // get flags
                    var flags = new AccessFlags(row["Flags"]);
                    this.ReadAccess.Checked = flags.ReadAccess;
                    this.PostAccess.Checked = flags.PostAccess;
                    this.ReplyAccess.Checked = flags.ReplyAccess;
                    this.PriorityAccess.Checked = flags.PriorityAccess;
                    this.PollAccess.Checked = flags.PollAccess;
                    this.VoteAccess.Checked = flags.VoteAccess;
                    this.ModeratorAccess.Checked = flags.ModeratorAccess;
                    this.EditAccess.Checked = flags.EditAccess;
                    this.DeleteAccess.Checked = flags.DeleteAccess;
                    this.UploadAccess.Checked = flags.UploadAccess;
                    this.DownloadAccess.Checked = flags.DownloadAccess;
                }
            }

            this.DataBind();
        }

        #endregion
    }
}