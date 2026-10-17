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
/// Hide Group BBCode Module
/// </summary>
public class GroupHide : BBCodeControl
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
        var groupString = this.Parameters["group"];

        if (hiddenContent.IsNotSet())
        {
            return;
        }

        // the admins and the author of the post always see the content
        if (BoardContext.Current.IsAdmin || this.DisplayUserID == BoardContext.Current.PageUserID)
        {
            writer.Write(hiddenContent);
            return;
        }

        if (BoardContext.Current.IsGuest)
        {
            var descriptionGuest = this.LocalizedString(
                "HIDDENMOD_GUEST",
                "This board requires you to be registered and logged-in before you can view hidden messages.");

            writer.Write($"<div class=\"alert alert-danger\" role=\"alert\">{descriptionGuest}</div>");
            return;
        }

        // without roles the content is only hidden from guests ([group-hide] showed it to nobody)
        if (groupString.IsNotSet())
        {
            writer.Write(hiddenContent);
            return;
        }

        // with roles the content is shown to the members of one of the roles (it was shown to the users with
        // any other role)
        var groups = groupString.Split(';').Select(g => g.Trim()).Where(g => g.Length > 0).ToList();

        var isMember = this.Get<IAspNetRolesHelper>().GetRolesForUser(BoardContext.Current.MembershipUser)
            .Any(role => groups.Exists(group => group.Equals(role, StringComparison.OrdinalIgnoreCase)));

        if (isMember)
        {
            writer.Write(hiddenContent);
            return;
        }

        var description = this.LocalizedString(
            "HIDDENMOD_GROUP",
            "You dont´t have the right to see the Hidden Content.");

        writer.Write($"<div class=\"alert alert-danger\" role=\"alert\">{description}</div>");
    }
}