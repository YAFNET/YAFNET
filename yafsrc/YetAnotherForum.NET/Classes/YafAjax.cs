/* YetAnotherForum.NET
 * Copyright (C) 2006-2011 Jaben Cargman
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

namespace YAF.Classes
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.Script.Services;
    using System.Web.Security;
    using System.Web.Services;

    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// Class for JS jQuery  Ajax Methods
    /// </summary>
    [WebService(Namespace = "http://yetanotherforum.net/yafajax")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ScriptService]
    public class YafAjax : WebService, IHaveServiceLocator
    {
        #region Properties

        /// <summary>
        ///   Gets ServiceLocator.
        /// </summary>
        public IServiceLocator ServiceLocator
        {
            get
            {
                return YafContext.Current.ServiceLocator;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The change album title.
        /// </summary>
        /// <param name="albumID">
        /// The album id.
        /// </param>
        /// <param name="newTitle">
        /// The New title.
        /// </param>
        /// <returns>
        /// the return object.
        /// </returns>
        [WebMethod(EnableSession = true)]
        public YafAlbum.ReturnClass ChangeAlbumTitle(int albumID, [NotNull] string newTitle)
        {
            return YafAlbum.ChangeAlbumTitle(albumID, newTitle);
        }

        /// <summary>
        /// The change image caption.
        /// </summary>
        /// <param name="imageID">
        /// The Image id.
        /// </param>
        /// <param name="newCaption">
        /// The New caption.
        /// </param>
        /// <returns>
        /// the return object.
        /// </returns>
        [WebMethod(EnableSession = true)]
        public YafAlbum.ReturnClass ChangeImageCaption(int imageID, [NotNull] string newCaption)
        {
            return YafAlbum.ChangeImageCaption(imageID, newCaption);
        }

        /// <summary>
        /// The refresh shout box.
        /// </summary>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <returns>
        /// The refresh shout box.
        /// </returns>
        [WebMethod]
        public int RefreshShoutBox(int boardId)
        {
            var messages = this.Get<IDataCache>().GetOrSet(
                Constants.Cache.Shoutbox + "_basic",
                () => LegacyDb.shoutbox_getmessages(boardId, 1, false).AsEnumerable(),
                TimeSpan.FromMilliseconds(1000));

            var message = messages.FirstOrDefault();

            return message != null ? message.Field<int>("ShoutBoxMessageID") : 0;
        }

       #region Favorite Topic Function

        /// <summary>
        /// The add favorite topic.
        /// </summary>
        /// <param name="topicId">
        /// The topic ID.
        /// </param>
        /// <returns>
        /// The add favorite topic.
        /// </returns>
        [WebMethod(EnableSession = true)]
        public int AddFavoriteTopic(int topicId)
        {
            return this.Get<IFavoriteTopic>().AddFavoriteTopic(topicId);
        }

        /// <summary>
        /// The remove favorite topic.
        /// </summary>
        /// <param name="topicId">
        /// The favorite topic id.
        /// </param>
        /// <returns>
        /// The remove favorite topic.
        /// </returns>
        [WebMethod(EnableSession = true)]
        public int RemoveFavoriteTopic(int topicId)
        {
            return this.Get<IFavoriteTopic>().RemoveFavoriteTopic(topicId);
        }
       #endregion

       #region Thanks Functions

        /// <summary>
        /// Add Thanks to post
        /// </summary>
        /// <param name="msgID">
        /// The msg id.
        /// </param>
        /// <returns>
        /// Returns ThankYou Info
        /// </returns>
        [CanBeNull, WebMethod(EnableSession = true)]
        public ThankYouInfo AddThanks([NotNull] object msgID)
        {
            var messageID = msgID.ToType<int>();

            string username =
              LegacyDb.message_AddThanks(
                UserMembershipHelper.GetUserIDFromProviderUserKey(Membership.GetUser().ProviderUserKey), messageID);

            // if the user is empty, return a null object...
            return username.IsNotSet()
                     ? null
                     : YafThankYou.CreateThankYou(username, "BUTTON_THANKSDELETE", "BUTTON_THANKSDELETE_TT", messageID);
        }

        /// <summary>
        /// This method is called asynchronously when the user clicks on "Remove Thank" button.
        /// </summary>
        /// <param name="msgID">
        /// Message Id
        /// </param>
        /// <returns>
        /// Returns ThankYou Info
        /// </returns>
        [NotNull, WebMethod(EnableSession = true)]
        public ThankYouInfo RemoveThanks([NotNull] object msgID)
        {
            var messageID = msgID.ToType<int>();

            string username =
              LegacyDb.message_RemoveThanks(
                UserMembershipHelper.GetUserIDFromProviderUserKey(Membership.GetUser().ProviderUserKey), messageID);

            return YafThankYou.CreateThankYou(username, "BUTTON_THANKS", "BUTTON_THANKS_TT", messageID);
        }

#endregion

        #endregion
    }
}