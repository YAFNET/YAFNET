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
    /// <param name="writer">
    /// The writer.
    /// </param>
    override protected void Render(HtmlTextWriter writer)
    {
        var userName = this.Parameters["inner"];

        if (userName.StartsWith("@"))
        {
            userName = userName.Replace("@", string.Empty);
        }

        if (userName.IsNotSet() || userName.Length > 50)
        {
            return;
        }

        var user = this.Get<IAspNetUsersHelper>().GetUserByName(userName.Trim());

        if (user != null)
        {
            var boardUser = this.GetRepository<User>().GetSingle(u => u.ProviderUserKey == user.Id);

            if (boardUser == null)
            {
                writer.Write(this.HtmlEncode(userName));
                return;
            }

            writer.Write("<!-- BEGIN userlink -->");

            var badgeColor = boardUser == BoardContext.Current.PageUser ? "text-bg-primary" : "text-bg-secondary";

            writer.Write("<span class=\"badge rounded-pill ");

            writer.Write(badgeColor);

            writer.Write(" fs-6\">");

            writer.WriteBeginTag("a");

            writer.WriteAttribute("href", this.Get<LinkBuilder>().GetUserProfileLink(boardUser.ID, boardUser.DisplayOrUserName()));

            writer.WriteAttribute(HtmlTextWriterAttribute.Class.ToString(), "link-light");

            writer.Write(HtmlTextWriter.TagRightChar);

            writer.Write(this.HtmlEncode(boardUser.DisplayOrUserName()));


            writer.WriteEndTag("a");

            writer.Write("</span>");
            writer.Write("<!-- END userlink -->");
        }
        else
        {
            writer.Write(this.HtmlEncode(userName));
        }
    }
}