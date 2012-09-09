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
    using System.Linq;
    using System.Web.Security;
    using System.Web.UI.WebControls;

    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// Interface for creating or editing user roles/groups.
    /// </summary>
    public partial class editgroup : AdminPage
    {
        #region Methods

        public DataTable AccessMasksList { get; set; }
        /// <summary>
        /// Handles databinding event of initial access maks dropdown control.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void BindData_AccessMaskID([NotNull] object sender, [NotNull] EventArgs e)
        {
            // We don't change access masks if it's a guest
            if (!this.IsGuestX.Checked)
            {
                // get sender object as dropdown list
                var c = (DropDownList) sender;

                // list all access masks as data source
                c.DataSource = this.AccessMasksList;
                // set value and text field names
                c.DataValueField = "AccessMaskID";
                c.DataTextField = "Name";
            }
            else
            {
                
            }
        }

        /// <summary>
        /// Handles click on cancel button.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Cancel_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            // go back to roles administration
            YafBuildLink.Redirect(ForumPages.admin_groups);
        }

        /// <summary>
        /// Creates page links for this page.
        /// </summary>
        protected override void CreatePageLinks()
        {
            // forum index
            this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));

            // admin index
            this.PageLinks.AddLink(this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));

            this.PageLinks.AddLink(this.GetText("ADMIN_GROUPS", "TITLE"), YafBuildLink.GetLink(ForumPages.admin_groups));

            // current page label (no link)
            this.PageLinks.AddLink(this.GetText("ADMIN_EDITGROUP", "TITLE"), string.Empty);

            this.Page.Header.Title = "{0} - {1} - {2}".FormatWith(
               this.GetText("ADMIN_ADMIN", "Administration"),
               this.GetText("ADMIN_GROUPS", "TITLE"),
               this.GetText("ADMIN_EDITGROUP", "TITLE"));
        }

        /// <summary>
        /// Handles page load event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            // this needs to be done just once, not during postbacks
            if (this.IsPostBack)
            {
                return;
            }

            // create page links
            this.CreatePageLinks();

            this.Save.Text = this.GetText("COMMON", "SAVE");
            this.Cancel.Text = this.GetText("COMMON", "CANCEL");


            // bind data
            this.BindData();
            // is this editing of existing role or creation of new one?
            if (this.Request.QueryString.GetFirstOrDefault("i") == null)
            {
                return;
            }

            // we are not creating new role
            this.NewGroupRow.Visible = false;

            // get data about edited role
            using (
                DataTable dt = LegacyDb.group_list(this.PageContext.PageBoardID, this.Request.QueryString.GetFirstOrDefault("i")))
            {
                // get it as row
                DataRow row = dt.Rows[0];

                // get role flags
                var flags = new GroupFlags(row["Flags"]);

                // set controls to role values
                this.Name.Text = (string)row["Name"];

                this.IsAdminX.Checked = flags.IsAdmin;
                this.IsAdminX.Enabled = !flags.IsGuest;

                this.IsStartX.Checked = flags.IsStart;
                this.IsStartX.Enabled = !flags.IsGuest;

                this.IsModeratorX.Checked = flags.IsModerator;
                this.IsModeratorX.Enabled = !flags.IsGuest;

                this.PMLimit.Text = row["PMLimit"].ToString();
                this.PMLimit.Enabled = !flags.IsGuest;

                this.StyleTextBox.Text = row["Style"].ToString();

                this.Priority.Text = row["SortOrder"].ToString();

                this.UsrAlbums.Text = row["UsrAlbums"].ToString();
                this.UsrAlbums.Enabled = !flags.IsGuest;

                this.UsrAlbumImages.Text = row["UsrAlbumImages"].ToString();
                this.UsrAlbumImages.Enabled = !flags.IsGuest;

                this.UsrSigChars.Text = row["UsrSigChars"].ToString();
                this.UsrSigChars.Enabled = !flags.IsGuest;

                this.UsrSigBBCodes.Text = row["UsrSigBBCodes"].ToString();
                this.UsrSigBBCodes.Enabled = !flags.IsGuest;

                this.UsrSigHTMLTags.Text = row["UsrSigHTMLTags"].ToString();
                this.UsrSigHTMLTags.Enabled = !flags.IsGuest;

                this.Description.Text = row["Description"].ToString();

                this.IsGuestX.Checked = flags.IsGuest;

                // IsGuest flag can be set for only one role. if it isn't for this, disable that row
                if (flags.IsGuest)
                {
                    this.IsGuestTR.Visible = true;
                    this.IsGuestX.Enabled = !flags.IsGuest;
                    this.AccessList.Visible = false;
                }

            }
        }

        /// <summary>
        /// Handles click on save button.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Save_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!ValidationHelper.IsValidInt(this.PMLimit.Text.Trim()))
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITGROUP", "MSG_VALID_NUMBER"));
                return;
            }

            if (!ValidationHelper.IsValidInt(this.Priority.Text.Trim()))
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITGROUP", "MSG_INTEGER"));
                return;
            }

            if (!ValidationHelper.IsValidInt(this.UsrAlbums.Text.Trim()))
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITGROUP", "MSG_ALBUM_NUMBER"));
                return;
            }

            if (!ValidationHelper.IsValidInt(this.UsrSigChars.Text.Trim()))
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITGROUP", "MSG_SIG_NUMBER"));
                return;
            }

            if (!ValidationHelper.IsValidInt(this.UsrAlbumImages.Text.Trim()))
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITGROUP", "MSG_TOTAL_NUMBER"));
                return;
            }

            // Role
            long roleID = 0;

            // get role ID from page's parameter
            if (this.Request.QueryString.GetFirstOrDefault("i") != null)
            {
                roleID = long.Parse(this.Request.QueryString.GetFirstOrDefault("i"));
            }

            // get new and old name
            string roleName = this.Name.Text.Trim();
            string oldRoleName = string.Empty;

            // if we are editing exising role, get it's original name
            if (roleID != 0)
            {
                // get the current role name in the DB
                using (DataTable dt = LegacyDb.group_list(YafContext.Current.PageBoardID, roleID))
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        oldRoleName = row["Name"].ToString();
                    }
                }
            }

            // save role and get its ID if it's new (if it's old role, we get it anyway)
            roleID = LegacyDb.group_save(
              roleID,
              this.PageContext.PageBoardID,
              roleName,
              this.IsAdminX.Checked,
              this.IsGuestX.Checked,
              this.IsStartX.Checked,
              this.IsModeratorX.Checked,
              AccessMaskID.SelectedValue,
              this.PMLimit.Text.Trim(),
              this.StyleTextBox.Text.Trim(),
              this.Priority.Text.Trim(),
              this.Description.Text,
              this.UsrSigChars.Text,
              this.UsrSigBBCodes.Text,
              this.UsrSigHTMLTags.Text,
              this.UsrAlbums.Text.Trim(),
              this.UsrAlbumImages.Text.Trim());

            // empty out access table
            LegacyDb.activeaccess_reset();

            // see if need to rename an existing role...
            if (oldRoleName.IsSet() && roleName != oldRoleName && RoleMembershipHelper.RoleExists(oldRoleName) && !RoleMembershipHelper.RoleExists(roleName) && !this.IsGuestX.Checked)
            {
                // transfer users in addition to changing the name of the role...
                var users = this.Get<RoleProvider>().GetUsersInRole(oldRoleName);

                // delete the old role...
                RoleMembershipHelper.DeleteRole(oldRoleName, false);

                // create new role...
                RoleMembershipHelper.CreateRole(roleName);

                if (users.Any())
                {
                    // put users into new role...
                    this.Get<RoleProvider>().AddUsersToRoles(users, new[] { roleName });
                }
            }
            else if (!RoleMembershipHelper.RoleExists(roleName) && !this.IsGuestX.Checked)
            {
                // if role doesn't exist in provider's data source, create it

                // simply create it
                RoleMembershipHelper.CreateRole(roleName);
            }

            // Access masks for a newly created or an existing role
            if (this.Request.QueryString.GetFirstOrDefault("i") != null)
            {
                    // go trhough all forums
                    for (int i = 0; i < this.AccessList.Items.Count; i++)
                    {
                        // get current repeater item
                        RepeaterItem item = this.AccessList.Items[i];

                        // get forum ID
                        int forumID = int.Parse(((Label)item.FindControl("ForumID")).Text);

                        // save forum access maks for this role
                        LegacyDb.forumaccess_save(forumID, roleID,
                                                  ((DropDownList)item.FindControl("AccessmaskID")).SelectedValue);
                    }

                YafBuildLink.Redirect(ForumPages.admin_groups);
            }

            // remove caching in case something got updated...
            this.Get<IDataCache>().Remove(Constants.Cache.ForumModerators);

            // Clearing cache with old permissions data...
            this.Get<IDataCache>().Remove(k => k.StartsWith(Constants.Cache.ActiveUserLazyData.FormatWith(String.Empty)));

            // Done, redirect to role editing page
            YafBuildLink.Redirect(ForumPages.admin_editgroup, "i={0}", roleID);
        }

        /// <summary>
        /// Handles pre-render event of each forum's access mask dropdown.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void SetDropDownIndex([NotNull] object sender, [NotNull] EventArgs e)
        {
            // get dropdown which raised this event
            var list = (DropDownList)sender;

            // select value from the list
            ListItem item = list.Items.FindByValue(list.Attributes["value"]);

            // verify something was found...
            if (item != null)
            {
                item.Selected = true;
            }

        }

        /// <summary>
        /// Bind data for this control.
        /// </summary>
        private void BindData()
        {
            // set datasource of access list (list of forums and role's access masks) if we are editing existing mask
            if (this.Request.QueryString.GetFirstOrDefault("i") != null)
            {
                this.AccessList.DataSource = LegacyDb.forumaccess_group(this.Request.QueryString.GetFirstOrDefault("i"));
            }
            this.AccessMasksList = LegacyDb.accessmask_list(this.PageContext.PageBoardID, null);
            // bind data to controls
            this.DataBind();

        }

        #endregion
    }
}