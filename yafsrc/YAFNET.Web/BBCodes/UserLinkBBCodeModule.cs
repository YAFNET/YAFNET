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
/// The BB Code UserLink Module
/// </summary>
public class UserLinkBBCodeModule : BBCodeControl
{
    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="stringBuilder">
    ///     The string Builder.
    /// </param>
    public async override Task RenderAsync(StringBuilder stringBuilder)
    {
        var userName = this.Parameters["inner"];

        if (userName.StartsWith('@'))
        {
            userName = userName.Replace("@", string.Empty);
        }

        if (userName.IsNotSet() || userName.Length > 50)
        {
            return;
        }

        var user = await this.Get<IAspNetUsersHelper>().GetUserByNameAsync(userName.Trim());

        if (user is not null)
        {
            var boardUser = await this.GetRepository<User>().GetSingleAsync(u => u.ProviderUserKey == user.Id);

            if (boardUser is null)
            {
                stringBuilder.Append(HtmlEncode(userName));
                return;
            }

            var badgeColor = boardUser == BoardContext.Current.PageUser ? "text-bg-primary" : "text-bg-secondary";

            stringBuilder.Append("<!-- BEGIN user link -->");
            stringBuilder.Append("<span class=\"badge rounded-pill ");

            stringBuilder.Append(badgeColor);

            stringBuilder.Append(" fs-6\">");

            var link = new TagBuilder(HtmlTag.A);

            link.MergeAttribute(HtmlAttribute.Href, this.Get<LinkBuilder>().GetUserProfileLink(boardUser.ID, boardUser.DisplayOrUserName()));

            link.AddCssClass("link-light");

            link.InnerHtml.Append(boardUser.DisplayOrUserName());

            stringBuilder.Append(link.RenderToString());

            stringBuilder.Append("</span>");
            stringBuilder.Append("<!-- END user link -->");
        }
        else
        {
            stringBuilder.Append(HtmlEncode(userName));
        }
    }
}