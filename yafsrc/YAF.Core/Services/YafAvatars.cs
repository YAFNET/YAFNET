/* Yet Another Forum.net
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

namespace YAF.Core.Services
{
    #region Using

    using System;
    using System.Web;

    using YAF.Classes;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The YAF avatars.
    /// </summary>
    public class YafAvatars : IAvatars
    {
        #region Constants and Fields

        /// <summary>
        /// The YAF board settings.
        /// </summary>
        private readonly YafBoardSettings _yafBoardSettings;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="YafAvatars"/> class.
        /// </summary>
        /// <param name="boardSettings">
        /// The board settings.
        /// </param>
        public YafAvatars(YafBoardSettings boardSettings)
        {
            this._yafBoardSettings = boardSettings;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The get avatar url for current user.
        /// </summary>
        /// <returns>
        /// Returns the Avatar Url 
        /// </returns>
        public string GetAvatarUrlForCurrentUser()
        {
            return this.GetAvatarUrlForUser(YafContext.Current.CurrentUserData);
        }

        /// <summary>
        /// The get avatar url for user.
        /// </summary>
        /// <param name="userId">
        /// The user id. 
        /// </param>
        /// <returns>
        /// Returns the Avatar Url 
        /// </returns>
        public string GetAvatarUrlForUser(int userId)
        {
            var userData = new CombinedUserDataHelper(userId);

            return this.GetAvatarUrlForUser(userData);
        }

        /// <summary>
        /// The get avatar url for user.
        /// </summary>
        /// <param name="userData">
        /// The user data. 
        /// </param>
        /// <returns>
        /// Returns the Avatar Url 
        /// </returns>
        public string GetAvatarUrlForUser([NotNull] IUserData userData)
        {
            CodeContracts.VerifyNotNull(userData, "userData");

            var getUserEmail = new Func<string>(
                () =>
                    {
                        string userEmail;
                        try
                        {
                            userEmail = userData.Email;
                        }
                        catch (Exception)
                        {
                            userEmail = string.Empty;
                        }
                        return userEmail;
                    });

            return this.GetAvatarUrlForUser(
                userData.UserID,
                userData.Avatar,
                userData.HasAvatarImage,
                this._yafBoardSettings.AvatarGravatar ? getUserEmail() : string.Empty);
        }

        /// <summary>
        /// The get avatar url for user.
        /// </summary>
        /// <param name="userId">
        /// The user Id. 
        /// </param>
        /// <param name="avatarString">
        /// The avatarString. 
        /// </param>
        /// <param name="hasAvatarImage">
        /// The hasAvatarImage. 
        /// </param>
        /// <param name="email">
        /// The email. 
        /// </param>
        /// <returns>
        /// Returns the Avatar Url 
        /// </returns>
        public string GetAvatarUrlForUser(int userId, string avatarString, bool hasAvatarImage, string email)
        {
            string avatarUrl = string.Empty;

            if (this._yafBoardSettings.AvatarUpload && hasAvatarImage)
            {
                avatarUrl = "{0}resource.ashx?u={1}".FormatWith(YafForumInfo.ForumClientFileRoot, userId);
            }
            else if (avatarString.IsSet())
            {
                // Took out PageContext.BoardSettings.AvatarRemote
                avatarUrl =
                    "{3}resource.ashx?url={0}&width={1}&height={2}".FormatWith(
                        HttpUtility.UrlEncode(avatarString),
                        this._yafBoardSettings.AvatarWidth,
                        this._yafBoardSettings.AvatarHeight,
                        YafForumInfo.ForumClientFileRoot);
            }
            else if (this._yafBoardSettings.AvatarGravatar && email.IsSet())
            {
                // JoeOuts added 8/17/09 for Gravatar use

                // string noAvatarGraphicUrl = HttpContext.Current.Server.UrlEncode( string.Format( "{0}/images/avatars/{1}", YafForumInfo.ForumBaseUrl, "NoAvatar.gif" ) );
                string gravatarUrl =
                    @"http://www.gravatar.com/avatar/{0}.jpg?r={1}".FormatWith(
                        StringExtensions.StringToHexBytes(email), this._yafBoardSettings.GravatarRating);

                avatarUrl =
                    @"{3}resource.ashx?url={0}&width={1}&height={2}".FormatWith(
                        HttpUtility.UrlEncode(gravatarUrl),
                        this._yafBoardSettings.AvatarWidth,
                        this._yafBoardSettings.AvatarHeight,
                        YafForumInfo.ForumClientFileRoot);
            }

            // Return NoAvatar Image is no Avatar available for that user.
            if (avatarUrl.IsNotSet())
            {
                avatarUrl = "{0}images/noavatar.gif".FormatWith(YafForumInfo.ForumClientFileRoot);
            }

            return avatarUrl;
        }

        #endregion
    }
}