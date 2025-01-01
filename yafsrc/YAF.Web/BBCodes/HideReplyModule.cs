/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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
/// Hide Reply BBCode Module
/// </summary>
public class HideReplyModule : BBCodeControl
{
    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    override protected void Render(HtmlTextWriter writer)
    {
        var hiddenContent = this.Parameters["inner"];

        if (hiddenContent.IsNotSet())
        {
            return;
        }

        var description = this.LocalizedString(
            "HIDEMOD_REPLY",
            "Hidden Content (You must be registered and reply to the message to see the hidden Content)");

        var descriptionGuest = this.LocalizedString(
            "HIDDENMOD_GUEST",
            "This board requires you to be registered and logged-in before you can view hidden messages.");

        var shownContentGuest = $"<div class=\"alert alert-danger\" role=\"alert\">{descriptionGuest}</div>";

        var shownContent = $"<div class=\"alert alert-warning\" role=\"alert\">{description}</div>";

        if (BoardContext.Current.IsAdmin)
        {
            writer.Write(hiddenContent);
            return;
        }

        var userId = BoardContext.Current.PageUserID;

        if (BoardContext.Current.IsGuest)
        {
            writer.Write(shownContentGuest);
            return;
        }

        if (this.DisplayUserID == userId ||
            this.GetRepository<Message>().RepliedTopic(this.PageBoardContext.PageTopicID, userId))
        {
            // Show hidden content if user is the poster or have thanked the poster.
            shownContent = hiddenContent;
        }

        writer.Write(shownContent);
    }
}