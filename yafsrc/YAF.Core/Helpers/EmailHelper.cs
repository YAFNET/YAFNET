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
namespace YAF.Core
{
    using System;
    using System.Net.Mail;
    using System.Web.Security;

    using YAF.Classes;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;

    /// <summary>
    /// The email helper.
    /// </summary>
    public static class EmailHelper
    {
        #region Public Methods and Operators

        /// <summary>
        /// The send verification email.
        /// </summary>
        /// <param name="haveServiceLocator">
        /// The have service locator.
        /// </param>
        /// <param name="user"></param>
        public static void SendVerificationEmail(
            [NotNull] this IHaveServiceLocator haveServiceLocator, [NotNull] MembershipUser user, [NotNull] string email, int? userID, string newUsername = null)
        {
            CodeContracts.VerifyNotNull(email, "email");
            CodeContracts.VerifyNotNull(user, "user");
            CodeContracts.VerifyNotNull(haveServiceLocator, "haveServiceLocator");

            string hashinput = DateTime.UtcNow + email + Security.CreatePassword(20);
            string hash = FormsAuthentication.HashPasswordForStoringInConfigFile(hashinput, "md5");

            // save verification record...
            haveServiceLocator.GetRepository<CheckEmail>().Save(userID, hash, user.Email);

            var verifyEmail = new YafTemplateEmail("VERIFYEMAIL");

            string subject = haveServiceLocator.Get<ILocalization>().GetTextFormatted("VERIFICATION_EMAIL_SUBJECT", haveServiceLocator.Get<YafBoardSettings>().Name);

            verifyEmail.TemplateParams["{link}"] = YafBuildLink.GetLinkNotEscaped(ForumPages.approve, true, "k={0}", hash);
            verifyEmail.TemplateParams["{key}"] = hash;
            verifyEmail.TemplateParams["{forumname}"] = haveServiceLocator.Get<YafBoardSettings>().Name;
            verifyEmail.TemplateParams["{forumlink}"] = "{0}".FormatWith(YafForumInfo.ForumURL);

            verifyEmail.SendEmail(new MailAddress(email, newUsername ?? user.UserName), subject, true);
        }

        #endregion
    }
}