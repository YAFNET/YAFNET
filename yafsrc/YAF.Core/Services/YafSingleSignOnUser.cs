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

namespace YAF.Core.Services
{
    using System.Linq;
    using System.Web.Security;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;

    /// <summary>
    /// Single Sign On User Class to handle Twitter and Facebook Logins
    /// </summary>
    public class YafSingleSignOnUser
    {
        /// <summary>
        /// Generates the oAUTH callback login URL.
        /// </summary>
        /// <param name="authService">The AUTH service.</param>
        /// <param name="generatePopUpUrl">if set to <c>true</c> [generate pop up URL].</param>
        /// <param name="connectCurrentUser">if set to <c>true</c> [connect current user].</param>
        /// <returns>
        /// Returns the login Url
        /// </returns>
        public static string GenerateLoginUrl([NotNull] AuthService authService, [NotNull] bool generatePopUpUrl, [CanBeNull] bool connectCurrentUser = false)
        {
            switch (authService)
            {
                case AuthService.twitter:
                    {
                        return new Auth.Twitter().GenerateLoginUrl(generatePopUpUrl, connectCurrentUser);
                    }

                case AuthService.facebook:
                    {
                        return new Auth.Facebook().GenerateLoginUrl(generatePopUpUrl, connectCurrentUser);
                    }

                case AuthService.google:
                    {
                        return new Auth.Google().GenerateLoginUrl(generatePopUpUrl, connectCurrentUser);
                    }

                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// The send registration notification email.
        /// </summary>
        /// <param name="user">The Membership User.</param>
        /// <param name="userID">The user identifier.</param>
        public static void SendRegistrationNotificationEmail([NotNull] MembershipUser user, [NotNull] int userID)
        {
            var emails = YafContext.Current.Get<YafBoardSettings>().NotificationOnUserRegisterEmailList.Split(';');

            var notifyAdmin = new YafTemplateEmail();

            var subject =
                YafContext.Current.Get<ILocalization>()
                    .GetText("COMMON", "NOTIFICATION_ON_USER_REGISTER_EMAIL_SUBJECT")
                    .FormatWith(YafContext.Current.Get<YafBoardSettings>().Name);

            notifyAdmin.TemplateParams["{adminlink}"] = YafBuildLink.GetLinkNotEscaped(
                ForumPages.admin_edituser,
                true,
                "u={0}",
                userID);
            notifyAdmin.TemplateParams["{user}"] = user.UserName;
            notifyAdmin.TemplateParams["{email}"] = user.Email;
            notifyAdmin.TemplateParams["{forumname}"] = YafContext.Current.Get<YafBoardSettings>().Name;

            string emailBody = notifyAdmin.ProcessTemplate("NOTIFICATION_ON_USER_REGISTER");

            foreach (string email in emails.Where(email => email.Trim().IsSet()))
            {
                YafContext.Current.GetRepository<Mail>()
                    .Create(YafContext.Current.Get<YafBoardSettings>().ForumEmail, email.Trim(), subject, emailBody);
            }
        }

        /// <summary>
        /// Do login and set correct flag
        /// </summary>
        /// <param name="authService">The AUTH service.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="userID">The user ID.</param>
        /// <param name="doLogin">if set to <c>true</c> [do login].</param>
        public static void LoginSuccess([NotNull] AuthService authService, [CanBeNull] string userName, [NotNull] int userID, [NotNull] bool doLogin)
        {
            // Add Flag to User that indicates with what service the user is logged in
            LegacyDb.user_update_single_sign_on_status(userID, authService);

            if (!doLogin)
            {
                return;
            }

            FormsAuthentication.SetAuthCookie(userName, true);

            YafContext.Current.Get<IRaiseEvent>().Raise(new SuccessfulUserLoginEvent(userID));
        }
    }
}