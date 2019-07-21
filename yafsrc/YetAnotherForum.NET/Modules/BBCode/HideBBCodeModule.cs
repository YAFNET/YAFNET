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
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Web;
    using YAF.Web.Controls;

    /// <summary>
    /// Hidden BBCode Module
    /// </summary>
    public class HideBBCodeModule : YafBBCodeControl
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

            var postsCount = -1;

            if (this.Parameters.ContainsKey(key: "posts"))
            {
                postsCount = int.Parse(s: this.Parameters[key: "posts"]);
            }

            var thanksCount = -1;

            if (this.Parameters.ContainsKey(key: "thanks"))
            {
                thanksCount = int.Parse(s: this.Parameters[key: "thanks"]);
            }

            var messageId = this.MessageID;

            if (hiddenContent.IsNotSet())
            {
                return;
            }

            var description = this.LocalizedString(
                tag: "HIDDENMOD_DESC",
                defaultStr: "The content of this post is hidden. After you THANK the poster, refresh the page to see the hidden content. You only need to thank the Current Post");

            var descriptionGuest = this.LocalizedString(
                tag: "HIDDENMOD_GUEST",
                defaultStr: "This board requires you to be registered and logged-in before you can view hidden messages.");

            var shownContentGuest = $"<div class=\"alert alert-danger\" role=\"alert\">{descriptionGuest}</div>";

            string shownContent;

            if (YafContext.Current.IsAdmin)
            {
                writer.Write(s: hiddenContent);
                return;
            }

            var userId = YafContext.Current.CurrentUserData.UserID;

            if (postsCount > -1)
            {
                // Handle Hide Posts Count X BBCOde
                var descriptionPost = string.Format(format: this.LocalizedString(
                    tag: "HIDDENMOD_POST",
                    defaultStr: "Hidden Content (You must be registered and have {0} post(s) or more)"), arg0: postsCount);

                var shownContentPost = $"<div class=\"alert alert-danger\" role=\"alert\">{descriptionPost}</div>";

                if (YafContext.Current.IsGuest)
                {
                    writer.Write(s: shownContentGuest);
                    return;
                }

                if (this.DisplayUserID == userId ||
                    YafContext.Current.CurrentUserData.NumPosts >= postsCount)
                {
                    shownContent = hiddenContent;
                }
                else
                {
                    shownContent = shownContentPost;
                }
            }
            else if (thanksCount > -1)
            {
                // Handle Hide Thanks Count X BBCode
                var descriptionPost = string.Format(format: this.LocalizedString(
                    tag: "HIDDENMOD_THANKS",
                    defaultStr: "Hidden Content (You must be registered and have at least {0} thank(s) received)"), arg0: thanksCount);

                var shownContentPost = $"<div class=\"alert alert-danger\" role=\"alert\">{descriptionPost}</div>";

                if (YafContext.Current.IsGuest)
                {
                    writer.Write(s: shownContentGuest);
                    return;
                }

                if (this.DisplayUserID == userId ||
                    this.GetRepository<Thanks>().Count(criteria: t => t.ThanksFromUserID == userId) >= thanksCount)
                {
                    shownContent = hiddenContent;
                }
                else
                {
                    shownContent = shownContentPost;
                }
            }
            else
            {
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
                    this.GetRepository<User>().ThankedMessage(messageId: messageId.ToType<int>(), userId: userId))
                {
                    // Show hidden content if user is the poster or have thanked the poster.
                    shownContent = hiddenContent;
                }
                else
                {
                    shownContent = $@"<div class=""alert alert-danger"" role=""alert"">
                <h4 class=""alert-heading"">Hidden Content</h4>
                <hr>
                <p class=""mb-0"">{description}</p>
</div>";
                }
            }

            writer.Write(s: shownContent);
        }
    }
}