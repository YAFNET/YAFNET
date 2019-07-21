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
    using System.Linq;
    using System.Web.UI;

    using YAF.Core;
    using YAF.Types.Extensions;
    using YAF.Web;
    using YAF.Web.Controls;

    /// <summary>
    /// Hide Group BBCode Module
    /// </summary>
    public class GroupHide : YafBBCodeControl
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

            var groupString = this.Parameters[key: "group"];

            if (hiddenContent.IsNotSet())
            {
                return;
            }

           var descriptionGuest = this.LocalizedString(
                tag: "HIDDENMOD_GUEST",
                defaultStr: "This board requires you to be registered and logged-in before you can view hidden messages.");

            var shownContentGuest = $"<div class=\"alert alert-danger\" role=\"alert\">{descriptionGuest}</div>";

            if (groupString.IsNotSet())
            {
                // Hide from Guests only
                if (YafContext.Current.IsGuest)
                {
                    writer.Write(s: shownContentGuest);
                    return;
                }
            }
            else
            {
                if (YafContext.Current.IsGuest)
                {
                    writer.Write(s: shownContentGuest);
                    return;
                }

                descriptionGuest = this.LocalizedString(
                tag: "HIDDENMOD_GROUP",
                defaultStr: "You dont´t have the right to see the Hidden Content.");

                shownContentGuest = $"<div class=\"alert alert-danger\" role=\"alert\">{descriptionGuest}</div>";

                var groups = groupString.Split(';');

                /*List<string> groups = new List<string>();
                List<string> ranks = new List<string>();

                foreach (string group in groupsAndRanks)
                {
                    if (group.StartsWith("group."))
                    {
                        groups.Add(group.Substring(group.IndexOf(".") + 1));
                    }
                    else if (group.StartsWith("rank."))
                    {
                        ranks.Add(group.Substring(group.IndexOf(".") + 1));
                    }
                    else
                    {
                        groups.Add(group);
                    }
                }*/

                // Check For Role Hiding
                if (RoleMembershipHelper.GetRolesForUser(
                            username: YafContext.Current.User.UserName).Any(predicate: role => !groups.Any(predicate: role.Equals)))
                {
                    shownContentGuest = hiddenContent;
                }

                // TODO : Check for Rank Hiding 
                /*if (ranks.Any())
                {
                    var yafUserData = new CombinedUserDataHelper(YafContext.Current.CurrentUserData.PageUserID);

                    if (!ranks.Where(rank => yafUserData.RankName.Equals(rank)).Any())
                    {
                        shownContentGuest = hiddenContent;
                    }
                }*/
            }

            // Override Admin, or User is Post Author
            if (YafContext.Current.IsAdmin || this.DisplayUserID == YafContext.Current.CurrentUserData.UserID)
            {
                shownContentGuest = hiddenContent;
            }

            writer.Write(s: shownContentGuest);
        }
    }
}