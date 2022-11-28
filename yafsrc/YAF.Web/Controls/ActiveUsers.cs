/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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

namespace YAF.Web.Controls;

/// <summary>
/// Control displaying list of user currently active on a forum.
/// </summary>
public class ActiveUsers : BaseControl
{
    /// <summary>
    ///   The _active user table.
    /// </summary>
    private List<ActiveUser> activeUsers;

    /// <summary>
    ///   Gets or sets list of users to display in control.
    /// </summary>
    [CanBeNull]
    public List<ActiveUser> ActiveUsersList
    {
        get
        {
            if (this.activeUsers != null)
            {
                return this.activeUsers;
            }

            // read there data from view state
            if (this.ViewState["ActiveUsers"] != null)
            {
                // cast it
                this.activeUsers = this.ViewState["ActiveUsers"] as List<ActiveUser>;
            }

            // return data table
            return this.activeUsers;
        }

        set
        {
            // save it to view state
            this.ViewState["ActiveUsers"] = value;

            // save it also to local variable to avoid repetitive casting from ViewState in getter
            this.activeUsers = value;
        }
    }

    /// <summary>
    /// Raises PreRender event and prepares control for rendering by creating links to active users.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnPreRender([NotNull] EventArgs e)
    {
        // IMPORTANT : call base implementation, raises PreRender event
        base.OnPreRender(e);

        // we shall continue only if there are active user data available
        if (this.ActiveUsersList == null)
        {
            return;
        }

        var crawlers = new List<string>();

        // go through the table and process each row
        this.ActiveUsersList.ForEach(
            user =>
            {
                UserLink userLink;

                var isCrawler = user.ActiveFlags.IsCrawler;

                // create new link and set its parameters
                if (isCrawler)
                {
                    if (crawlers.Contains(user.Browser))
                    {
                        return;
                    }

                    crawlers.Add(user.Browser);
                    userLink = new UserLink
                               {
                                   Suspended = user.Suspended,
                                   CrawlerName = user.Browser,
                                   UserID = user.UserID,
                                   Style = user.UserStyle
                               };
                }
                else
                {
                    userLink = new UserLink
                               {
                                   Suspended = user.Suspended,
                                   UserID = user.UserID,
                                   Style = user.UserStyle,
                                   ReplaceName = this.PageBoardContext.BoardSettings.EnableDisplayName
                                                     ? user.UserDisplayName
                                                     : user.UserName
                               };
                }

                // how many users of this type is present (valid for guests, others have it 1)
                var userCount = user.UserCount;
                if (userCount > 1 && (!isCrawler || !this.PageBoardContext.BoardSettings.ShowCrawlersInActiveList))
                {
                    // add postfix if there is more the one user of this name
                    userLink.PostfixText = $" ({userCount})";
                }

                // if user is guest and guest should be hidden
                var addControl = !(user.IsGuest && !this.PageBoardContext.IsAdmin);

                // we might not want to add this user link if user is marked as hidden
                if (user.IsActiveExcluded)
                {
                    // hidden user are always visible to admin and himself)
                    if (this.PageBoardContext.IsAdmin || userLink.UserID == this.PageBoardContext.PageUserID)
                    {
                        userLink.PostfixText = "  <i class=\"fas fa-user-secret\"></i>";
                    }
                    else
                    {
                        // user is hidden from this user...
                        addControl = false;
                    }
                }

                // add user link if it's not suppressed
                if (!addControl)
                {
                    return;
                }

                this.Controls.Add(userLink);
            });
    }

    /// <summary>
    /// Implements rendering of control to the client through use of <see cref="HtmlTextWriter"/>.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected override void Render([NotNull] HtmlTextWriter writer)
    {
        // writes starting tag
        writer.Write(@"<ul class=""list-inline"">");

        // cycle through active user links contained within this control (see OnPreRender where this links are added)
        this.Controls.Cast<Control>().Where(control => control is UserLink && control.Visible).ForEach(
            control =>
                {
                    writer.Write(@"<li class=""list-inline-item"">");

                    // render UserLink
                    control.RenderControl(writer);

                    writer.Write(@"</li>");
                });

        // writes ending tag
        writer.Write(@"</ul>");
    }
}