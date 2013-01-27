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

namespace YAF.DotNetNuke.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using global::DotNetNuke.Entities.Modules;

    using global::DotNetNuke.Entities.Users;

    using global::DotNetNuke.Security.Roles;

    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Core.Model;
    using YAF.DotNetNuke.Controller;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;

    /// <summary>
    /// YAF DNN Profile Synchronization 
    /// </summary>
    public class RoleSyncronizer : PortalModuleBase
    {
        /// <summary>
        /// Synchronizes the user roles.
        /// </summary>
        /// <param name="boardId">The board id.</param>
        /// <param name="portalId">The portal id.</param>
        /// <param name="yafUserId">The YAF user id.</param>
        /// <param name="dnnUserInfo">The DNN user info.</param>
        /// <returns>Returns if the users was synced or not</returns>
        public static bool SynchronizeUserRoles(int boardId, int portalId, int yafUserId, UserInfo dnnUserInfo)
        {
            // Make sure are roles exist
            ImportDNNRoles(boardId, dnnUserInfo.Roles);

            var yafUserRoles = Data.GetYafUserRoles(boardId, yafUserId);

            var yafBoardRoles = LegacyDb.group_list(boardId, DBNull.Value);

            var rolesChanged = false;

            // add yaf only roles to yaf
            foreach (DataRow row in yafBoardRoles.Rows)
            {
                GroupFlags roleFlags = new GroupFlags(row["Flags"]);

                var role = new RoleInfo
                {
                    RoleName = row["Name"].ToString(),
                    RoleID = row["GroupID"].ToType<int>()
                };

                if (roleFlags.IsGuest)
                {
                    continue;
                }

                if (roleFlags.IsStart)
                {
                    if (!yafUserRoles.Any(existRole => existRole.RoleName.Equals(role.RoleName)))
                    {
                        UpdateUserRole(role, yafUserId, dnnUserInfo.Username, true);

                        rolesChanged = true;
                    }
                }
                else
                {
                    if (dnnUserInfo.Roles.Any(dnnRole => dnnRole.Equals(row["Name"].ToString())))
                    {
                        if (!yafUserRoles.Any(existRole => existRole.RoleName.Equals(role.RoleName)))
                        {
                            UpdateUserRole(role, yafUserId, dnnUserInfo.Username, true);

                            rolesChanged = true;
                        }
                    }
                }
            }

            var roleController = new RoleController();

            // Remove user from dnn role if no longer included
            foreach (
                RoleInfo role in
                    roleController.GetPortalRoles(portalId)
                                  .Cast<RoleInfo>()
                                  .Where(
                                      role =>
                                      !dnnUserInfo.Roles.Any(existRole => existRole.Equals(role.RoleName))
                                      && yafUserRoles.Any(existRole => existRole.RoleName.Equals(role.RoleName))))
            {
                UpdateUserRole(role, yafUserId, dnnUserInfo.Username, false);

                rolesChanged = true;
            }

            // empty out access table
            if (rolesChanged)
            {
                YafContext.Current.GetRepository<ActiveAccess>().Reset();
            }

            return rolesChanged;
        }

        /// <summary>
        /// Checks if the <paramref name="roles"/> exists, in YAF, the user is in 
        /// </summary>
        /// <param name="boardId">The board id.</param>
        /// <param name="roles">The <paramref name="roles"/>.</param>
        public static void ImportDNNRoles(int boardId, string[] roles)
        {
            var yafBoardRoles = Data.GetYafBoardRoles(boardId);
            var yafBoardAccessMasks = Data.GetYafBoardAccessMasks(boardId);

            // Check If Dnn Roles Exists in Yaf
            foreach (string role in from role in roles
                                    where !string.IsNullOrEmpty(role)
                                    let any = yafBoardRoles.Any(yafRole => yafRole.RoleName.Equals(role))
                                    where !any
                                    select role)
            {
                CreateYafRole(role, boardId, yafBoardAccessMasks);
            }
        }

        /// <summary>
        /// Creates the YAF role.
        /// </summary>
        /// <param name="roleName">Name of the role.</param>
        /// <param name="boardId">The board id.</param>
        /// <param name="yafBoardAccessMasks">The YAF board access masks.</param>
        /// <returns>
        /// Returns the Role id of the created Role
        /// </returns>
        public static long CreateYafRole(string roleName, int boardId, List<RoleInfo> yafBoardAccessMasks)
        {
            // If not Create Role in YAF
            if (!RoleMembershipHelper.RoleExists(roleName))
            {
                RoleMembershipHelper.CreateRole(roleName);
            }

            int accessMaskId;

            try
            {
                // Give the default DNN Roles Member Access, unknown roles get "Read Only Access"
                accessMaskId = roleName.Equals("Registered Users") || roleName.Equals("Subscribers")
                               ? yafBoardAccessMasks.Find(mask => mask.RoleName.Equals("Member Access")).RoleID
                               : yafBoardAccessMasks.Find(mask => mask.RoleName.Equals("Read Only Access")).RoleID;
            }
            catch (Exception)
            {
                accessMaskId = yafBoardAccessMasks.Find(mask => mask.RoleGroupID.Equals(1)).RoleID;
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
                accessMaskId,
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
        /// Updates the user <paramref name="role" />.
        /// </summary>
        /// <param name="role">The <paramref name="role" />.</param>
        /// <param name="yafUserId">The YAF user id.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="addRole">if set to true [add role].</param>
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