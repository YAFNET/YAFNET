/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Web.UI.WebControls;
    using FarsiLibrary;
    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utilities;
    using YAF.Utils;
    using YAF.Utils.Helpers;

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
        protected void DeleteAll_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            LegacyDb.eventlog_deletebyuser(this.PageContext.PageBoardID, this.PageContext.PageUserID);

            // re-bind controls
            this.BindData();
        }

        /// <summary>
        /// Handles load event for delete all button.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        /// <remarks>
        /// Adds confirmation popup to click event of this button.
        /// </remarks>
        protected void DeleteAll_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            ((ThemeButton)sender).Attributes["onclick"] =
                "return confirm('{0}')".FormatWith(this.GetText("ADMIN_EVENTLOG", "CONFIRM_DELETE_ALL"));
        }

        /// <summary>
        /// Handles load event for log entry delete link button.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        /// <remarks>
        /// Adds confirmation popup to click event of this button.
        /// </remarks>
        protected void Delete_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            ControlHelper.AddOnClickConfirmDialog(sender, this.GetText("ADMIN_EVENTLOG", "CONFIRM_DELETE"));
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
        protected string EventImageCode([NotNull] object dataRow)
        {
            // cast object to the DataRowView
            var row = (DataRowView)dataRow;

            // set defaults
            string imageType = Enum.GetName(typeof(EventLogTypes), (int)row["Type"]);

            if (imageType.IsNotSet())
            {
                imageType = "Error";
            }

            // return HTML code of event log entry image
            return
                @"<img src=""{0}"" alt=""{1}"" title=""{1}"" />".FormatWith(
                    YafForumInfo.GetURLToResource("icons/{0}.png".FormatWith(imageType.ToLowerInvariant())), imageType);
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
                        cssClass = "ui-state-error";
                        break;
                    case EventLogTypes.Warning:
                        cssClass = "ui-state-warning";
                        break;
                    case EventLogTypes.Information:
                        cssClass = "ui-state-highlight";
                        break;
                    default:
                        cssClass = "ui-state-highlight";
                        break;
                }
            }
            catch (Exception)
            {
                return "ui-state-highlight";
            }

            return cssClass;
        }

        /// <summary>
        /// The on init.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            this.List.ItemCommand += this.List_ItemCommand;

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
            YafContext.Current.PageElements.RegisterJQuery();
            YafContext.Current.PageElements.RegisterJQueryUI();

            var ci = CultureInfo.CreateSpecificCulture(PageContext.CultureUser);

            if (!string.IsNullOrEmpty(this.GetText("COMMON", "CAL_JQ_CULTURE")))
            {
                var jqueryuiUrl = !Config.JQueryUILangFile.StartsWith("http")
                                      ? YafForumInfo.GetURLToResource(Config.JQueryUILangFile)
                                      : Config.JQueryUILangFile;

                YafContext.Current.PageElements.RegisterJsInclude("datepickerlang", jqueryuiUrl);

                if (ci.IsFarsiCulture())
                {
                    YafContext.Current.PageElements.RegisterJsResourceInclude("datepicker-farsi", "js/jquery.ui.datepicker-farsi.js");
                }
            }

            YafContext.Current.PageElements.RegisterJsBlockStartup(
               "DatePickerJs",
               JavaScriptBlocks.DatePickerLoadJs(
                   "{0}, #{1}".FormatWith(this.ToDate.ClientID, this.SinceDate.ClientID),
                   this.GetText("COMMON", "CAL_JQ_CULTURE_DFORMAT"),
                   this.GetText("COMMON", "CAL_JQ_CULTURE")));

            YafContext.Current.PageElements.RegisterJsBlockStartup(
                "ToggleEventLogItemJs",
                JavaScriptBlocks.ToggleEventLogItemJs(
                    this.GetText("ADMIN_EVENTLOG", "SHOW"), this.GetText("ADMIN_EVENTLOG", "HIDE")));

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

            // create page links
            // board index first
            this.PageLinks.AddLink(this.Get<YafBoardSettings>().Name, YafBuildLink.GetLink(ForumPages.forum));

            // administration index second
            this.PageLinks.AddLink(
                this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));

            this.PageLinks.AddLink(this.GetText("ADMIN_EVENTLOG", "TITLE"), string.Empty);

            this.Page.Header.Title = "{0} - {1}".FormatWith(
                this.GetText("ADMIN_ADMIN", "Administration"), this.GetText("ADMIN_EVENTLOG", "TITLE"));

            this.PagerTop.PageSize = 25;

            this.Types.Items.Add(new ListItem(this.GetText("ALL"), "-1"));

            foreach (int eventTypeId in Enum.GetValues(typeof(EventLogTypes)))
            {
                var eventTypeName = this.GetText("ADMIN_EVENTLOGROUPACCESS",
                                                 "LT_{0}".FormatWith(Enum.GetName(typeof(EventLogTypes), eventTypeId).ToUpperInvariant()));

                this.Types.Items.Add(
                    new ListItem(eventTypeName, eventTypeId.ToString()));
            }

            var ci = CultureInfo.CreateSpecificCulture(this.GetCulture());

            if (this.Get<YafBoardSettings>().EnableDNACalendar) // <-- Should be removed legacy settting not needed anymore.
            {
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
            }
            else
            {
                this.SinceDate.Visible = false;
                this.SinceDateLabel.Visible = false;
                this.ToDate.Visible = false;
                this.ToDateLabel.Visible = false;
            }

            // bind data to controls
            this.BindData();
        }

        /// <summary>
        /// The pager top_ page change.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void PagerTop_PageChange([NotNull] object sender, [NotNull] EventArgs e)
        {
            // rebind
            this.BindData();
        }

        /// <summary>
        /// Handles the Click event of the ApplyButton control.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="eventArgs">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void ApplyButton_Click([NotNull] object source, EventArgs eventArgs)
        {
            this.BindData();
        }

        /// <summary>
        /// Populates data source and binds data to controls.
        /// </summary>
        private void BindData()
        {
            int baseSize = this.Get<YafBoardSettings>().MemberListPageSize;
            int nCurrentPageIndex = this.PagerTop.CurrentPageIndex;
            this.PagerTop.PageSize = baseSize;

            var sinceDate = DateTime.UtcNow.AddDays(-this.Get<YafBoardSettings>().EventLogMaxDays);
            var toDate = DateTime.UtcNow;

            var ci = CultureInfo.CreateSpecificCulture(this.GetCulture());

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
            DataTable dt = this.GetRepository<EventLog>()
                               .List(this.PageContext.PageUserID,
                                     this.Get<YafBoardSettings>().EventLogMaxMessages,
                                     this.Get<YafBoardSettings>().EventLogMaxDays,
                                     nCurrentPageIndex,
                                     baseSize,
                                     sinceDate,
                                     toDate.AddDays(1).AddMinutes(-1),
                                     this.Types.SelectedValue.Equals("-1") ? null : this.Types.SelectedValue);

            this.List.DataSource = dt;

            this.PagerTop.Count = dt != null && dt.Rows.Count > 0
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
        private void List_ItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
        {
            // what command are we serving?
            switch (e.CommandName)
            {
                // delete log entry
                case "delete":

                    // delete just this particular log entry
                    this.GetRepository<EventLog>().Delete(e.CommandArgument.ToType<int?>(), this.PageContext.PageUserID);

                    // re-bind controls
                    this.BindData();
                    break;
            }
        }

        /// <summary>
        /// Gets the culture.
        /// </summary>
        /// <returns>
        /// The get culture.
        /// </returns>
        private string GetCulture()
        {
            // Language and culture
            string languageFile = this.Get<YafBoardSettings>().Language;
            string culture4tag = this.Get<YafBoardSettings>().Culture;

            if (!string.IsNullOrEmpty(this.PageContext.LanguageFile))
            {
                languageFile = this.PageContext.LanguageFile;
            }

            if (!string.IsNullOrEmpty(this.PageContext.CultureUser))
            {
                culture4tag = this.PageContext.CultureUser;
            }

            // Get first default full culture from a language file tag.
            string langFileCulture = StaticDataHelper.CultureDefaultFromFile(languageFile);
            return langFileCulture.Substring(0, 2) == culture4tag.Substring(0, 2) ? culture4tag : langFileCulture;
        }

        #endregion
    }
}