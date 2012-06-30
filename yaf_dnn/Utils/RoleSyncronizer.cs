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

namespace YAF.DotNetNuke.Utils
{
    using System;
    using System.Linq;

    using global::DotNetNuke.Entities.Modules;
    using global::DotNetNuke.Entities.Users;
    using global::DotNetNuke.Security.Roles;

    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.DotNetNuke.Controller;

    /// <summary>
    /// YAF DNN Profile Syncronization 
    /// </summary>
    public class RoleSyncronizer : PortalModuleBase
    {
        /// <summary>
        /// Syncronizes the user roles.
        /// </summary>
        /// <param name="boardId">The board id.</param>
        /// <param name="portalId">The portal id.</param>
        /// <param name="yafUserId">The yaf user id.</param>
        /// <param name="dnnUserInfo">The DNN user info.</param>
        public static void SyncronizeUserRoles(int boardId, int portalId, int yafUserId, UserInfo dnnUserInfo)
        {
            var yafUserRoles = Data.GetYafUserRoles(boardId, yafUserId);

            var yafBoardRoles = Data.GetYafBoardRoles(boardId);

            var roleController = new RoleController();

            foreach (var role in
                roleController.GetRolesByUser(dnnUserInfo.UserID, portalId).Where(
                    role => !RoleMembershipHelper.IsUserInRole(dnnUserInfo.Username, role)))
            {
                RoleInfo yafRoleFound = null;

                var updateRole = false;

                // First Create role in yaf if not exists
                if (!yafBoardRoles.Any(yafRoleA => yafRoleA.RoleName.Equals(role)))
                {
                    // If not Create Role in YAF
                    yafRoleFound = new RoleInfo { RoleID = (int)CreateYafRole(role, boardId), RoleName = role };

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
                    UpdateUserRole(yafRoleFound, yafUserId, dnnUserInfo.Username, true);
                }
            }

            LegacyDb.activeaccess_reset();
        }

        /// <summary>
        /// Syncronizes the yaf roles.
        /// </summary>
        /// <param name="boardId">The board id.</param>
        /// <param name="portalId">The portal id.</param>
        /// <returns>
        /// If Roles where synced or not
        /// </returns>
        public static bool SyncronizeAllRoles(int boardId, int portalId)
        {
            var rolesChanged = false;

            try
            {
                var dnnPortalRoles = Data.GetDnnPortalRoles(portalId);
                var yafBoardRoles = Data.GetYafBoardRoles(boardId);

                // Check If Dnn Roles Exists in Yaf
                foreach (RoleInfo dnnRole in
                    dnnPortalRoles.Where(
                        dnnRole => !yafBoardRoles.Any(yafRole => yafRole.RoleName.Equals(dnnRole.RoleName))))
                {
                    // If not Create Role in YAF
                    try
                    {
                        CreateYafRole(dnnRole.RoleName, boardId);
                    }
                    catch (Exception)
                    {
                        rolesChanged = false;
                    }
                   
                    rolesChanged = true;
                }

                // Check if Yaf Role Still exists in DNN
                foreach (RoleInfo yafRole in
                    yafBoardRoles.Where(
                        yafRole => !yafRole.RoleName.Equals("Guests") && !yafRole.RoleName.Equals("Registered")).Where(
                            yafRole => !dnnPortalRoles.Any(dnnRole => dnnRole.Equals(yafRole.RoleName))))
                {
                    LegacyDb.group_delete(yafRole.RoleID);

                    rolesChanged = true;
                }

                if (rolesChanged)
                {
                    // sync roles just in case...
                    RoleMembershipHelper.SyncRoles(boardId);
                }
            }
            catch (Exception)
            {
                rolesChanged = false;
            }

            return rolesChanged;
        }

        /// <summary>
        /// Creates the yaf role.
        /// </summary>
        /// <param name="roleName">Name of the role.</param>
        /// <param name="boardId">The board id.</param>
        /// <returns>Returns the Role id of the created Role</returns>
        public static long CreateYafRole(string roleName, int boardId)
        {
            // If not Create Role in YAF
            if (!RoleMembershipHelper.RoleExists(roleName))
            {
                RoleMembershipHelper.CreateRole(roleName);
            }

            // Role exists in membership but not in yaf itself simply add it to yaf
            return LegacyDb.group_save(
                DBNull.Value,
                boardId,
                roleName,
                false,
                false,
                false,
                false,
                1,
                0,
                null,
                100,
                null,
                0,
                null,
                null,
                0,
                0);
        }

        /// <summary>
        /// Updates the user role.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="yafUserId">The yaf user id.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="addRole">if set to <c>true</c> [add role].</param>
        public static void UpdateUserRole(RoleInfo role, int yafUserId, string userName, bool addRole)
        {
            // save user in role
            LegacyDb.usergroup_save(yafUserId, role.RoleID, addRole);

            if (addRole && !RoleMembershipHelper.IsUserInRole(userName, role.RoleName))
            {
                RoleMembershipHelper.AddUserToRole(userName, role.RoleName);
            }
            else if (!addRole && RoleMembershipHelper.IsUserInRole(userName, role.RoleName))
            {
                RoleMembershipHelper.RemoveUserFromRole(userName, role.RoleName);
            }
        }
    }
}