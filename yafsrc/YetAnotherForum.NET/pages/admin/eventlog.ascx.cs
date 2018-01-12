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

namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Web.UI.WebControls;

    using FarsiLibrary.Utils;

    using YAF.Classes;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utilities;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The Admin Event Log Page.
    /// </summary>
    public partial class eventlog : AdminPage
    {
        #region Methods

        /// <summary>
        /// Delete Selected Event Log Entry
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void DeleteAllClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.GetRepository<EventLog>()
                .DeleteByUser(userId: this.PageContext.PageUserID, boardId: this.PageContext.PageBoardID);

            // re-bind controls
            this.BindData();
        }

        /// <summary>
        /// Gets HTML IMG code representing given log event icon.
        /// </summary>
        /// <param name="dataRow">
        /// Data row containing event log entry data.
        /// </param>
        /// <returns>
        /// return HTML code of event log entry image
        /// </returns>
        protected string EventCssClass([NotNull] object dataRow)
        {
            // cast object to the DataRowView
            var row = (DataRowView)dataRow;

            string cssClass;

            try
            {
                // find out of what type event log entry is
                var eventType = (EventLogTypes)row["Type"].ToType<int>();

                switch (eventType)
                {
                    case EventLogTypes.Error:
                        cssClass = "danger";
                        break;
                    case EventLogTypes.Warning:
                        cssClass = "warning";
                        break;
                    case EventLogTypes.Information:
                        cssClass = "info";
                        break;
                    default:
                        cssClass = "active";
                        break;
                }
            }
            catch (Exception)
            {
                return "active";
            }

            return cssClass;
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
            // setup jQuery and DatePicker JS...
            YafContext.Current.PageElements.RegisterJsBlockStartup(
                "DatePickerJs",
                JavaScriptBlocks.DatePickerLoadJs(
                    this.GetText("COMMON", "CAL_JQ_CULTURE_DFORMAT"),
                    this.GetText("COMMON", "CAL_JQ_CULTURE")));

            base.OnPreRender(e);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            // do it only once, not on postbacks
            if (this.IsPostBack)
            {
                return;
            }

            this.Types.Items.Add(new ListItem(this.GetText("ALL"), "-1"));

            foreach (int eventTypeId in Enum.GetValues(typeof(EventLogTypes)))
            {
                var eventTypeName = this.Get<ILocalization>().GetText(
                    "ADMIN_EVENTLOGROUPACCESS",
                    "LT_{0}".FormatWith(Enum.GetName(typeof(EventLogTypes), eventTypeId).ToUpperInvariant()));

                this.Types.Items.Add(
                    new ListItem(eventTypeName, eventTypeId.ToString()));
            }

            var ci = this.Get<ILocalization>().Culture;

            if (this.Get<YafBoardSettings>().UseFarsiCalender && ci.IsFarsiCulture())
            {
                this.SinceDate.Text = PersianDateConverter.ToPersianDate(PersianDate.MinValue).ToString("d");
                this.ToDate.Text = PersianDateConverter.ToPersianDate(PersianDate.Now).ToString("d");
            }
            else
            {
                this.SinceDate.Text = DateTime.UtcNow.AddDays(-this.Get<YafBoardSettings>().EventLogMaxDays).ToString(
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
            this.PageLinks.AddLink(
                this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));

            this.PageLinks.AddLink(this.GetText("ADMIN_EVENTLOG", "TITLE"), string.Empty);

            this.Page.Header.Title = "{0} - {1}".FormatWith(
                this.GetText("ADMIN_ADMIN", "Administration"), this.GetText("ADMIN_EVENTLOG", "TITLE"));

            this.PagerTop.PageSize = 25;
        }

        /// <summary>
        /// The pager top_ page change.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
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
        /// Populates data source and binds data to controls.
        /// </summary>
        private void BindData()
        {
            var baseSize = this.Get<YafBoardSettings>().MemberListPageSize;
            var currentPageIndex = this.PagerTop.CurrentPageIndex;
            this.PagerTop.PageSize = baseSize;

            var sinceDate = DateTime.UtcNow.AddDays(-this.Get<YafBoardSettings>().EventLogMaxDays);
            var toDate = DateTime.UtcNow;

            var ci = this.Get<ILocalization>().Culture;

            if (this.SinceDate.Text.IsSet())
            {
                if (this.Get<YafBoardSettings>().UseFarsiCalender && ci.IsFarsiCulture())
                {
                    var persianDate = new PersianDate(this.SinceDate.Text);

                    sinceDate = PersianDateConverter.ToGregorianDateTime(persianDate);
                }
                else
                {
                    DateTime.TryParse(this.SinceDate.Text, ci, DateTimeStyles.None, out sinceDate);
                }
            }

            if (this.ToDate.Text.IsSet())
            {
                if (this.Get<YafBoardSettings>().UseFarsiCalender && ci.IsFarsiCulture())
                {
                    var persianDate = new PersianDate(this.ToDate.Text);

                    toDate = PersianDateConverter.ToGregorianDateTime(persianDate);
                }
                else
                {
                    DateTime.TryParse(this.ToDate.Text, ci, DateTimeStyles.None, out toDate);
                }
            }

            // list event for this board
            var dt = this.GetRepository<EventLog>()
                               .List(
                                   this.PageContext.PageUserID,
                                   this.Get<YafBoardSettings>().EventLogMaxMessages,
                                   this.Get<YafBoardSettings>().EventLogMaxDays,
                                   currentPageIndex,
                                   baseSize,
                                   sinceDate,
                                   toDate.AddDays(1).AddMinutes(-1),
                                   this.Types.SelectedValue.Equals("-1") ? null : this.Types.SelectedValue);

            this.List.DataSource = dt;

            this.PagerTop.Count = dt != null && dt.HasRows()
                                      ? dt.AsEnumerable().First().Field<int>("TotalRows")
                                      : 0;

            // bind data to controls
            this.DataBind();
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
                    this.GetRepository<EventLog>().DeleteById(e.CommandArgument.ToType<int>());

                    // re-bind controls
                    this.BindData();
                    break;
            }
        }

        #endregion
    }
}