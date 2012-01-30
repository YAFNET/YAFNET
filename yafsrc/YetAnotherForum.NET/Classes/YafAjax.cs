/* YetAnotherForum.NET
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

namespace YAF.Classes
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web.Script.Serialization;
    using System.Web.Script.Services;
    using System.Web.Security;
    using System.Web.Services;
    using System.Xml;

    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
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

        /// <summary>
        /// Spell check via google api.
        /// </summary>
        /// <param name="text">
        /// The text to check.
        /// </param>
        /// <param name="lang">
        /// The langauage of the text.
        /// </param>
        /// <param name="engine">
        /// The engine.
        /// </param>
        /// <param name="suggest">
        /// The suggest words.
        /// </param>
        /// <returns>
        /// Returns List of Suggest Words.
        /// </returns>
        [WebMethod(EnableSession = true)]
        public string SpellCheck(string text, string lang, string engine, string suggest)
        {
            if (suggest.Equals("undefined", StringComparison.OrdinalIgnoreCase))
            {
                suggest = string.Empty;
            }

            string xml;

            List<string> result;

            if (string.IsNullOrEmpty(suggest))
            {
                xml = GetSpellCheckRequest(text, lang);
                result = GetListOfMisspelledWords(xml, text);
            }
            else
            {
                xml = GetSpellCheckRequest(suggest, lang);
                result = GetListOfSuggestWords(xml, suggest);
            }

            return new JavaScriptSerializer().Serialize(result);
        }

        /// <summary>
        /// SSO Login From Facebook
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="first_name">
        /// The first name.
        /// </param>
        /// <param name="last_name">
        /// The last name.
        /// </param>
        /// <param name="link">
        /// The link.
        /// </param>
        /// <param name="username">
        /// The user name.
        /// </param>
        /// <param name="birthday">
        /// The birthday.
        /// </param>
        /// <param name="hometown">
        /// The hometown.
        /// </param>
        /// <param name="gender">
        /// The gender.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <param name="timezone">
        /// The timezone.
        /// </param>
        /// <param name="locale">
        /// The locale.
        /// </param>
        /// <param name="remember">
        /// The remember.
        /// </param>
        /// <returns>
        /// Returns the Login Status
        /// </returns>
        [WebMethod(EnableSession = true)]
        public string LoginFacebookUser(
            string id,
            string name,
            string first_name,
            string last_name,
            string link,
            string username,
            string birthday,
            string hometown,
            string gender,
            string email,
            string timezone,
            string locale,
            bool remember)
        {
            if (!YafContext.Current.Get<YafBoardSettings>().AllowSingleSignOn)
            {
                return this.Get<ILocalization>().GetText("LOGIN", "SSO_DEACTIVATED");
            }

            // Check if username is null
            if (string.IsNullOrEmpty(username))
            {
                username = name;
            }

            var userGender = 0;

            if (!string.IsNullOrEmpty(gender))
            {
                switch (gender)
                {
                    case "male":
                        userGender = 1;
                        break;
                    case "female":
                        userGender = 2;
                        break;
                }
            }

            // Check if user exists
            var userName = YafContext.Current.Get<MembershipProvider>().GetUserNameByEmail(email);

            // Login user if exists
            if (!string.IsNullOrEmpty(userName))
            {
                var yafUser = YafUserProfile.GetProfile(userName);

                var yafUserData =
                    new CombinedUserDataHelper(YafContext.Current.Get<MembershipProvider>().GetUser(userName, true));

                if (!yafUserData.UseSingleSignOn)
                {
                    return this.Get<ILocalization>().GetText("LOGIN", "SSO_DEACTIVATED_BYUSER");
                }

                if (yafUser.Facebook.Equals(id))
                {
                    // Add Flag to User that indicates that the user is logged in via facebook
                    LegacyDb.user_update_single_sign_on_status(yafUserData.UserID, true, false);

                    FormsAuthentication.SetAuthCookie(userName, remember);

                    YafContext.Current.Get<IRaiseEvent>().Raise(
                        new SuccessfulUserLoginEvent(YafContext.Current.PageUserID));

                    return "OK";
                }

                return this.Get<ILocalization>().GetText("LOGIN", "SSO_ID_NOTMATCH");
            }

            // Create User if not exists?!
            if (YafContext.Current.Get<YafBoardSettings>().RegisterNewFacebookUser &&
                !YafContext.Current.Get<YafBoardSettings>().DisableRegistrations)
            {
                MembershipCreateStatus status;

                var pass = Membership.GeneratePassword(32, 16);
                var securityAnswer = Membership.GeneratePassword(64, 30);

                MembershipUser user = this.Get<MembershipProvider>().CreateUser(
                    username, pass, email, "Answer is a generated Pass", securityAnswer, true, null, out status);

                // setup inital roles (if any) for this user
                RoleMembershipHelper.SetupUserRoles(YafContext.Current.PageBoardID, username);

                // create the user in the YAF DB as well as sync roles...
                int? userID = RoleMembershipHelper.CreateForumUser(user, YafContext.Current.PageBoardID);

                // create empty profile just so they have one
                YafUserProfile userProfile = YafUserProfile.GetProfile(username);

                userProfile.Facebook = id;
                userProfile.Homepage = link;

                if (!string.IsNullOrEmpty(birthday))
                {
                    DateTime userBirthdate;
                    var ci = CultureInfo.CreateSpecificCulture("en-US");
                    DateTime.TryParse(birthday, ci, DateTimeStyles.None, out userBirthdate);

                    if (userBirthdate > DateTime.MinValue.Date)
                    {
                        userProfile.Birthday = userBirthdate;
                    }
                }

                userProfile.RealName = username;
                userProfile.Gender = userGender;
                
                if (!string.IsNullOrEmpty(hometown))
                {
                    userProfile.Location = hometown;
                }

                userProfile.Save();

                // setup their inital profile information
                userProfile.Save();

                if (userID == null)
                {
                    // something is seriously wrong here -- redirect to failure...
                    return this.Get<ILocalization>().GetText("LOGIN", "SSO_FAILED");
                }

                if (this.Get<YafBoardSettings>().NotificationOnUserRegisterEmailList.IsSet())
                {
                    // send user register notification to the following admin users...
                    this.SendRegistrationNotificationEmail(user);
                }

                // send user register notification to the following admin users...
                this.SendRegistrationNotificationToUser(user, pass, securityAnswer);

                // save the time zone...
                int userId = UserMembershipHelper.GetUserIDFromProviderUserKey(user.ProviderUserKey);

                LegacyDb.user_save(
                    userId,
                    YafContext.Current.PageBoardID,
                    username,
                    null,
                    email,
                    timezone,
                    null,
                    null,
                    null,
                    true,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null);

                bool autoWatchTopicsEnabled = this.Get<YafBoardSettings>().DefaultNotificationSetting ==
                                              UserNotificationSetting.TopicsIPostToOrSubscribeTo;

                // save the settings...
                LegacyDb.user_savenotification(
                    userId,
                    true,
                    autoWatchTopicsEnabled,
                    this.Get<YafBoardSettings>().DefaultNotificationSetting,
                    this.Get<YafBoardSettings>().DefaultSendDigestEmail);

                // save avatar
                LegacyDb.user_saveavatar(userId, "https://graph.facebook.com/{0}/picture".FormatWith(id), null, null);

                // Clearing cache with old Active User Lazy Data ...
                this.Get<IDataCache>().Remove(Constants.Cache.ActiveUserLazyData.FormatWith(userId));

                this.Get<IRaiseEvent>().Raise(new NewUserRegisteredEvent(user, userId));

                // Add Flag to User that indicates that the user is logged in via facebook
                LegacyDb.user_update_single_sign_on_status(userId, true, false);

                FormsAuthentication.SetAuthCookie(user.UserName, remember);

                YafContext.Current.Get<IRaiseEvent>().Raise(new SuccessfulUserLoginEvent(YafContext.Current.PageUserID));

                return "OK";
            }

            return this.Get<ILocalization>().GetText("LOGIN", "SSO_FAILED");
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
        /// The refresh shout box js.
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
        /// The add favorite topic js.
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
        /// The remove favorite topic js.
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
        /// The msg id.
        /// </param>
        /// <returns>
        /// Returns ThankYou Info
        /// </returns>
        [CanBeNull]
        [WebMethod(EnableSession = true)]
        public ThankYouInfo AddThanks([NotNull] object msgID)
        {
            var messageID = msgID.ToType<int>();

            string username =
                LegacyDb.message_AddThanks(
                    UserMembershipHelper.GetUserIDFromProviderUserKey(Membership.GetUser().ProviderUserKey), messageID);

            // if the user is empty, return a null object...
            return username.IsNotSet()
                       ? null
                       : YafThankYou.CreateThankYou(
                           username, "BUTTON_THANKSDELETE", "BUTTON_THANKSDELETE_TT", messageID);
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

            string username =
                LegacyDb.message_RemoveThanks(
                    UserMembershipHelper.GetUserIDFromProviderUserKey(Membership.GetUser().ProviderUserKey), messageID);

            return YafThankYou.CreateThankYou(username, "BUTTON_THANKS", "BUTTON_THANKS_TT", messageID);
        }

        #endregion

        #endregion

        #region private methods

        /// <summary>
        /// Gets the list of suggest words.
        /// </summary>
        /// <param name="xml">
        /// The XML.
        /// </param>
        /// <param name="suggest">
        /// The suggest.
        /// </param>
        /// <returns>
        /// The get list of suggest words.
        /// </returns>
        private static List<string> GetListOfSuggestWords(string xml, string suggest)
        {
            if (string.IsNullOrEmpty(xml) || string.IsNullOrEmpty(suggest))
            {
                return null;
            }

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            if (!xmlDoc.HasChildNodes)
            {
                return null;
            }

            XmlNodeList nodeList = xmlDoc.SelectNodes("//c");

            if (null == nodeList || 0 >= nodeList.Count)
            {
                return null;
            }

            List<string> list = new List<string>();

            foreach (XmlNode node in nodeList)
            {
                list.AddRange(node.InnerText.Split('\t'));
                return list;
            }

            return list;
        }

        /// <summary>
        /// Gets the list of misspelled words.
        /// </summary>
        /// <param name="xml">
        /// The XML.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The get list of misspelled words.
        /// </returns>
        private static List<string> GetListOfMisspelledWords(string xml, string text)
        {
            if (string.IsNullOrEmpty(xml) || string.IsNullOrEmpty(text))
            {
                return null;
            }

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            if (!xmlDoc.HasChildNodes)
            {
                return null;
            }

            var nodeList = xmlDoc.SelectNodes("//c");

            if (null == nodeList || 0 >= nodeList.Count)
            {
                return null;
            }

            return (from XmlNode node in nodeList
                    let offset = node.Attributes["o"].Value.ToType<int>()
                    let length = node.Attributes["l"].Value.ToType<int>()
                    select text.Substring(offset, length)).ToList();
        }

        /// <summary>
        /// Requests the spell check and get the result back.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="lang">
        /// The lang.
        /// </param>
        /// <returns>
        /// The get spell check request.
        /// </returns>
        private static string GetSpellCheckRequest(string text, string lang)
        {
            var requestUrl = ConstructRequestUrl(text, lang);
            var requestContentXml = ConstructSpellRequestContentXml(text);

            byte[] buffer = Encoding.UTF8.GetBytes(requestContentXml);

            WebClient webClient = new WebClient();
            webClient.Headers.Add("Content-Type", "text/xml");
            byte[] response = webClient.UploadData(requestUrl, "POST", buffer);
            return Encoding.UTF8.GetString(response);
        }

        /// <summary>
        /// Constructs the request URL.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="lang">
        /// The lang.
        /// </param>
        /// <returns>
        /// The construct request url.
        /// </returns>
        private static string ConstructRequestUrl(string text, string lang)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            lang = string.IsNullOrEmpty(lang) ? "en" : lang;

            return "https://www.google.com/tbproxy/spell?lang={0}&text={1}".FormatWith(lang, text);
        }

        /// <summary>
        /// Constructs the spell request content XML.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The construct spell request content xml.
        /// </returns>
        private static string ConstructSpellRequestContentXml(string text)
        {
            var doc = new XmlDocument();
            var declaration = doc.CreateXmlDeclaration("1.0", null, null);

            doc.AppendChild(declaration);

            var root = doc.CreateElement("spellrequest");

            root.SetAttribute("textalreadyclipped", "0");
            root.SetAttribute("ignoredups", "0");
            root.SetAttribute("ignoredigits", "1");
            root.SetAttribute("ignoreallcaps", "1");

            doc.AppendChild(root);

            var textElement = doc.CreateElement("text");

            textElement.InnerText = text;
            root.AppendChild(textElement);

            return doc.InnerXml;
        }

        /// <summary>
        /// Send an Email to the Newly Created User with
        /// his Account Info (Pass, Security Question and Answer)
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="pass">
        /// The pass.
        /// </param>
        /// <param name="securityAnswer">
        /// The security answer.
        /// </param>
        private void SendRegistrationNotificationToUser(
            [NotNull] MembershipUser user, [NotNull] string pass, [NotNull] string securityAnswer)
        {
            var notifyUser = new YafTemplateEmail();

            string subject =
                this.Get<ILocalization>().GetText("COMMON", "NOTIFICATION_ON_NEW_FACEBOOK_USER_SUBJECT").FormatWith(
                    this.Get<YafBoardSettings>().Name);

            notifyUser.TemplateParams["{user}"] = user.UserName;
            notifyUser.TemplateParams["{email}"] = user.Email;
            notifyUser.TemplateParams["{pass}"] = pass;
            notifyUser.TemplateParams["{answer}"] = securityAnswer;
            notifyUser.TemplateParams["{forumname}"] = this.Get<YafBoardSettings>().Name;

            string emailBody = notifyUser.ProcessTemplate("NOTIFICATION_ON_FACEBOOK_REGISTER");

            this.Get<ISendMail>().Queue(this.Get<YafBoardSettings>().ForumEmail, user.Email, subject, emailBody);
        }

        /// <summary>
        /// The send registration notification email.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        private void SendRegistrationNotificationEmail([NotNull] MembershipUser user)
        {
            string[] emails = this.Get<YafBoardSettings>().NotificationOnUserRegisterEmailList.Split(';');

            var notifyAdmin = new YafTemplateEmail();

            string subject =
                this.Get<ILocalization>().GetText("COMMON", "NOTIFICATION_ON_USER_REGISTER_EMAIL_SUBJECT").FormatWith(
                    this.Get<YafBoardSettings>().Name);

            notifyAdmin.TemplateParams["{adminlink}"] = YafBuildLink.GetLinkNotEscaped(ForumPages.admin_admin, true);
            notifyAdmin.TemplateParams["{user}"] = user.UserName;
            notifyAdmin.TemplateParams["{email}"] = user.Email;
            notifyAdmin.TemplateParams["{forumname}"] = this.Get<YafBoardSettings>().Name;

            string emailBody = notifyAdmin.ProcessTemplate("NOTIFICATION_ON_USER_REGISTER");

            foreach (string email in emails.Where(email => email.Trim().IsSet()))
            {
                this.Get<ISendMail>().Queue(this.Get<YafBoardSettings>().ForumEmail, email.Trim(), subject, emailBody);
            }
        }

        #endregion
    }
}