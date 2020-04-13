/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Core.Services
{
    using System.Text;
    using System.Web;

    using YAF.Configuration;
    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Core.UsersRoles;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Types.Objects;
    using YAF.Utils;

    /// <summary>
    ///  ThankYou Class to handle Thanks
    /// </summary>
    public class ThankYou : IThankYou
    {
        #region Public Methods

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
        public ThankYouInfo CreateThankYou(
            [NotNull] string username,
            [NotNull] string textTag,
            [NotNull] string titleTag,
            int messageId)
        {
            return new ThankYouInfo
                       {
                           MessageID = messageId,
                           ThanksInfo = BoardContext.Current.Get<IThankYou>().ThanksInfo(username, messageId),
                           Text = BoardContext.Current.Get<ILocalization>().GetText("BUTTON", textTag),
                           Title = BoardContext.Current.Get<ILocalization>().GetText("BUTTON", titleTag)
                       };
        }

        /// <summary>
        /// This method returns a string which shows how many times users have
        ///   thanked the message with the provided messageID. Returns an empty string.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="messageId">
        /// The Message ID.
        /// </param>
        /// <returns>
        /// The thanks number.
        /// </returns>
        public string ThanksInfo([NotNull] string username, int messageId)
        {
            var thanksNumber = BoardContext.Current.GetRepository<Thanks>().Count(t => t.MessageID == messageId);

            if (thanksNumber == 0)
            {
                return "&nbsp;";
            }

            var displayName = username;
            if (BoardContext.Current.Get<BoardSettings>().EnableDisplayName)
            {
                // get the user's display name.
                var mu = UserMembershipHelper.GetMembershipUserByName(username);
                if (mu != null)
                {
                    displayName = BoardContext.Current.Get<IUserDisplayName>().GetName(
                        UserMembershipHelper.GetUserIDFromProviderUserKey(mu.ProviderUserKey));
                }
            }

            displayName = BoardContext.Current.Get<HttpServerUtilityBase>().HtmlEncode(displayName);

            var thanksText = BoardContext.Current.Get<ILocalization>()
                .GetTextFormatted("THANKSINFO", thanksNumber, displayName);

            var thanks = GetThanks(messageId);

            return $@"<a class=""btn btn-sm btn-link thanks-popover"" 
                           data-toggle=""popover"" 
                           data-trigger=""click hover""
                           data-html=""true""
                           title=""{thanksText}"" 
                           data-content=""{thanks.Replace("\"", "'")}"">
                               <i class=""fa fa-heart"" style= ""color:#e74c3c""></i>&nbsp;+{thanksNumber}</a>";
        }

        /// <summary>
        /// This method returns a string containing the HTML code for
        ///   showing the the post footer. the HTML content is the name of users
        ///   who thanked the post and the date they thanked.
        /// </summary>
        /// <param name="messageId">
        /// The message Id.
        /// </param>
        /// <returns>
        /// The get thanks.
        /// </returns>
        [NotNull]
        private static string GetThanks([NotNull] int messageId)
        {
            var filler = new StringBuilder();

            var thanks = BoardContext.Current.GetRepository<Thanks>().MessageGetThanksList(messageId);

            filler.Append("<ol>");

            thanks.ForEach(
                dr =>
                    {
                        var name = BoardContext.Current.Get<BoardSettings>().EnableDisplayName
                                       ? BoardContext.Current.Get<HttpServerUtilityBase>()
                                           .HtmlEncode(dr.Item2.DisplayName)
                                       : BoardContext.Current.Get<HttpServerUtilityBase>().HtmlEncode(dr.Item2.Name);

                        // vzrus: quick fix for the incorrect link. URL rewriting don't work :(
                        filler.AppendFormat(
                            @"<li class=""list-inline-item""><a id=""{0}"" href=""{1}""><u>{2}</u></a>",
                            dr.Item2.ID,
                            BuildLink.GetLink(ForumPages.Profile, "u={0}&name={1}", dr.Item2.ID, name),
                            name);

                        if (BoardContext.Current.Get<BoardSettings>().ShowThanksDate)
                        {
                            filler.AppendFormat(
                                " {0}",
                                BoardContext.Current.Get<ILocalization>().GetTextFormatted(
                                    "ONDATE",
                                    BoardContext.Current.Get<IDateTime>().FormatDateShort(dr.Item1.ThanksDate)));
                        }

                        filler.Append("</li>");
                    });

            filler.Append("</ol>");

            return filler.ToString();
        }

        #endregion
    }
}