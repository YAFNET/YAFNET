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

namespace YAF.Core.Services
{
    using System.Data;
    using System.Text;
    using System.Web;
    using System.Web.Security;
    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    /// <summary>
    /// Yaf ThankYou Class to handle Thanks
    /// </summary>
    public class YafThankYou
    {
        #region Public Methods

        /// <summary>
        /// This method returns a string containing the HTML code for
        ///   showing the the post footer. the HTML content is the name of users
        ///   who thanked the post and the date they thanked.
        /// </summary>
        /// <param name="messageId">
        /// The msg ID.
        /// </param>
        /// <returns>
        /// The get thanks.
        /// </returns>
        [NotNull]
        public static string GetThanks([NotNull] int messageId)
        {
            var filler = new StringBuilder();

            using (DataTable dt = LegacyDb.message_GetThanks(messageId))
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
                        YafContext.Current.Get<YafBoardSettings>().EnableDisplayName  
                            ? YafContext.Current.Get<HttpServerUtilityBase>().HtmlEncode(dr["DisplayName"].ToString())
                            : YafContext.Current.Get<HttpServerUtilityBase>().HtmlEncode(dr["Name"].ToString()));

                    if (YafContext.Current.Get<YafBoardSettings>().ShowThanksDate)
                    {
                        filler.AppendFormat(
                            @" {0}",
                            YafContext.Current.Get<ILocalization>().GetText("DEFAULT", "ONDATE").FormatWith(YafContext.Current.Get<IDateTime>().FormatDateShort(dr["ThanksDate"])));
                    }
                }
            }

            return filler.ToString();
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
        [NotNull]
        public static ThankYouInfo CreateThankYou(
          [NotNull] string username, [NotNull] string textTag, [NotNull] string titleTag, int messageId)
        {
            return new ThankYouInfo
            {
                MessageID = messageId,
                ThanksInfo = ThanksNumber(username, messageId),
                Thanks = GetThanks(messageId),
                Text = YafContext.Current.Get<ILocalization>().GetText("BUTTON", textTag),
                Title = YafContext.Current.Get<ILocalization>().GetText("BUTTON", titleTag)
            };
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
        protected static string ThanksNumber([NotNull] string username, int messageID)
        {
            int thanksNumber = LegacyDb.message_ThanksNumber(messageID);

            string displayName = username;
            if (YafContext.Current.Get<YafBoardSettings>().EnableDisplayName)
            {
                // get the user's display name.
                MembershipUser mu = UserMembershipHelper.GetMembershipUserByName(username);
                if (mu != null)
                {
                    displayName = YafContext.Current.Get<IUserDisplayName>().GetName(
                        UserMembershipHelper.GetUserIDFromProviderUserKey(
                            mu.ProviderUserKey));
                }
            }

            displayName = YafContext.Current.Get<HttpServerUtilityBase>().HtmlEncode(displayName);

            string thanksText;

            switch (thanksNumber)
            {
                case 0:
                    return string.Empty;
                case 1:
                    thanksText =
                        YafContext.Current.Get<ILocalization>().GetText("POSTS", "THANKSINFOSINGLE").FormatWith(
                            displayName);

                    return
                        "<img id=\"ThanksInfoImage{0}\" src=\"{1}\"  runat=\"server\" title=\"{2}\" />&nbsp;{2}".FormatWith(
                                messageID,
                                YafContext.Current.Get<ITheme>().GetItem("ICONS", "THANKSINFOLIST_IMAGE"),
                                thanksText);
            }

            thanksText = YafContext.Current.Get<ILocalization>().GetText("POSTS", "THANKSINFO").FormatWith(thanksNumber, displayName);

            return
                "<img id=\"ThanksInfoImage{0}\" src=\"{1}\"  runat=\"server\" title=\"{2}\" />&nbsp;{2}".FormatWith(
                    messageID, YafContext.Current.Get<ITheme>().GetItem("ICONS", "THANKSINFOLIST_IMAGE"), thanksText);
        }

        #endregion
    }

    /// <summary>
    /// Thank You Info
    /// </summary>
    public class ThankYouInfo
    {
        /// <summary>
        /// Gets or sets Text.
        /// </summary>
        public string Text
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Title.
        /// </summary>
        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets MessageID.
        /// </summary>
        public int MessageID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets ThanksInfo.
        /// </summary>
        public string ThanksInfo
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Thanks.
        /// </summary>
        public string Thanks
        {
            get;
            set;
        }
    }
}