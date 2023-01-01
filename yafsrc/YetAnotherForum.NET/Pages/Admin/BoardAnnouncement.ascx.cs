/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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

namespace YAF.Pages.Admin;

/// <summary>
/// The Admin Board Announcement Page.
/// </summary>
public partial class BoardAnnouncement : AdminPage
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "Profile.Attachments" /> class.
    /// </summary>
    public BoardAnnouncement()
        : base("ADMIN_BOARDSETTINGS", ForumPages.Admin_BoardAnnouncement)
    {
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
        this.PageBoardContext.PageElements.RegisterJsBlockStartup(
            nameof(JavaScriptBlocks.FormValidatorJs),
            JavaScriptBlocks.FormValidatorJs(this.SaveAnnouncement.ClientID));

        if (this.IsPostBack)
        {
            return;
        }

        this.BindData();
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddLink(this.PageBoardContext.BoardSettings.Name, this.Get<LinkBuilder>().GetLink(ForumPages.Board));
        this.PageBoardContext.PageLinks.AddAdminIndex();
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_BOARDSETTINGS", "ANNOUNCEMENT_TITLE"), string.Empty);
    }

    /// <summary>
    /// Saves the Board Announcement
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void SaveAnnouncementClick([NotNull] object sender, [NotNull] EventArgs e)
    {
        var boardAnnouncementUntil = DateTime.Now;

        // number inserted by suspending user
        var count = int.Parse(this.BoardAnnouncementUntil.Text);

        // what time units are used for suspending
        boardAnnouncementUntil = this.BoardAnnouncementUntilUnit.SelectedValue switch
            {
                // days
                "1" => boardAnnouncementUntil.AddDays(count),
                // hours
                "2" => boardAnnouncementUntil.AddHours(count),
                // month
                "3" => boardAnnouncementUntil.AddMonths(count),
                _ => boardAnnouncementUntil
            };

        var boardSettings = this.PageBoardContext.BoardSettings;

        boardSettings.BoardAnnouncementUntil = boardAnnouncementUntil.ToString(CultureInfo.InvariantCulture);
        boardSettings.BoardAnnouncement = this.Message.Text;
        boardSettings.BoardAnnouncementType = this.BoardAnnouncementType.SelectedValue;

        // save the settings to the database
        this.Get<BoardSettingsService>().SaveRegistry(boardSettings);

        this.Get<LinkBuilder>().Redirect(ForumPages.Admin_BoardAnnouncement);
    }

    /// <summary>
    /// Deletes the Announcement
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void DeleteClick(object sender, EventArgs e)
    {
        var boardSettings = this.PageBoardContext.BoardSettings;

        boardSettings.BoardAnnouncementUntil = DateTime.MinValue.ToString(CultureInfo.InvariantCulture);
        boardSettings.BoardAnnouncement = null;
        boardSettings.BoardAnnouncementType = "info";

        // save the settings to the database
        this.Get<BoardSettingsService>().SaveRegistry(boardSettings);

        this.Get<LinkBuilder>().Redirect(ForumPages.Admin_BoardAnnouncement);
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private void BindData()
    {
        var boardSettings = this.PageBoardContext.BoardSettings;

        // add items to the dropdown
        this.BoardAnnouncementUntilUnit.Items.Add(new ListItem(this.GetText("PROFILE", "MONTH"), "3"));
        this.BoardAnnouncementUntilUnit.Items.Add(new ListItem(this.GetText("PROFILE", "DAYS"), "1"));
        this.BoardAnnouncementUntilUnit.Items.Add(new ListItem(this.GetText("PROFILE", "HOURS"), "2"));

        // select hours
        this.BoardAnnouncementUntilUnit.SelectedIndex = 0;

        // default number of hours to suspend user for
        this.BoardAnnouncementUntil.Text = "1";

        if (boardSettings.BoardAnnouncement.IsNotSet())
        {
            return;
        }

        this.CurrentAnnouncement.Visible = true;
        this.CurrentMessage.Text =
            $"<strong>{this.GetText("ANNOUNCEMENT_CURRENT")}:</strong>&nbsp;{boardSettings.BoardAnnouncementUntil}";
        this.BoardAnnouncementType.SelectedValue = boardSettings.BoardAnnouncementType;
        this.Message.Text = boardSettings.BoardAnnouncement;
    }
}