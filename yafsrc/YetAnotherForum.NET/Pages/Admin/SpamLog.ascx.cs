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

namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.Globalization;
    using System.Linq;
    using System.Web.UI.WebControls;

    using FarsiLibrary.Utils;

    using YAF.Core.BasePages;
    using YAF.Core.Extensions;
    using YAF.Core.Helpers;
    using YAF.Core.Model;
    using YAF.Core.Utilities;
    using YAF.Core.Utilities.Helpers;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Objects.Model;
    using YAF.Web.Controls;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// The SPAM Event Log Page.
    /// </summary>
    public partial class SpamLog : AdminPage
    {
        #region Methods

        /// <summary>
        /// Delete Selected Event Log Entry
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void DeleteAllClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.GetRepository<Types.Models.EventLog>().DeleteAll();

            // re-bind controls
            this.BindData();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            this.List.ItemCommand += this.ListItemCommand;

            base.OnInit(e);
        }

        /// <summary>
        /// The On PreRender event.
        /// </summary>
        /// <param name="e">
        /// the Event Arguments
        /// </param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            this.PageContext.PageElements.RegisterJsBlockStartup(
               "DatePickerJs",
               JavaScriptBlocks.DatePickerLoadJs(
                   this.GetText("COMMON", "CAL_JQ_CULTURE_DFORMAT"),
                   this.GetText("COMMON", "CAL_JQ_CULTURE")));

            this.PageContext.PageElements.RegisterJsBlock("dropDownToggleJs", JavaScriptBlocks.DropDownToggleJs());

            this.PageContext.PageElements.RegisterJsBlock(
                "collapseToggleJs",
                JavaScriptBlocks.CollapseToggleJs(
                    this.GetText("ADMIN_EVENTLOG", "HIDE"),
                    this.GetText("ADMIN_EVENTLOG", "SHOW")));

            base.OnPreRender(e);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            // do it only once, not on post-backs
            if (this.IsPostBack)
            {
                return;
            }

            this.PageSize.DataSource = StaticDataHelper.PageEntries();
            this.PageSize.DataTextField = "Name";
            this.PageSize.DataValueField = "Value";
            this.PageSize.DataBind();

            var ci = this.Get<ILocalization>().Culture;

            if (this.PageContext.BoardSettings.UseFarsiCalender && ci.IsFarsiCulture())
            {
                this.SinceDate.Text = PersianDateConverter.ToPersianDate(PersianDate.MinValue).ToString("d");
                this.ToDate.Text = PersianDateConverter.ToPersianDate(PersianDate.Now).ToString("d");
            }
            else
            {
                this.SinceDate.Text = DateTime.UtcNow.AddDays(-this.PageContext.BoardSettings.EventLogMaxDays).ToString(
                                             ci.DateTimeFormat.ShortDatePattern, CultureInfo.InvariantCulture);
                this.ToDate.Text = DateTime.UtcNow.Date.ToString(
                                             ci.DateTimeFormat.ShortDatePattern, CultureInfo.InvariantCulture);
            }

            this.ToDate.ToolTip = this.SinceDate.ToolTip = this.GetText("COMMON", "CAL_JQ_TT");

            // bind data to controls
            this.BindData();
        }

        /// <summary>
        /// Creates page links for this page.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddRoot();

            // administration index second
            this.PageLinks.AddAdminIndex();

            this.PageLinks.AddLink(this.GetText("ADMIN_SPAMLOG", "TITLE"), string.Empty);

            this.Page.Header.Title = this.GetText("ADMIN_SPAMLOG", "TITLE");
        }

        /// <summary>
        /// Handles the PageChange event of the PagerTop control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        protected void PagerTopPageChange([NotNull] object sender, [NotNull] EventArgs e)
        {
            // rebind
            this.BindData();
        }

        /// <summary>
        /// Handles the Click event of the ApplyButton control.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="eventArgs">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void ApplyButtonClick([NotNull] object source, EventArgs eventArgs)
        {
            this.BindData();
        }

        /// <summary>
        /// The page size on selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void PageSizeSelectedIndexChanged(object sender, EventArgs e)
        {
            this.BindData();
        }

        /// <summary>
        /// Renders the UserLink
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected string UserLink([NotNull] PagedEventLog item)
        {
            var userLink = new UserLink
            {
                UserID = item.UserID,
                Suspended = item.Suspended,
                Style = item.UserStyle,
                ReplaceName = this.PageContext.BoardSettings.EnableDisplayName ? item.DisplayName : item.Name
            };

            return userLink.RenderToString();
        }

        /// <summary>
        /// Populates data source and binds data to controls.
        /// </summary>
        private void BindData()
        {
            var baseSize = this.PageSize.SelectedValue.ToType<int>();
            var currentPageIndex = this.PagerTop.CurrentPageIndex;
            this.PagerTop.PageSize = baseSize;

            var sinceDate = DateTime.UtcNow.AddDays(-this.PageContext.BoardSettings.EventLogMaxDays);
            var toDate = DateTime.UtcNow;

            var ci = this.Get<ILocalization>().Culture;

            if (this.SinceDate.Text.IsSet())
            {
                if (this.PageContext.BoardSettings.UseFarsiCalender && ci.IsFarsiCulture())
                {
                    var persianDate = new PersianDate(this.SinceDate.Text.PersianNumberToEnglish());

                    sinceDate = PersianDateConverter.ToGregorianDateTime(persianDate);
                }
                else
                {
                    DateTime.TryParse(this.SinceDate.Text, ci, DateTimeStyles.None, out sinceDate);
                }
            }

            if (this.ToDate.Text.IsSet())
            {
                if (this.PageContext.BoardSettings.UseFarsiCalender && ci.IsFarsiCulture())
                {
                    var persianDate = new PersianDate(this.ToDate.Text.PersianNumberToEnglish());

                    toDate = PersianDateConverter.ToGregorianDateTime(persianDate);
                }
                else
                {
                    DateTime.TryParse(this.ToDate.Text, ci, DateTimeStyles.None, out toDate);
                }
            }

            // list event for this board
            var list = this.GetRepository<Types.Models.EventLog>()
                               .ListPaged(
                                   this.PageContext.PageBoardID,
                                   this.PageContext.BoardSettings.EventLogMaxMessages,
                                   this.PageContext.BoardSettings.EventLogMaxDays,
                                   currentPageIndex,
                                   baseSize,
                                   sinceDate,
                                   toDate.AddDays(1).AddMinutes(-1),
                                   null,
                                   true);

            this.List.DataSource = list;

            this.PagerTop.Count = list != null && list.Any()
                                      ? list.FirstOrDefault().TotalRows
                                      : 0;

            // bind data to controls
            this.DataBind();

            if (this.List.Items.Count == 0)
            {
                this.NoInfo.Visible = true;
            }
        }

        /// <summary>
        /// Handles single record commands in a repeater.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.RepeaterCommandEventArgs"/> instance containing the event data.</param>
        private void ListItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
        {
            // what command are we serving?
            switch (e.CommandName)
            {
                // delete log entry
                case "delete":

                    // delete just this particular log entry
                    this.GetRepository<Types.Models.EventLog>().DeleteById(e.CommandArgument.ToType<int>());

                    // re-bind controls
                    this.BindData();
                    break;
            }
        }

        #endregion
    }
}