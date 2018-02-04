/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
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

namespace YAF.Controls
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web.UI;

    using YAF.Classes;
    using YAF.Core;
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
                if (this.activeUserTable == null)
                {
                    // read there data from viewstate
                    if (this.ViewState["ActiveUserTable"] != null)
                    {
                        // cast it
                        this.activeUserTable = this.ViewState["ActiveUserTable"] as DataTable;
                    }
                }

                // return datatable
                return this.activeUserTable;
            }

            set
            {
                // save it to viewstate
                this.ViewState["ActiveUserTable"] = value;

                // save it also to local variable to avoid repetitive casting from viewstate in getter
                this.activeUserTable = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether treat displaying of guest users same way as that of hidden users.
        /// </summary>
        public bool TreatGuestAsHidden
        {
            get
            {
                return this.ViewState["TreatGuestAsHidden"] != null
                       && Convert.ToBoolean(this.ViewState["TreatGuestAsHidden"]);
            }

            set
            {
                this.ViewState["TreatGuestAsHidden"] = value;
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
            get
            {
                // return string.empty for null. 
                return (this.ViewState["InstantId"] as string) + string.Empty;
            }

            set
            {
                this.ViewState["InstantId"] = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Raises PreRender event and prepares control for rendering by creating links to active users.
        /// </summary>
        /// <param name="e">Ein <see cref="T:System.EventArgs" />-Objekt, das die Ereignisdaten enthält.</param>
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
            foreach (DataRow row in this.ActiveUserTable.Rows)
            {
                UserLink userLink;

                var isCrawler = row["IsCrawler"].ToType<int>() > 0;

                // create new link and set its parameters
                if (isCrawler)
                {
                    if (crawlers.Contains(row["Browser"].ToString()))
                    {
                        continue;
                    }

                    crawlers.Add(row["Browser"].ToString());
                    userLink = new UserLink
                                   {
                                       CrawlerName = row["Browser"].ToString(),
                                       UserID = row["UserID"].ToType<int>(),
                                       Style = this.Get<YafBoardSettings>().UseStyledNicks
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
                                       Style =
                                           this.Get<YafBoardSettings>().UseStyledNicks
                                               ? this.Get<IStyleTransform>().DecodeStyleByString(
                                                   row["Style"].ToString())
                                               : string.Empty,
                                       ReplaceName =
                                           this.Get<YafBoardSettings>().EnableDisplayName
                                               ? row["UserDisplayName"].ToString()
                                               : row["UserName"].ToString()
                                   };
                    userLink.ID = "UserLink{0}{1}".FormatWith(this.InstantId, userLink.UserID);
                }

                // how many users of this type is present (valid for guests, others have it 1)
                var userCount = row["UserCount"].ToType<int>();
                if (userCount > 1 && (!isCrawler || !this.Get<YafBoardSettings>().ShowCrawlersInActiveList))
                {
                    // add postfix if there is more the one user of this name
                    userLink.PostfixText = " ({0})".FormatWith(userCount);
                }

                // indicates whether user link should be added or not
                var addControl = true;

                // we might not want to add this user link if user is marked as hidden
                if (Convert.ToBoolean(row["IsHidden"]) || // or if user is guest and guest should be hidden
                    (this.TreatGuestAsHidden && Convert.ToBoolean(row["IsGuest"])))
                {
                    // hidden user are always visible to admin and himself)
                    if (this.PageContext.IsAdmin || userLink.UserID == this.PageContext.PageUserID)
                    {
                        // but use css style to distinguish such users
                        userLink.CssClass = "active_hidden";

                        // and also add postfix
                        userLink.PostfixText = " ({0})".FormatWith(this.GetText("HIDDEN"));
                    }
                    else
                    {
                        // user is hidden from this user...
                        addControl = false;
                    }
                }

                // add user link if it's not supressed
                if (!addControl)
                {
                    continue;
                }

                // vzrus: if guests or crawlers there can be a control with the same id. 
                var ul = this.FindControlRecursiveAs<UserLink>(userLink.ID);
                if (ul != null)
                {
                    this.Controls.Remove(ul);
                }

                this.Controls.Add(userLink);
            }
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
            writer.WriteLine(@"<p class=""card-text"">");

            // indicates whether we are processing first active user
            var isFirst = true;

            // cycle through active user links contained within this control (see OnPreRender where this links are added)
            foreach (var control in this.Controls.Cast<Control>()
                .Where(control => control is UserLink && control.Visible))
            {
                // control is visible UserLink
                // if we are rendering other then first UserLink, write down separator first to divide it from previus link
                if (!isFirst)
                {
                    writer.WriteLine(", ");
                }
                else
                {
                    // we are past first link
                    isFirst = false;
                }

                // render UserLink
                control.RenderControl(writer);
            }

            // write ending tag
            writer.WriteLine("</p>");
        }

        #endregion
    }
}