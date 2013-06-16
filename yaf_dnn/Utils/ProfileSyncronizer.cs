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
    using System.Data;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Web.Security;

    using global::DotNetNuke.Common.Utilities;

    using global::DotNetNuke.Entities.Modules;

    using global::DotNetNuke.Entities.Portals;

    using global::DotNetNuke.Entities.Profile;

    using global::DotNetNuke.Entities.Users;

    using global::DotNetNuke.Services.Exceptions;

    using global::DotNetNuke.Services.FileSystem;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    /// <summary>
    /// YAF DNN Profile Synchronization 
    /// </summary>
    public class ProfileSyncronizer : PortalModuleBase
    {
        /// <summary>
        /// Synchronize The YAF Profile with DNN Profile or reverse if
        /// one profile is newer then the other
        /// </summary>
        /// <param name="yafUserId">The YAF UserId</param>
        /// <param name="yafUserProfile">The YAF user profile.</param>
        /// <param name="dnnUserInfo">DNN UserInfo of current User</param>
        /// <param name="membershipUser">MemberShip of current User</param>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="boardId">The board Id.</param>
        public static void UpdateUserProfile([NotNull]int yafUserId, [NotNull]YafUserProfile yafUserProfile, [NotNull]UserInfo dnnUserInfo, [NotNull]MembershipUser membershipUser, [NotNull]int portalID, [NotNull]int boardId)
        {
            try
            {
                if (yafUserProfile == null)
                {
                    yafUserProfile = YafUserProfile.GetProfile(membershipUser.UserName);
                }

                var yafTime = yafUserProfile.LastUpdatedDate;
                var dnnTime = dnnUserInfo.LastModifiedOnDate;

                 TimeSpan timeCompare = dnnTime - yafTime;

                 if (timeCompare.TotalSeconds > 0)
                 {
                     SyncDnnProfileToYaf(dnnTime, yafUserId, yafUserProfile, dnnUserInfo, portalID, boardId);
                 }
                 else
                 {
                     SyncYafProfileToDnn(
                         yafTime, yafUserId, yafUserProfile, dnnUserInfo, membershipUser, portalID, boardId);
                 }
            }
            catch (Exception ex)
            {
               Exceptions.LogException(ex);
            }
        }

        /// <summary>
        /// Gets the user time zone offset.
        /// </summary>
        /// <param name="userInfo">The user info.</param>
        /// <param name="portalSettings">Current Portal Settings</param>
        /// <returns>
        /// Returns the User Time Zone Offset Value
        /// </returns>
        public static int GetUserTimeZoneOffset([NotNull]UserInfo userInfo, [NotNull]PortalSettings portalSettings)
        {
            int timeZone;

            if ((userInfo != null) && (userInfo.UserID != Null.NullInteger))
            {
                timeZone = userInfo.Profile.TimeZone;
            }
            else
            {
                if (portalSettings != null)
                {
                    timeZone = portalSettings.TimeZoneOffset;
                }
                else
                {
                    timeZone = -480;
                }
            }

            return timeZone;
        }

        /// <summary>
        /// YAF profile is newer sync DNN now
        /// </summary>
        /// <param name="dnnTime">The DNN Time.</param>
        /// <param name="yafUserId">The YAF user id.</param>
        /// <param name="yafUserProfile">The YAF user profile.</param>
        /// <param name="dnnUserInfo">The DNN user info.</param>
        /// <param name="membershipUser">The membership user.</param>
        /// <param name="portalId">The portal id.</param>
        /// <param name="boardId">The board id.</param>
        private static void SyncYafProfileToDnn([NotNull]DateTime dnnTime, [NotNull]int yafUserId, [NotNull]IYafUserProfile yafUserProfile, [NotNull]UserInfo dnnUserInfo, [NotNull]MembershipUser membershipUser, [NotNull]int portalId, [NotNull]int boardId)
        {
            var cacheKeyDnnName = "dnnsync_userid{0}_portalid{1}".FormatWith(dnnUserInfo.UserID, portalId);

            var cacheTime = dnnTime;

            // Make sure its syncs only when needed
            try
            {
                if (DataCache.GetCache(cacheKeyDnnName) != null)
                {
                    cacheTime = (DateTime)DataCache.GetCache(cacheKeyDnnName);
                }
            }
            catch (Exception)
            {
                cacheTime = dnnTime;
            }
            
            if (dnnTime < cacheTime)
            {
                return;
            }

            var yafUserData = new CombinedUserDataHelper(yafUserId);

            if (!string.IsNullOrEmpty(yafUserProfile.RealName.Trim()))
            {
                if (yafUserProfile.RealName.Contains(" "))
                {
                    // Split Fullname into First and Lastname
                    var firstName = yafUserProfile.RealName.Remove(yafUserProfile.RealName.IndexOf(" "));
                    var lastName = yafUserProfile.RealName.Substring(yafUserProfile.RealName.IndexOf(" ") + 1);

                    dnnUserInfo.Profile.FirstName = firstName;
                    dnnUserInfo.Profile.LastName = lastName;
                }
                else
                {
                    dnnUserInfo.Profile.FirstName = yafUserProfile.RealName;
                }
            }

            dnnUserInfo.DisplayName = yafUserData.DisplayName;

            if (yafUserProfile.Country.IsSet())
            {
                dnnUserInfo.Profile.Country = new RegionInfo(yafUserProfile.Country).EnglishName;
            }

            dnnUserInfo.Profile.City = yafUserProfile.City;

            dnnUserInfo.Profile.Website = yafUserProfile.Homepage;
            dnnUserInfo.Email = membershipUser.Email;

            if (yafUserData.CultureUser.IsSet())
            {
                dnnUserInfo.Profile.PreferredLocale = yafUserData.CultureUser;
            }

            // Save other Yaf Profile Properties as Custom DNN Profile Properties
            dnnUserInfo.Profile.SetProfileProperty("Birthday", yafUserProfile.Birthday.ToString());
            dnnUserInfo.Profile.SetProfileProperty("Occupation", yafUserProfile.Occupation);
            dnnUserInfo.Profile.SetProfileProperty("Gender", yafUserProfile.Gender.ToString());
            dnnUserInfo.Profile.SetProfileProperty("Blog", yafUserProfile.Blog);
            dnnUserInfo.Profile.SetProfileProperty("MSN", yafUserProfile.MSN);
            dnnUserInfo.Profile.SetProfileProperty("YIM", yafUserProfile.YIM);
            dnnUserInfo.Profile.SetProfileProperty("AIM", yafUserProfile.AIM);
            dnnUserInfo.Profile.SetProfileProperty("ICQ", yafUserProfile.ICQ);
            dnnUserInfo.Profile.SetProfileProperty("Facebook", yafUserProfile.Facebook);
            dnnUserInfo.Profile.SetProfileProperty("Twitter", yafUserProfile.Twitter);
            dnnUserInfo.Profile.SetProfileProperty("Twitter1", yafUserProfile.TwitterId);
            dnnUserInfo.Profile.SetProfileProperty("XMPP", yafUserProfile.XMPP);
            dnnUserInfo.Profile.SetProfileProperty("Skype", yafUserProfile.Skype);

            ProfileController.UpdateUserProfile(dnnUserInfo, dnnUserInfo.Profile.ProfileProperties);

            UserController.UpdateUser(portalId, dnnUserInfo);

            LegacyDb.user_setnotdirty(boardId, yafUserId);

            DataCache.SetCache(cacheKeyDnnName, DateTime.Now);
        }

        /// <summary>
        /// DNN Profile is newer, sync YAF now
        /// NOTE : no need to manually sync Email Address
        /// </summary>
        /// <param name="yafTime">The YAF Time.</param>
        /// <param name="yafUserId">The YAF user id.</param>
        /// <param name="yafUserProfile">The YAF user profile.</param>
        /// <param name="dnnUserInfo">The DNN user info.</param>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="boardId">The board Id.</param>
        private static void SyncDnnProfileToYaf([NotNull]DateTime yafTime, [NotNull]int yafUserId, [NotNull]YafUserProfile yafUserProfile, [NotNull]UserInfo dnnUserInfo, [NotNull]int portalID, [NotNull]int boardId)
        {
            var cacheKeyYafName = "yafsync_userid{0}_portalid{1}".FormatWith(dnnUserInfo.UserID, portalID);

            var cacheTime = yafTime;

            // Make sure its syncs only when needed
            try
            {
                if (DataCache.GetCache(cacheKeyYafName) != null)
                {
                    cacheTime = (DateTime)DataCache.GetCache(cacheKeyYafName);
                }
            }
            catch (Exception)
            {
                cacheTime = yafTime;
            }

            if (yafTime < cacheTime)
            {
                return;
            }

            var yafUserData = new CombinedUserDataHelper(yafUserId);

            YafCultureInfo userCuluture = new YafCultureInfo
            {
                LanguageFile = yafUserData.LanguageFile,
                Culture = yafUserData.CultureUser
            };

            if (dnnUserInfo.Profile.PreferredLocale.IsSet())
            {
                CultureInfo newCulture = new CultureInfo(dnnUserInfo.Profile.PreferredLocale);

                foreach (DataRow row in
                    StaticDataHelper.Cultures().Rows.Cast<DataRow>().Where(
                        row => dnnUserInfo.Profile.PreferredLocale == row["CultureTag"].ToString() || newCulture.TwoLetterISOLanguageName == row["CultureTag"].ToString()))
                {
                    userCuluture.LanguageFile = row["CultureFile"].ToString();
                    userCuluture.Culture = row["CultureTag"].ToString();
                }
            }

            LegacyDb.user_save(
                yafUserId,
                boardId,
                null,
                dnnUserInfo.DisplayName,
                null,
                yafUserData.TimeZone,
                userCuluture.LanguageFile,
                userCuluture.Culture,
                yafUserData.ThemeFile,
                yafUserData.UseSingleSignOn,
                yafUserData.TextEditor,
                yafUserData.UseMobileTheme,
                null,
                null,
                null,
                yafUserData.DSTUser,
                yafUserData.IsActiveExcluded,
                null);

            yafUserProfile.RealName = dnnUserInfo.Profile.FullName;

            if (dnnUserInfo.Profile.Country.IsSet())
            {
                yafUserProfile.Country =
                    GetRegionInfoFromCountryName(dnnUserInfo.Profile.Country).TwoLetterISORegionName;
            }

            yafUserProfile.City = dnnUserInfo.Profile.City;
            yafUserProfile.Homepage = dnnUserInfo.Profile.Website;

            if (!string.IsNullOrEmpty(dnnUserInfo.Profile.Photo))
            {
                SaveDnnAvatarToYaf(dnnUserInfo.Profile.PhotoURL, yafUserId);
            }

            // Save other Yaf Profile Properties as Custom DNN Profile Properties
            try
            {
                if (dnnUserInfo.Profile.GetPropertyValue("Birthday").IsSet())
                {
                    yafUserProfile.Birthday = DateTime.Parse(dnnUserInfo.Profile.GetPropertyValue("Birthday"));
                }
                
                yafUserProfile.Occupation = dnnUserInfo.Profile.GetPropertyValue("Occupation");

                if (dnnUserInfo.Profile.GetPropertyValue("Gender").IsSet()
                    && ValidationHelper.IsValidInt(dnnUserInfo.Profile.GetPropertyValue("Gender")))
                {
                    yafUserProfile.Gender = dnnUserInfo.Profile.GetPropertyValue("Gender").ToType<int>();
                }

                yafUserProfile.Blog = dnnUserInfo.Profile.GetPropertyValue("Blog");
                yafUserProfile.MSN = dnnUserInfo.Profile.GetPropertyValue("MSN");
                yafUserProfile.YIM = dnnUserInfo.Profile.GetPropertyValue("YIM");
                yafUserProfile.AIM = dnnUserInfo.Profile.GetPropertyValue("AIM");
                yafUserProfile.ICQ = dnnUserInfo.Profile.GetPropertyValue("ICQ");
                yafUserProfile.Facebook = dnnUserInfo.Profile.GetPropertyValue("Facebook");
                yafUserProfile.Twitter = dnnUserInfo.Profile.GetPropertyValue("Twitter");
                yafUserProfile.TwitterId = dnnUserInfo.Profile.GetPropertyValue("TwitterId");
                yafUserProfile.XMPP = dnnUserInfo.Profile.GetPropertyValue("XMPP");
                yafUserProfile.Skype = dnnUserInfo.Profile.GetPropertyValue("Skype");
            }
            finally 
            {
                yafUserProfile.Save();
            }

            YafContext.Current.Get<IRaiseEvent>().Raise(new UpdateUserEvent(yafUserId));

            DataCache.SetCache(cacheKeyYafName, DateTime.Now);
        }
        /*
        /// <summary>
        /// Extract YAF Avatar to DNN Photo
        /// </summary>
        /// <param name="avatarUrl">
        /// The avatar url.
        /// </param>
        /// <param name="dnnUserInfo">
        /// The DNN user info.
        /// </param>
        /// <param name="portalId">
        /// The portal id.
        /// </param>
        /// <returns>
        /// File ID of the YAF Avatar inside DNN
        /// </returns>
        private static string SaveYafAvatarToDnn([NotNull]string avatarUrl, [NotNull]UserInfo dnnUserInfo, [NotNull]int portalId)
        {
            try
            {
                var folderInfo = FolderManager.Instance.GetUserFolder(dnnUserInfo);

                const string YafAvatarName = "YafAvatar.jpg";

                if (!avatarUrl.StartsWith("http"))
                {
                    avatarUrl = "{0}{1}".FormatWith(BaseUrlBuilder.BaseUrl, avatarUrl);
                }

                // Check if the file exists
                if (File.Exists(Path.Combine(folderInfo.PhysicalPath, YafAvatarName)))
                {
                    File.Delete(Path.Combine(folderInfo.PhysicalPath, YafAvatarName));
                }

                // Download Yaf Avatar
                WebClient wc = new WebClient();

                wc.Headers.Add("Referer", YafBuildLink.GetLink(ForumPages.forum));

                wc.DownloadFile(avatarUrl, Path.Combine(folderInfo.PhysicalPath, YafAvatarName));

                var fileInfo = FileManager.Instance.GetFile(folderInfo, YafAvatarName);

                if (fileInfo == null)
                {
                    FileSystemUtils.AddFile(YafAvatarName, portalId, true, (FolderInfo)folderInfo);

                    fileInfo = FileManager.Instance.GetFile(folderInfo, YafAvatarName);
                }

                return fileInfo.FileId.ToString();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }*/

        /// <summary>
        /// Gets the name of the region info from country (English Name).
        /// </summary>
        /// <param name="countryEnglishName">Name of the country english.</param>
        /// <returns>The RegionInfo for the Country</returns>
        private static RegionInfo GetRegionInfoFromCountryName([NotNull]string countryEnglishName)
        {
            return
                CultureInfo.GetCultures(CultureTypes.SpecificCultures).Select(ci => new RegionInfo(ci.LCID)).FirstOrDefault(region => region.EnglishName.Equals(countryEnglishName));
        }

        /// <summary>
        /// Save DNN Avatar as YAF Remote Avatar with relative Path.
        /// </summary>
        /// <param name="avatarUrl">The avatar URL.</param>
        /// <param name="yafUserId">The YAF user id.</param>
        private static void SaveDnnAvatarToYaf([NotNull]string avatarUrl, [NotNull]int yafUserId)
        {
            // Delete old first
            LegacyDb.user_deleteavatar(yafUserId);

            // update
            LegacyDb.user_saveavatar(
                yafUserId,
                "{0}{1}".FormatWith(BaseUrlBuilder.BaseUrl, avatarUrl),
                null,
                null);

            // clear the cache for this user...
            YafContext.Current.Get<IRaiseEvent>().Raise(new UpdateUserEvent(yafUserId));
        }
    }
}