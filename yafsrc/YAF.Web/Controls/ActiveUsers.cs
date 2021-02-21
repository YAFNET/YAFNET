/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2021 Ingo Herbote
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

namespace YAF.Web.Controls
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI;

    using YAF.Core.BaseControls;
    using YAF.Core.Utilities.Helpers;
    using YAF.Types;
    using YAF.Types.Extensions;

    #endregion

    /// <summary>
    /// Control displaying list of user currently active on a forum.
    /// </summary>
    public class ActiveUsers : BaseControl
    {
        #region Constants and Fields

        /// <summary>
        ///   The _active user table.
        /// </summary>
        private List<dynamic> activeUsers;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets list of users to display in control.
        /// </summary>
        [CanBeNull]
        public List<dynamic> ActiveUsersList
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
                    this.activeUsers = this.ViewState["ActiveUsers"] as List<dynamic>;
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
        /// Gets or sets the Instant ID for this control. 
        /// </summary> 
        /// <remarks> 
        /// Multiple instants of this control can exist in the same page but 
        /// each must have a different instant ID. Not specifying an Instant ID 
        /// default to the ID being string.Empty. 
        /// </remarks> 
        public string InstantId
        {
            get => (this.ViewState["InstantId"] as string) + string.Empty;

            set => this.ViewState["InstantId"] = value;
        }

        #endregion

        #region Methods

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

                        var isCrawler = (int)user.IsCrawler > 0;

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
                                Style = (string)user.UserStyle
                            };
                            userLink.ID += userLink.CrawlerName;
                        }
                        else
                        {
                            userLink = new UserLink
                            {
                                Suspended = user.Suspended,
                                UserID = user.UserID,
                                Style = (string)user.UserStyle,
                                ReplaceName = this.PageContext.BoardSettings.EnableDisplayName
                                    ? user.UserDisplayName
                                    : user.UserName
                            };
                            userLink.ID = $"UserLink{this.InstantId}{userLink.UserID}";
                        }
                        
                        // how many users of this type is present (valid for guests, others have it 1)
                        var userCount = (int)user.UserCount;
                        if (userCount > 1 && (!isCrawler || !this.PageContext.BoardSettings.ShowCrawlersInActiveList))
                        {
                            // add postfix if there is more the one user of this name
                            userLink.PostfixText = $" ({userCount})";
                        }

                        // indicates whether user link should be added or not
                        var addControl = true;

                        // we might not want to add this user link if user is marked as hidden
                        if (user.IsActiveExcluded == true || // or if user is guest and guest should be hidden
                            user.IsGuest == true)
                        {
                            // hidden user are always visible to admin and himself)
                            if (this.PageContext.IsAdmin || userLink.UserID == this.PageContext.PageUserID)
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

        #endregion
    }
}