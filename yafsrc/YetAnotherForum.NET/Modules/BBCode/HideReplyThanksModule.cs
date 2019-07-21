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

    using YAF.Configuration;
    using YAF.Core;
    using YAF.Core.Model;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Web;
    using YAF.Web.Controls;

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
            var hiddenContent = this.Parameters[key: "inner"];

            var messageId = this.MessageID;

            if (hiddenContent.IsNotSet())
            {
                return;
            }

            var description = this.LocalizedString(
                     tag: "HIDEMOD_REPLYTHANKS",
                     defaultStr: "Hidden Content (You must be registered and reply to the message, or give thank, to see the hidden Content)");

            var descriptionGuest = this.LocalizedString(
                tag: "HIDDENMOD_GUEST",
                defaultStr: "This board requires you to be registered and logged-in before you can view hidden messages.");

            var shownContentGuest = $"<div class=\"alert alert-danger\" role=\"alert\">{descriptionGuest}</div>";

            var shownContent = $"<div class=\"alert alert-warning\" role=\"alert\">{description}</div>";

            if (YafContext.Current.IsAdmin)
            {
                writer.Write(s: hiddenContent);
                return;
            }

            var userId = YafContext.Current.CurrentUserData.UserID;

            // Handle Hide Thanks
            if (!this.Get<YafBoardSettings>().EnableThanksMod)
            {
                writer.Write(s: hiddenContent);
                return;
            }

            if (YafContext.Current.IsGuest)
            {
                writer.Write(s: shownContentGuest);
                return;
            }

            if (this.DisplayUserID == userId ||
                this.GetRepository<User>().ThankedMessage(messageId: messageId.ToType<int>(), userId: userId) ||
                this.GetRepository<User>().RepliedTopic(messageId: messageId.ToType<int>(), userId: userId))
            {
                // Show hidden content if user is the poster or have thanked the poster.
                shownContent = hiddenContent;
            }

            writer.Write(s: shownContent);
        }
    }
}