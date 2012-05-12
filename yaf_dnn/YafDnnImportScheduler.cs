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
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Security;

    using global::DotNetNuke.Common.Utilities;
    using global::DotNetNuke.Entities.Users;
    using global::DotNetNuke.Security.Roles;
    using global::DotNetNuke.Services.Exceptions;
    using global::DotNetNuke.Services.Scheduling;

    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.DotNetNuke.Controller;
    using YAF.DotNetNuke.Utils;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The yaf dnn import scheduler.
    /// </summary>
    public class YafDnnImportScheduler : SchedulerClient
    {
        #region Constants and Fields

        /// <summary>
        /// The s info.
        /// </summary>
        private string sInfo = string.Empty;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="YafDnnImportScheduler"/> class.
        /// </summary>
        /// <param name="objScheduleHistoryItem">
        /// The obj schedule history item.
        /// </param>
        public YafDnnImportScheduler(ScheduleHistoryItem objScheduleHistoryItem)
        {
            this.ScheduleHistoryItem = objScheduleHistoryItem;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The do work.
        /// </summary>
        public override void DoWork()
        {
            try
            {
                this.GetSettings();

                // report success to the scheduler framework
                this.ScheduleHistoryItem.Succeeded = true;

                this.ScheduleHistoryItem.AddLogNote(this.sInfo);
            }
            catch (Exception exc)
            {
                this.ScheduleHistoryItem.Succeeded = false;
                this.ScheduleHistoryItem.AddLogNote("EXCEPTION: " + exc);
                this.Errored(ref exc);
                Exceptions.LogException(exc);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The create yaf host user.
        /// </summary>
        /// <param name="userId">
        /// The yaf user id.
        /// </param>
        /// <param name="boardId">
        /// The iboard id.
        /// </param>
        private static void CreateYafHostUser(int userId, int boardId)
        {
            // get this user information...
            DataTable userInfo = LegacyDb.user_list(boardId, userId, null, null, null);

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
                boardId, userId, row["Name"], row["DisplayName"], row["Email"], userFlags.BitValue, row["RankID"]);
        }

        /// <summary>
        /// The get settings.
        /// </summary>
        private void GetSettings()
        {
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
                sw.WriteLine("<Import PortalId=\"0\" BoardId=\"1\"/>");
                sw.WriteLine("</YafImports>");

                sw.Close();
                file.Close();

                dsSettings.ReadXml(sFile);
            }

            foreach (DataRow dataRow in dsSettings.Tables[0].Rows)
            {
                int iPortalId = dataRow["PortalId"].ToType<int>();
                int iBoardId = dataRow["BoardId"].ToType<int>();

                this.ImportUsers(iBoardId, iPortalId);
            }
        }

        /// <summary>
        /// The import users.
        /// </summary>
        /// <param name="boardId">The board id.</param>
        /// <param name="portalId">The portal id.</param>
        private void ImportUsers(int boardId, int portalId)
        {
            int iNewUsers = 0;

            var users = UserController.GetUsers(portalId);
            users.Sort(new UserComparer());

            try
            {
                // Sync Roles
                var rolesChanged = RoleSyncronizer.SyncronizeAllRoles(boardId, portalId);

                var yafBoardRoles = Data.GetYafBoardRoles(boardId);

                foreach (UserInfo dnnUserInfo in users)
                {
                    MembershipUser dnnUser = Membership.GetUser(dnnUserInfo.Username, true);

                    if (dnnUser == null)
                    {
                        return;
                    }

                    UserInfo info = dnnUserInfo;

                    /*foreach (var role in dnnUserInfo.Roles.Where(role => !RoleMembershipHelper.IsUserInRole(info.Username, role)))
                    {
                        try
                        {
                            RoleMembershipHelper.AddUserToRole(info.Username, role);
                        }
                        catch
                        {
                            // TODO :Dont do anything when user is already in role ?!
                        }
                    }

                    // check if the user is still part of the dnn role
                    foreach (var yafUserRole in RoleMembershipHelper.GetRolesForUser(info.Username).Where(yafUserRole => !info.IsInRole(yafUserRole)))
                    {
                        RoleMembershipHelper.RemoveUserFromRole(info.Username, yafUserRole);
                    }*/

                    int yafUserId;

                    try
                    {
                        yafUserId = LegacyDb.user_get(boardId, dnnUser.ProviderUserKey);

                        ProfileSyncronizer.UpdateUserProfile(yafUserId, dnnUserInfo, dnnUser, portalId, boardId);

                        var yafUserRoles = Data.GetYafUserRoles(boardId, yafUserId);

                        var roleController = new RoleController();

                        foreach (
                            var role in
                                roleController.GetRolesByUser(dnnUserInfo.UserID, portalId).Where(
                                    role => !RoleMembershipHelper.IsUserInRole(info.Username, role)))
                        {
                            RoleInfo yafRoleFound = null;

                            var updateRole = false;

                            // First Create role in yaf if not exists
                            if (!yafBoardRoles.Any(yafRoleA => yafRoleA.RoleName.Equals(role)))
                            {
                                // If not Create Role in YAF
                                yafRoleFound = new RoleInfo
                                    {
                                        RoleID = (int)RoleSyncronizer.CreateYafRole(role, boardId), 
                                        RoleName = role
                                    };

                                updateRole = true;
                            }
                            else
                            {
                                if (!yafUserRoles.Any(yafRoleC => yafRoleC.RoleName.Equals(role)))
                                {
                                    yafRoleFound = yafBoardRoles.Find(yafRole => yafRole.RoleName.Equals(role));

                                    updateRole = true;
                                }
                            }

                            if (updateRole && yafRoleFound != null)
                            {
                                // add/remove user to yaf role ?!
                                RoleSyncronizer.UpdateUserRole(yafRoleFound, yafUserId, dnnUserInfo.Username, true);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        yafUserId = UserImporter.CreateYafUser(dnnUserInfo, dnnUser, boardId, null);
                        iNewUsers++;
                    }

                    // Create user if Not Exist

                    // super admin check...
                    if (dnnUserInfo.IsSuperUser)
                    {
                        CreateYafHostUser(yafUserId, boardId);
                    }

                    if (YafContext.Current.Settings != null)
                    {
                        RoleMembershipHelper.UpdateForumUser(dnnUser, YafContext.Current.Settings.BoardID);
                    }
                }

                this.sInfo = "{0} User(s) Imported".FormatWith(iNewUsers);

                if (rolesChanged)
                {
                    this.sInfo += ", but all Roles are syncronized!";
                }
                else
                {
                    this.sInfo += ", Roles already syncronized!";
                }

                YafContext.Current.Get<IDataCache>().Clear();

                DataCache.ClearCache();

                // Session.Clear();
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
            }
        }

        #endregion
    }
}