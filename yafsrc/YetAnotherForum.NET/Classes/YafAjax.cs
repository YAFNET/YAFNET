 /* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Classes
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web.Script.Services;
    using System.Web.Security;
    using System.Web.Services;

    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils.Helpers.StringUtils;

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

        /// <summary>
        /// Handles the multi quote Button.
        /// </summary>
        /// <param name="buttonId">The button id.</param>
        /// <param name="multiquoteButton">The Multi quote Button Checkbox checked</param>
        /// <param name="messageId">The message id.</param>
        /// <param name="buttonCssClass">The button CSS class.</param>
        /// <returns>Returns the Message Id and the Updated CSS Class for the Button</returns>
        [WebMethod(EnableSession = true)]
        public YafAlbum.ReturnClass HandleMultiQuote([NotNull]string buttonId, [NotNull]bool multiquoteButton, [NotNull]int messageId, [NotNull]string buttonCssClass)
        {
            var yafSession = this.Get<IYafSession>();

            if (multiquoteButton)
            {
                if (yafSession.MultiQuoteIds != null)
                {
                    if (!yafSession.MultiQuoteIds.Contains(messageId))
                    {
                        yafSession.MultiQuoteIds.Add(messageId);
                    }
                }
                else
                {
                    yafSession.MultiQuoteIds = new List<int> { messageId };
                }

                buttonCssClass += " Checked";
            }
            else
            {
                if (yafSession.MultiQuoteIds != null && yafSession.MultiQuoteIds.Contains(messageId))
                {
                    yafSession.MultiQuoteIds.Remove(messageId);
                }

                buttonCssClass = "MultiQuoteButton";
            }

            return new YafAlbum.ReturnClass { Id = buttonId, NewTitle = buttonCssClass };
        }

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
        /// The refresh shout box JS.
        /// </returns>
        [WebMethod]
        public int RefreshShoutBox(int boardId)
        {
            var messages = this.Get<IDataCache>().GetOrSet(
                "{0}_basic".FormatWith(Constants.Cache.Shoutbox),
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
        /// The add favorite topic JS.
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
        /// The remove favorite topic JS.
        /// </returns>
        [WebMethod(EnableSession = true)]
        public int RemoveFavoriteTopic(int topicId)
        {
            return this.Get<IFavoriteTopic>().RemoveFavoriteTopic(topicId);
        }

        #region Thanks Functions

        /// <summary>
        /// Add Thanks to post
        /// </summary>
        /// <param name="msgID">
        /// The message id.
        /// </param>
        /// <returns>
        /// Returns ThankYou Info
        /// </returns>
        [CanBeNull]
        [WebMethod(EnableSession = true)]
        public ThankYouInfo AddThanks([NotNull] object msgID)
        {
            var messageId = msgID.ToType<int>();

            var membershipUser = Membership.GetUser();

            if (membershipUser == null)
            {
                return null;
            }

            var username =
                LegacyDb.message_AddThanks(
                    UserMembershipHelper.GetUserIDFromProviderUserKey(membershipUser.ProviderUserKey),
                    messageId,
                    this.Get<YafBoardSettings>().EnableDisplayName);

            // if the user is empty, return a null object...
            return username.IsNotSet()
                       ? null
                       : YafThankYou.CreateThankYou(
                           new UnicodeEncoder().XSSEncode(username),
                           "BUTTON_THANKSDELETE",
                           "BUTTON_THANKSDELETE_TT",
                           messageId);
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
        [NotNull]
        [WebMethod(EnableSession = true)]
        public ThankYouInfo RemoveThanks([NotNull] object msgID)
        {
            var messageID = msgID.ToType<int>();

            var username =
                LegacyDb.message_RemoveThanks(
                    UserMembershipHelper.GetUserIDFromProviderUserKey(Membership.GetUser().ProviderUserKey), messageID, this.Get<YafBoardSettings>().EnableDisplayName);

            return YafThankYou.CreateThankYou(username, "BUTTON_THANKS", "BUTTON_THANKS_TT", messageID);
        }

        #endregion

        #endregion
    }
}