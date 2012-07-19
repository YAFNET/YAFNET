/* Yet Another Forum.NET
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

namespace YAF.DotNetNuke
{
    #region Using

    using System;
    using System.Collections;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Security;
    using System.Web.UI.WebControls;

    using YAF.Classes;

    using global::DotNetNuke.Common;
    using global::DotNetNuke.Common.Utilities;
    using global::DotNetNuke.Entities.Modules;
    using global::DotNetNuke.Entities.Users;
    using global::DotNetNuke.Services.Exceptions;
    using global::DotNetNuke.Services.Localization;
    using global::DotNetNuke.Services.Scheduling;

    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.DotNetNuke.Utils;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// User Importer.
    /// </summary>
    public partial class YafDnnModuleImport : PortalModuleBase
    {
        #region Constants and Fields

        /// <summary>
        /// The type full name.
        /// </summary>
        private const string TypeFullName = "YAF.DotNetNuke.YafDnnImportScheduler, YAF.DotNetNuke.Module";

        /// <summary>
        /// The board id.
        /// </summary>
        private int boardId;

        /// <summary>
        /// Gets or sets The New Users Counter
        /// </summary>
        private int NewUsers
        {
            get
            {
                return this.ViewState["NewUsers"] != null ? (int)this.ViewState["NewUsers"] : 0;
            }

            set
            {
                this.ViewState["NewUsers"] = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The add scheduler click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void AddSchedulerClick(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            switch (btn.CommandArgument)
            {
                case "add":
                    this.InstallScheduleClient();
                    btn.CommandArgument = "update";
                    btn.Text = Localization.GetString("UpdateSheduler.Text", this.LocalResourceFile);
                    break;
                case "update":
                    UpdateScheduleItem(GetIdOfScheduleClient(TypeFullName));
                    btn.CommandArgument = "delete";
                    btn.Text = Localization.GetString("DeleteShedulerText", this.LocalResourceFile);
                    break;
                case "delete":
                    RemoveScheduleClient(GetIdOfScheduleClient(TypeFullName));
                    btn.CommandArgument = "add";
                    btn.Text = Localization.GetString("InstallSheduler.Text", this.LocalResourceFile);
                    break;
            }
        }

        /// <summary>
        /// The on init.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnInit(EventArgs e)
        {
            this.Load += this.DotNetNukeModuleImport_Load;

            this.btnImportUsers.Click += this.ImportClick;
            this.Close.Click += this.CloseClick;
            this.btnAddScheduler.Click += this.AddSchedulerClick;

            base.OnInit(e);
        }

        /// <summary>
        /// The get id of schedule client.
        /// </summary>
        /// <param name="typeFullName">
        /// The type full name.
        /// </param>
        /// <returns>
        /// Returns the id of schedule client.
        /// </returns>
        private static int GetIdOfScheduleClient(string typeFullName)
        {
            // get array list of schedule items
            ArrayList schduleItems = SchedulingProvider.Instance().GetSchedule();

            // find schedule item with matching TypeFullName
            foreach (object item in
                schduleItems.Cast<object>().Where(item => ((ScheduleItem)item).TypeFullName == typeFullName))
            {
                return ((ScheduleItem)item).ScheduleID;
            }

            return -1;
        }

        /// <summary>
        /// The remove schedule client.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        private static void RemoveScheduleClient(int id)
        {
            // get the item by id
            ScheduleItem item = SchedulingProvider.Instance().GetSchedule(id);

            // tell the provider to remove the item
            SchedulingProvider.Instance().DeleteSchedule(item);
        }

        /// <summary>
        /// The update schedule item.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        private static void UpdateScheduleItem(int id)
        {
            // get the item by id
            ScheduleItem item = SchedulingProvider.Instance().GetSchedule(id);

            // set property on item
            item.TimeLapse = 60;

            // update item
            SchedulingProvider.Instance().UpdateSchedule(item);
        }

        /// <summary>
        /// The close click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CloseClick(object sender, EventArgs e)
        {
            this.Response.Redirect(Globals.NavigateURL(), true);
        }

        /// <summary>
        /// The create yaf host user.
        /// </summary>
        /// <param name="yafUserId">
        /// The yaf user id.
        /// </param>
        private void CreateYafHostUser(int yafUserId)
        {
            // get this user information...
            DataTable userInfo = LegacyDb.user_list(this.boardId, yafUserId, null, null, null);

            if (userInfo.Rows.Count <= 0)
            {
                return;
            }

            DataRow row = userInfo.Rows[0];

            if (Convert.ToBoolean(row["IsHostAdmin"]))
            {
                return;
            }

            // fix the ishostadmin flag...
            var userFlags = new UserFlags(row["Flags"]) { IsHostAdmin = true };

            // update...
            LegacyDb.user_adminsave(
                this.boardId,
                yafUserId,
                row["Name"],
                row["DisplayName"],
                row["Email"],
                userFlags.BitValue,
                row["RankID"]);
        }

        /// <summary>
        /// Handles the Load event of the DotNetNukeModuleImport control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void DotNetNukeModuleImport_Load(object sender, EventArgs e)
        {
            this.btnImportUsers.Text = Localization.GetString("ImportNow.Text", this.LocalResourceFile);
            this.Close.Text = Localization.GetString("Close.Text", this.LocalResourceFile);
            this.btnAddScheduler.Text = Localization.GetString("InstallSheduler.Text", this.LocalResourceFile);

            try
            {
                this.boardId = int.Parse(this.Settings["forumboardid"].ToString());
            }
            catch (Exception)
            {
                this.boardId = 1;
            }

            if (this.IsPostBack || GetIdOfScheduleClient(TypeFullName) <= 0)
            {
                return;
            }

            string sFile = "{0}App_Data/YafImports.xml".FormatWith(HttpRuntime.AppDomainAppPath);

            var dsSettings = new DataSet();

            try
            {
                dsSettings.ReadXml(sFile);
            }
            catch (Exception)
            {
                var file = new FileStream(sFile, FileMode.Create);
                var sw = new StreamWriter(file);

                sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                sw.WriteLine("<YafImports>");
                sw.WriteLine("<Import PortalId=\"{0}\" BoardId=\"{1}\"/>".FormatWith(this.PortalId, this.boardId));
                sw.WriteLine("</YafImports>");

                sw.Close();
                file.Close();
            }

            bool bUpdateXml = false;

            foreach (DataRow oRow in dsSettings.Tables[0].Rows)
            {
                int iPortal = oRow["PortalId"].ToType<int>();
                int iBoard = oRow["BoardId"].ToType<int>();

                if (iPortal.Equals(this.PortalId) && iBoard.Equals(this.boardId))
                {
                    bUpdateXml = false;
                    break;
                }

                bUpdateXml = true;
            }

            if (bUpdateXml)
            {
                DataRow dr = dsSettings.Tables["Import"].NewRow();

                dr["PortalId"] = this.PortalId.ToString();
                dr["BoardId"] = this.boardId.ToString();

                dsSettings.Tables[0].Rows.Add(dr);

                dsSettings.WriteXml(sFile);
            }

            this.btnAddScheduler.CommandArgument = "delete";
            this.btnAddScheduler.Text = Localization.GetString("DeleteSheduler.Text", this.LocalResourceFile);
        }

        /// <summary>
        /// Import/Update Users and Sync Roles
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ImportClick(object sender, EventArgs e)
        {
            this.NewUsers = 0;

            var users = UserController.GetUsers(this.PortalId);
            users.Sort(new UserComparer());

            try
            {
                // Sync Roles
                var rolesChanged = RoleSyncronizer.SyncronizeAllRoles(this.boardId, this.PortalSettings.PortalId);

                // Import Users
                foreach (UserInfo dnnUserInfo in users)
                {
                    // Get current Dnn user
                    // UserInfo dnnUserInfo = UserController.GetUserById(PortalId, UserId);

                    // get the user from the membership provider
                    MembershipUser dnnUser = Membership.GetUser(dnnUserInfo.Username, true);

                    if (dnnUser == null)
                    {
                        continue;
                    }

                    int yafUserId = LegacyDb.user_get(this.boardId, dnnUser.ProviderUserKey);

                    if (yafUserId.Equals(0))
                    {
                        yafUserId = UserImporter.CreateYafUser(
                            dnnUserInfo,
                            dnnUser,
                            this.boardId,
                            this.PortalSettings,
                            YafContext.Current.Get<YafBoardSettings>());

                        this.NewUsers++;
                    }
                    else
                    {
                        ProfileSyncronizer.UpdateUserProfile(
                            yafUserId,
                            dnnUserInfo,
                            dnnUser,
                            this.PortalSettings.PortalId,
                            this.PortalSettings.GUID,
                            this.boardId);
                    }

                    RoleSyncronizer.SyncronizeUserRoles(this.boardId, this.PortalSettings.PortalId, yafUserId, dnnUserInfo);

                    // super admin check...
                    if (dnnUserInfo.IsSuperUser)
                    {
                        this.CreateYafHostUser(yafUserId);
                    }
                }

                this.lInfo.Text =
                    Localization.GetString("UsersImported.Text", this.LocalResourceFile).FormatWith(this.NewUsers);

                if (rolesChanged)
                {
                    this.lInfo.Text += Localization.GetString("RolesSynced.Text", this.LocalResourceFile);
                }
                else
                {
                    this.lInfo.Text += Localization.GetString("RolesNotSynced.Text", this.LocalResourceFile);
                }

                YafContext.Current.Get<IDataCache>().Clear();

                // DataCache.ClearCache();
                DataCache.ClearPortalCache(this.PortalSettings.PortalId, true);
                this.Session.Clear();
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        /// <summary>
        /// The install schedule client.
        /// </summary>
        private void InstallScheduleClient()
        {
            var item = new ScheduleItem
                {
                    FriendlyName = "YAF DotNetNuke User Importer",
                    TypeFullName = TypeFullName,
                    TimeLapse = 1,
                    TimeLapseMeasurement = "d",
                    RetryTimeLapse = 1,
                    RetryTimeLapseMeasurement = "d",
                    RetainHistoryNum = 5,
                    AttachToEvent = "None",
                    CatchUpEnabled = true,
                    Enabled = true,
                    ObjectDependencies = string.Empty,
                    Servers = string.Empty
                };

            // add item
            SchedulingProvider.Instance().AddSchedule(item);

            var dsSettings = new DataSet();

            string sFile = "{0}App_Data/YafImports.xml".FormatWith(HttpRuntime.AppDomainAppPath);

            try
            {
                dsSettings.ReadXml(sFile);
            }
            catch (Exception)
            {
                var file = new FileStream(sFile, FileMode.Create);
                var sw = new StreamWriter(file);

                sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                sw.WriteLine("<YafImports>");
                sw.WriteLine("<Import PortalId=\"{0}\" BoardId=\"{1}\"/>".FormatWith(this.PortalId, this.boardId));
                sw.WriteLine("</YafImports>");

                sw.Close();
                file.Close();
            }
        }

        #endregion
    }
}