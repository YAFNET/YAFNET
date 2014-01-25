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