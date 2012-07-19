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
    using System.Web.Security;

    using YAF.Classes;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;

    using global::DotNetNuke.Entities.Portals;
    using global::DotNetNuke.Entities.Users;
    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Utils;

    /// <summary>
    /// Yaf User Importer
    /// </summary>
    public class UserImporter
    {
        /// <summary>
        /// Creates the yaf user.
        /// </summary>
        /// <param name="dnnUserInfo">The dnn user info.</param>
        /// <param name="dnnUser">The dnn user.</param>
        /// <param name="boardID">The board ID.</param>
        /// <param name="portalSettings">The portal settings.</param>
        /// <param name="boardSettings">The board settings.</param>
        /// <returns>
        /// Returns the User ID of the new User
        /// </returns>
        public static int CreateYafUser(UserInfo dnnUserInfo, MembershipUser dnnUser, int boardID, PortalSettings portalSettings, YafBoardSettings boardSettings)
        {
            // setup roles
            RoleMembershipHelper.SetupUserRoles(boardID, dnnUser.UserName);

            // create the user in the YAF DB so profile can ge created...
            var yafUserId = RoleMembershipHelper.CreateForumUser(dnnUser, boardID);

            if (yafUserId == null)
            {
                return 0;
            }

            // create profile
            var userProfile = YafUserProfile.GetProfile(dnnUser.UserName);

            // setup their inital profile information
            userProfile.Initialize(dnnUser.UserName, true);

            userProfile.RealName = dnnUserInfo.Profile.FullName;
            userProfile.Country = dnnUserInfo.Profile.Country;
            userProfile.Region = dnnUserInfo.Profile.Region;
            userProfile.City = dnnUserInfo.Profile.City;
            userProfile.Homepage = dnnUserInfo.Profile.Website;
            
            userProfile.Save();

            // Save User
            LegacyDb.user_save(
                yafUserId,
                boardID,
                dnnUserInfo.DisplayName,
                dnnUserInfo.DisplayName,
                null,
                ProfileSyncronizer.GetUserTimeZoneOffset(dnnUserInfo, portalSettings),
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null);

            bool autoWatchTopicsEnabled =
                boardSettings.DefaultNotificationSetting.Equals(UserNotificationSetting.TopicsIPostToOrSubscribeTo);

            // save notification Settings
            LegacyDb.user_savenotification(
                yafUserId,
                true,
                autoWatchTopicsEnabled,
                boardSettings.DefaultNotificationSetting,
                boardSettings.DefaultSendDigestEmail);

            return yafUserId.ToType<int>();
        }
    }
}