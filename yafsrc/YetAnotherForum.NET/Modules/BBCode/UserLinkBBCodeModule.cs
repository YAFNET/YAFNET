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
namespace YAF.Modules
{
    using System.Text;
    using System.Web.UI;

    using YAF.Controls;
    using YAF.Core;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils.Helpers;

    /// <summary>
    /// The BB Code UserLink Module
    /// </summary>
    public class UserLinkBBCodeModule : YafBBCodeControl
    {
        /// <summary>
        /// The render.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected override void Render(HtmlTextWriter writer)
        {
            var userName = this.Parameters[key: "inner"];

            if (userName.IsNotSet() || userName.Length > 50)
            {
                return;
            }

            var userId = this.Get<IUserDisplayName>().GetId(name: userName.Trim());

            if (userId.HasValue)
            {
                var stringBuilder = new StringBuilder();

                var userLink = new UserLink
                                   {
                                       UserID = userId.ToType<int>(),
                                       CssClass = "btn btn-outline-primary",
                                       BlankTarget = true,
                                       ID = $"UserLinkBBCodeFor{userId}"
                                   };

                stringBuilder.AppendLine(value: "<!-- BEGIN userlink -->");
                stringBuilder.AppendLine(value: @"<span>");
                stringBuilder.AppendLine(value: userLink.RenderToString());

                stringBuilder.AppendLine(value: "</span>");
                stringBuilder.AppendLine(value: "<!-- END userlink -->");

                writer.Write(s: stringBuilder.ToString());
            }
            else
            {
                writer.Write(s: this.HtmlEncode(data: userName));
            }
        }
    }
}