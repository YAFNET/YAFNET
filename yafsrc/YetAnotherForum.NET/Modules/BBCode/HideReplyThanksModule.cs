/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
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

namespace YAF.Modules.BBCode
{
    using System.Web.UI;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    /// <summary>
    /// Hide Reply Thanks BBCode Module
    /// </summary>
    public class HideReplyThanksModule : YafBBCodeControl
    {
        /// <summary>
        /// The render.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected override void Render(HtmlTextWriter writer)
        {
            var hiddenContent = this.Parameters["inner"];

            var messageId = this.MessageID;

            if (hiddenContent.IsNotSet())
            {
                return;
            }

            var description = this.LocalizedString(
                     "HIDEMOD_REPLYTHANKS",
                     "Hidden Content (You must be registered and reply to the message, or give thank, to see the hidden Content)");

            var descriptionGuest = this.LocalizedString(
                "HIDDENMOD_GUEST",
                "This board requires you to be registered and logged-in before you can view hidden messages.");

            var shownContentGuest =
                "<div class=\"alert alert-danger\" role=\"alert\">{0}</div>"
                    .FormatWith(descriptionGuest);

            var shownContent =
                "<div class=\"alert alert-warning\" role=\"alert\">{0}</div>"
                    .FormatWith(description);

            if (YafContext.Current.IsAdmin)
            {
                writer.Write(hiddenContent);
                return;
            }

            var userId = YafContext.Current.CurrentUserData.UserID;

            // Handle Hide Thanks
            if (!this.Get<YafBoardSettings>().EnableThanksMod)
            {
                writer.Write(hiddenContent);
                return;
            }

            if (YafContext.Current.IsGuest)
            {
                writer.Write(shownContentGuest);
                return;
            }

            if (this.DisplayUserID == userId || 
                LegacyDb.user_ThankedMessage(messageId.ToType<int>(), userId) ||
                LegacyDb.user_RepliedTopic(messageId.ToType<int>(), userId))
            {
                // Show hidden content if user is the poster or have thanked the poster.
                shownContent = hiddenContent;
            }

            writer.Write(shownContent);
        }
    }
}