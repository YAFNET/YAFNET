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
    /// <param name="stringBuilder">
    ///     The string Builder.
    /// </param>
    public async override Task RenderAsync(StringBuilder stringBuilder)
    {
        var hiddenContent = this.Parameters["inner"];

        var groupString = this.Parameters["group"];

        if (hiddenContent.IsNotSet())
        {
            return;
        }

        var descriptionGuest = this.LocalizedString(
            "HIDDENMOD_GUEST",
            "This board requires you to be registered and logged-in before you can view hidden messages.");

        var shownContentGuest = $"<div class=\"alert alert-danger\" role=\"alert\">{descriptionGuest}</div>";

        if (groupString.IsNotSet())
        {
            // Hide from Guests only
            if (BoardContext.Current.IsGuest)
            {
                stringBuilder.Append(shownContentGuest);
                return;
            }
        }
        else
        {
            if (BoardContext.Current.IsGuest)
            {
                stringBuilder.Append(shownContentGuest);
                return;
            }

            descriptionGuest = this.LocalizedString(
                "HIDDENMOD_GROUP",
                "You don´t have the right to see the Hidden Content.");

            shownContentGuest = $"<div class=\"alert alert-danger\" role=\"alert\">{descriptionGuest}</div>";

            var groups = groupString.Split(';');

            // Check For Role Hiding
            if ((await this.Get<IAspNetRolesHelper>().GetRolesForUserAsync(
                     BoardContext.Current.MembershipUser)).Any(role => !groups.Exists(role.Equals)))
            {
                shownContentGuest = hiddenContent;
            }
        }

        // Override Admin, or PageUser is Post Author
        if (BoardContext.Current.IsAdmin || this.DisplayUserID == BoardContext.Current.PageUserID)
        {
            shownContentGuest = hiddenContent;
        }

        stringBuilder.Append(shownContentGuest);
    }
}