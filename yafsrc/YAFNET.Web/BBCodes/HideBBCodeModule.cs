/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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

namespace YAF.Web.BBCodes;

/// <summary>
/// Hidden BBCode Module
/// </summary>
public class HideBBCodeModule : BBCodeControl
{
    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="stringBuilder">
    ///     The string Builder.
    /// </param>
    public override Task RenderAsync(StringBuilder stringBuilder)
    {
        var hiddenContent = this.Parameters["inner"];

        var postsCount = -1;

        if (this.Parameters.TryGetValue("posts", out var parameter))
        {
            postsCount = int.Parse(parameter);
        }

        var thanksCount = -1;

        if (this.Parameters.TryGetValue("thanks", out var parameter1))
        {
            thanksCount = int.Parse(parameter1);
        }

        var messageId = this.MessageID;

        if (hiddenContent.IsNotSet())
        {
            return Task.CompletedTask;
        }

        var description = this.LocalizedString(
            "HIDDENMOD_DESC",
            "The content of this post is hidden. After you THANK the poster, refresh the page to see the hidden content. You only need to thank the Current Post");

        var descriptionGuest = this.LocalizedString(
            "HIDDENMOD_GUEST",
            "This board requires you to be registered and logged-in before you can view hidden messages.");

        var shownContentGuest = $"<div class=\"alert alert-danger\" role=\"alert\">{descriptionGuest}</div>";

        string shownContent;

        if (BoardContext.Current.IsAdmin)
        {
            stringBuilder.Append(hiddenContent);
            return Task.CompletedTask;
        }

        var userId = BoardContext.Current.PageUserID;

        if (postsCount > -1)
        {
            // Handle Hide Posts Count X BBCOde
            var descriptionPost = string.Format(
                this.LocalizedString(
                    "HIDDENMOD_POST",
                    "Hidden Content (You must be registered and have {0} post(s) or more)"),
                postsCount);

            var shownContentPost = $"<div class=\"alert alert-danger\" role=\"alert\">{descriptionPost}</div>";

            if (BoardContext.Current.IsGuest)
            {
                stringBuilder.Append(shownContentGuest);
                return Task.CompletedTask;
            }

            if (this.DisplayUserID == userId ||
                BoardContext.Current.PageUser.NumPosts >= postsCount)
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
            var descriptionPost = string.Format(
                this.LocalizedString(
                    "HIDDENMOD_THANKS",
                    "Hidden Content (You must be registered and have at least {0} thank(s) received)"),
                thanksCount);

            var shownContentPost = $"<div class=\"alert alert-danger\" role=\"alert\">{descriptionPost}</div>";

            if (BoardContext.Current.IsGuest)
            {
                stringBuilder.Append(shownContentGuest);
                return Task.CompletedTask;
            }

            if (this.DisplayUserID == userId ||
                this.GetRepository<Thanks>().Count(t => t.ThanksFromUserID == userId) >= thanksCount)
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
            if (BoardContext.Current.IsGuest)
            {
                stringBuilder.Append(shownContentGuest);
                return Task.CompletedTask;
            }

            if (this.DisplayUserID == userId ||
                this.GetRepository<Thanks>().ThankedMessage(messageId.ToType<int>(), userId))
            {
                // Show hidden content if user is the poster or have thanked the poster.
                shownContent = hiddenContent;
            }
            else
            {
                shownContent = $"""
                                <div class="alert alert-danger" role="alert">
                                                <h4 class="alert-heading">Hidden Content</h4>
                                                <hr>
                                                <p class="mb-0">{description}</p>
                                </div>
                                """;
            }
        }

        stringBuilder.Append(shownContent);

        return Task.CompletedTask;
    }
}