/* YetAnotherForum.NET
 * Copyright (C) 2006-2010 Jaben Cargman
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
    #region

    using System;
    using System.Data;
    using System.Text;
    using System.Web;
    using System.Web.Script.Services;
    using System.Web.Security;
    using System.Web.Services;

    using YAF.Classes.Core;
    using YAF.Classes.Data;
    using YAF.Classes.Utils;
    using YAF.Controls;

    #endregion

    /// <summary>
    /// Class for JS jQuery  Ajax Methods
    /// </summary>
    [WebService(Namespace = "http://yetanotherforum.net/yafajax")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ScriptService]
    public class YafAjax : WebService
    {
        #region Public Methods

        /// <summary>
        /// This method returns a string containing the HTML code for
        ///   showing the the post footer. the HTML content is the name of users
        ///   who thanked the post and the date they thanked.
        /// </summary>
        /// <param name="msgID">
        /// The msg ID.
        /// </param>
        /// <returns>
        /// The get thanks.
        /// </returns>
        public static string GetThanks(object msgID)
        {
            var filler = new StringBuilder();

            using (DataTable dt = DB.message_GetThanks(msgID.ToType<int>()))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (filler.Length > 0)
                    {
                        filler.Append(",&nbsp;");
                    }

                    // vzrus: quick fix for the incorrect link. URL rewriting don't work :(
                    filler.AppendFormat(
                        @"<a id=""{0}"" href=""{1}""><u>{2}</u></a>", 
                        dr["UserID"], 
                        YafBuildLink.GetLink(ForumPages.profile, "u={0}", dr["UserID"]), 
                        dr["DisplayName"] != DBNull.Value
                            ? YafContext.Current.Get<HttpServerUtilityBase>().HtmlEncode(dr["DisplayName"].ToString())
                            : YafContext.Current.Get<HttpServerUtilityBase>().HtmlEncode(dr["Name"].ToString()));

                    if (YafContext.Current.BoardSettings.ShowThanksDate)
                    {
                        filler.AppendFormat(
                            @" {0}",
                            YafContext.Current.Get<ILocalizationHandler>().Localization.GetText("DEFAULT", "ONDATE").FormatWith(
                                YafContext.Current.Get<IDateTime>().FormatDateShort(dr["ThanksDate"])));
                    }
                }
            }

            return filler.ToString();
        }

        /// <summary>
        /// Add Thanks to post
        /// </summary>
        /// <param name="msgID">
        /// The msg id.
        /// </param>
        /// <returns>
        /// Returns ThankYou Info
        /// </returns>
        [WebMethod(EnableSession = true)]
        public ThankYouInfo AddThanks(object msgID)
        {
            var messageID = msgID.ToType<int>();

            string username =
                DB.message_AddThanks(
                    UserMembershipHelper.GetUserIDFromProviderUserKey(Membership.GetUser().ProviderUserKey), messageID);

            // if the user is empty, return a null object...
            return username.IsNotSet()
                       ? null
                       : this.CreateThankYou(username, "BUTTON_THANKSDELETE", "BUTTON_THANKSDELETE_TT", messageID);
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
        [WebMethod(EnableSession = true)]
        public ThankYouInfo RemoveThanks(object msgID)
        {
            var messageID = msgID.ToType<int>();

            string username =
                DB.message_RemoveThanks(
                    UserMembershipHelper.GetUserIDFromProviderUserKey(Membership.GetUser().ProviderUserKey), messageID);

            return this.CreateThankYou(username, "BUTTON_THANKS", "BUTTON_THANKS_TT", messageID);
        }

        #endregion

        #region Methods

        /// <summary>
        /// This method returns a string which shows how many times users have
        ///   thanked the message with the provided messageID. Returns an empty string.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="messageID">
        /// The Message ID.
        /// </param>
        /// <returns>
        /// The thanks number.
        /// </returns>
        protected string ThanksNumber(string username, int messageID)
        {
            int thanksNumber = DB.message_ThanksNumber(messageID);

            // get the user's display name.
            string displayName =
                YafContext.Current.UserDisplayName.GetName(
                    UserMembershipHelper.GetUserIDFromProviderUserKey(
                        UserMembershipHelper.GetMembershipUserByName(username).ProviderUserKey));

            // if displayname is enabled in admin section, and the user has a display name, use it instead of username.
            displayName = (displayName != string.Empty && YafContext.Current.BoardSettings.EnableDisplayName)
                              ? displayName
                              : username;

            switch (thanksNumber)
            {
                case 0:
                    return String.Empty;
                case 1:
                    return YafContext.Current.Get<ILocalizationHandler>().Localization.GetText("POSTS", "THANKSINFOSINGLE").FormatWith(displayName);
            }

            return YafContext.Current.Get<ILocalizationHandler>().Localization.GetText("POSTS", "THANKSINFO").FormatWith(thanksNumber, displayName);
        }

        /// <summary>
        /// Creates an instance of the thank you object from the current information.
        /// </summary>
        /// <param name="username">
        /// The Current Username
        /// </param>
        /// <param name="textTag">
        /// Button Text
        /// </param>
        /// <param name="titleTag">
        /// Button  Title
        /// </param>
        /// <param name="messageId">
        /// The message Id.
        /// </param>
        /// <returns>
        /// Returns ThankYou Info
        /// </returns>
        private ThankYouInfo CreateThankYou(string username, string textTag, string titleTag, int messageId)
        {
            // load the DB so YafContext can work...
            // verify DB is initialized...
            if (!YafContext.Current.Get<StartupInitializeDb>().Initialized)
            {
                YafContext.Current.Get<StartupInitializeDb>().Run();
            }

            return new ThankYouInfo
            {
                MessageID = messageId,
                ThanksInfo = this.ThanksNumber(username, messageId),
                Thanks = GetThanks(messageId),
                Text = YafContext.Current.Get<ILocalizationHandler>().Localization.GetText("BUTTON", textTag),
                Title = YafContext.Current.Get<ILocalizationHandler>().Localization.GetText("BUTTON", titleTag)
            };
        }

        #endregion
    }
}