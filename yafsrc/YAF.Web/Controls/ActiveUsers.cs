/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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
    using System.Data;
    using System.Linq;
    using System.Web.UI;

    using YAF.Configuration;
    using YAF.Core.BaseControls;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils.Helpers;

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
        private DataTable activeUserTable;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets list of users to display in control.
        /// </summary>
        [CanBeNull]
        public DataTable ActiveUserTable
        {
            get
            {
                if (this.activeUserTable != null)
                {
                    return this.activeUserTable;
                }

                // read there data from view state
                if (this.ViewState["ActiveUserTable"] != null)
                {
                    // cast it
                    this.activeUserTable = this.ViewState["ActiveUserTable"] as DataTable;
                }

                // return data table
                return this.activeUserTable;
            }

            set
            {
                // save it to view state
                this.ViewState["ActiveUserTable"] = value;

                // save it also to local variable to avoid repetitive casting from ViewState in getter
                this.activeUserTable = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether treat displaying of guest users same way as that of hidden users.
        /// </summary>
        public bool TreatGuestAsHidden
        {
            get =>
                this.ViewState["TreatGuestAsHidden"] != null && Convert.ToBoolean(this.ViewState["TreatGuestAsHidden"]);

            set => this.ViewState["TreatGuestAsHidden"] = value;
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
            if (this.ActiveUserTable == null)
            {
                return;
            }

            // add style column if there is no such column in the table
            // style column defines how concrete user's link should be styled
            if (!this.ActiveUserTable.Columns.Contains("Style"))
            {
                this.ActiveUserTable.Columns.Add("Style", typeof(string));
                this.ActiveUserTable.AcceptChanges();
            }

            var crawlers = new List<string>();

            // go through the table and process each row
            this.ActiveUserTable.Rows.Cast<DataRow>().ForEach(
                row =>
                    {
                        UserLink userLink;

                        var isCrawler = row["IsCrawler"].ToType<int>() > 0;

                        // create new link and set its parameters
                        if (isCrawler)
                        {
                            if (crawlers.Contains(row["Browser"].ToString()))
                            {
                                return;
                            }

                            crawlers.Add(row["Browser"].ToString());
                            userLink = new UserLink
                                           {
                                               CrawlerName = row["Browser"].ToString(),
                                               UserID = row["UserID"].ToType<int>(),
                                               Style = this.Get<BoardSettings>().UseStyledNicks
                                                           ? this.Get<IStyleTransform>().DecodeStyleByString(
                                                               row["Style"].ToString())
                                                           : string.Empty
                                           };
                            userLink.ID += userLink.CrawlerName;
                        }
                        else
                        {
                            userLink = new UserLink
                                           {
                                               UserID = row["UserID"].ToType<int>(),
                                               Style = this.Get<BoardSettings>().UseStyledNicks
                                                           ? this.Get<IStyleTransform>().DecodeStyleByString(
                                                               row["Style"].ToString())
                                                           : string.Empty,
                                               ReplaceName = this.Get<BoardSettings>().EnableDisplayName
                                                                 ? row["UserDisplayName"].ToString()
                                                                 : row["UserName"].ToString()
                                           };
                            userLink.ID = $"UserLink{this.InstantId}{userLink.UserID}";
                        }

                        // how many users of this type is present (valid for guests, others have it 1)
                        var userCount = row["UserCount"].ToType<int>();
                        if (userCount > 1 && (!isCrawler || !this.Get<BoardSettings>().ShowCrawlersInActiveList))
                        {
                            // add postfix if there is more the one user of this name
                            userLink.PostfixText = $" ({userCount})";
                        }

                        // indicates whether user link should be added or not
                        var addControl = true;

                        // we might not want to add this user link if user is marked as hidden
                        if (Convert.ToBoolean(row["IsHidden"]) || // or if user is guest and guest should be hidden
                            this.TreatGuestAsHidden && Convert.ToBoolean(row["IsGuest"]))
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

                        // vzrus: if guests or crawlers there can be a control with the same id. 
                        var ul = this.FindControlRecursiveAs<UserLink>(userLink.ID);
                        if (ul != null)
                        {
                            this.Controls.Remove(ul);
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