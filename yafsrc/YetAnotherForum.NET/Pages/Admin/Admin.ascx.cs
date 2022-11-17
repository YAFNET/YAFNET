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

namespace YAF.Pages.Admin;

using System.Net.Mail;
using FarsiLibrary.Utils;
using YAF.Types.Extensions.Data;
using YAF.Types.Interfaces.Data;
using YAF.Web.Controls;

using YAF.Types.Models;

/// <summary>
/// The Admin Index Page.
/// </summary>
public partial class Admin : AdminPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Admin"/> class. 
    /// </summary>
    public Admin()
        : base("ADMIN_ADMIN", ForumPages.Admin_Admin)
    {
    }

    /// <summary>
    /// Loads the Board Stats for the Selected Board
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    public void BoardStatsSelectChanged([NotNull] object sender, [NotNull] EventArgs e)
    {
        // re-bind data
        this.BindData();
    }

    /// <summary>
    /// Handles the ItemCommand event of the UserList control.
    /// </summary>
    /// <param name="source">The source of the event.</param>
    /// <param name="e">The <see cref="System.Web.UI.WebControls.RepeaterCommandEventArgs"/> instance containing the event data.</param>
    public void UserListItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "edit":
                this.Get<LinkBuilder>().Redirect(ForumPages.Admin_EditUser, new { u = e.CommandArgument });
                break;
            case "resendEmail":
                var commandArgument = e.CommandArgument.ToString().Split(';');

                var checkMail = this.GetRepository<CheckEmail>().ListTyped(commandArgument[0]).FirstOrDefault();

                if (checkMail != null)
                {
                    var verifyEmail = new TemplateEmail("VERIFYEMAIL");

                    var subject = this.Get<ILocalization>()
                        .GetTextFormatted("VERIFICATION_EMAIL_SUBJECT", this.PageBoardContext.BoardSettings.Name);

                    verifyEmail.TemplateParams["{link}"] = this.Get<LinkBuilder>().GetAbsoluteLink(
                        ForumPages.Account_Approve,
                        new { code = checkMail.Hash });
                    verifyEmail.TemplateParams["{key}"] = checkMail.Hash;
                    verifyEmail.TemplateParams["{forumname}"] = this.PageBoardContext.BoardSettings.Name;
                    verifyEmail.TemplateParams["{forumlink}"] = this.Get<LinkBuilder>().ForumUrl;

                    verifyEmail.SendEmail(new MailAddress(checkMail.Email, commandArgument[1]), subject);

                    this.PageBoardContext.Notify(
                        this.GetText("ADMIN_ADMIN", "MSG_MESSAGE_SEND"),
                        MessageTypes.success);
                }
                else
                {
                    var userFound = this.Get<IUserDisplayName>().FindUserContainsName(commandArgument[1]).FirstOrDefault();

                    var user = this.Get<IAspNetUsersHelper>().GetUserByName(userFound.Name);

                    this.Get<ISendNotification>().SendVerificationEmail(user, commandArgument[0], userFound.ID);
                }

                break;
            case "delete":
                if (!Config.IsAnyPortal)
                {
                    this.Get<IAspNetUsersHelper>().DeleteUser(e.CommandArgument.ToType<int>());
                }

                this.BindData();
                break;
            case "approve":
                this.Get<IAspNetUsersHelper>().ApproveUser(e.CommandArgument.ToType<int>());
                this.BindData();
                break;
            case "deleteall":

                // vzrus: Should not delete the whole providers portal data? Under investigation.
                var daysValueAll =
                    this.PageBoardContext.CurrentForumPage.FindControlRecursiveAs<TextBox>("DaysOld").Text.Trim();
                if (!ValidationHelper.IsValidInt(daysValueAll))
                {
                    this.PageBoardContext.Notify(
                        this.GetText("ADMIN_ADMIN", "MSG_VALID_DAYS"),
                        MessageTypes.warning);
                    return;
                }

                if (!Config.IsAnyPortal)
                {
                    this.Get<IAspNetUsersHelper>().DeleteAllUnapproved(DateTime.UtcNow.AddDays(-daysValueAll.ToType<int>()));
                }
                else
                {
                    this.GetRepository<User>().DeleteOld(this.PageBoardContext.PageBoardID, daysValueAll.ToType<int>());
                }

                this.BindData();
                break;
            case "approveall":
                this.Get<IAspNetUsersHelper>().ApproveAll();

                this.BindData();
                break;
        }
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
        if (this.IsPostBack)
        {
            return;
        }

        this.PageSize.DataSource = StaticDataHelper.PageEntries();
        this.PageSize.DataTextField = "Name";
        this.PageSize.DataValueField = "Value";
        this.PageSize.DataBind();

        this.PageSizeUnverified.DataSource = StaticDataHelper.PageEntries();
        this.PageSizeUnverified.DataTextField = "Name";
        this.PageSizeUnverified.DataValueField = "Value";
        this.PageSizeUnverified.DataBind();

        try
        {
            this.PageSize.SelectedValue =
                this.PageSizeUnverified.SelectedValue = this.PageBoardContext.PageUser.PageSize.ToString();
        }
        catch (Exception)
        {
            this.PageSize.SelectedValue = this.PageSizeUnverified.SelectedValue = "5";
        }

        this.BoardStatsSelect.Visible = this.PageBoardContext.PageUser.UserFlags.IsHostAdmin;

        // bind data
        this.BindBoardsList();

        this.BindData();

        this.ShowUpgradeMessage();
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        // forum index
        this.PageBoardContext.PageLinks.AddRoot();

        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_ADMIN", "Administration"), string.Empty);
    }

    /// <summary>
    /// The pager top_ page change.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void PagerTopChange([NotNull] object sender, [NotNull] EventArgs e)
    {
        this.BindActiveUserData();
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
        this.BindActiveUserData();
    }

    /// <summary>
    /// The pager top_ page change.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void PagerUnverifiedChange([NotNull] object sender, [NotNull] EventArgs e)
    {
        this.BindUnverifiedUsers();
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
    protected void PageSizeUnverifiedSelectedIndexChanged(object sender, EventArgs e)
    {
        this.BindUnverifiedUsers();
    }

    /// <summary>
    /// Shows the upgrade message.
    /// </summary>
    private void ShowUpgradeMessage()
    {
        try
        {
            var version = this.Get<IDataCache>().GetOrSet(
                "LatestVersion",
                () => this.Get<ILatestInformation>().GetLatestVersion(),
                TimeSpan.FromDays(1));

            var latestVersion = (DateTime) version.VersionDate;

            if (latestVersion.ToUniversalTime().Date <= BoardInfo.AppVersionDate.ToUniversalTime().Date)
            {
                return;
            }

            // updateLink
            this.UpdateHightlight.Visible = true;
            this.UpdateLinkHighlight.NavigateUrl = version.UpgradeUrl;
        }
        catch (Exception)
        {
            this.UpdateHightlight.Visible = false;
        }
    }

    /// <summary>
    /// Binds the active user data.
    /// </summary>
    private void BindActiveUserData()
    {
        this.PagerTop.PageSize = this.PageSize.SelectedValue.ToType<int>();

        var activeUsers = this.GetRepository<Active>().ListUsersPaged(
            this.PageBoardContext.PageUserID,
            true,
            true,
            this.PagerTop.CurrentPageIndex,
            this.PagerTop.PageSize);

        this.PagerTop.Count = activeUsers.Any() ?  activeUsers.First().UserCount : 0;

        this.ActiveList.DataSource = activeUsers;
        this.ActiveList.DataBind();
    }

    /// <summary>
    /// Bind list of boards to drop down
    /// </summary>
    private void BindBoardsList()
    {
        // only if user is host admin, otherwise boards' list is hidden
        if (!this.PageBoardContext.PageUser.UserFlags.IsHostAdmin)
        {
            return;
        }

        var boards = this.GetRepository<Board>().GetAll();

        // set data source
        this.BoardStatsSelect.DataSource = boards;
        this.BoardStatsSelect.DataBind();

        // select current board as default
        this.BoardStatsSelect.SelectedIndex =
            this.BoardStatsSelect.Items.IndexOf(
                this.BoardStatsSelect.Items.FindByValue(this.PageBoardContext.PageBoardID.ToString()));
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private void BindData()
    {
        this.BindUnverifiedUsers();

        // get stats for current board, selected board or all boards (see function)
        var data = this.GetRepository<Board>().Stats(this.GetSelectedBoardId());

        this.NumCategories.Text = data.Categories.ToString();
        this.NumForums.Text = data.Forums.ToString();
        this.NumTopics.Text = $@"{data.Topics:N0}";
        this.NumPosts.Text = $@"{data.Posts:N0}";
        this.NumUsers.Text = $@"{data.Users:N0}";

        var span = DateTime.UtcNow - data.BoardStart;
        double days = span.Days;

        this.BoardStart.Text = this.Get<IDateTimeService>().FormatDateTimeTopic(
            this.PageBoardContext.BoardSettings.UseFarsiCalender
                ? PersianDateConverter.ToPersianDate(data.BoardStart)
                : data.BoardStart);

        this.BoardStartAgo.Text = new DisplayDateTime
                                      {
                                          DateTime = data.BoardStart, Format = DateTimeFormat.BothTopic
                                      }.RenderToString();

        if (days < 1)
        {
            days = 1;
        }

        this.DayPosts.Text = $@"{data.Posts / days:N2}";
        this.DayTopics.Text = $@"{data.Topics / days:N2}";
        this.DayUsers.Text = $@"{data.Users / days:N2}";

        try
        {
            this.DBSize.Text = $@"{this.Get<IDbAccess>().GetDatabaseSize()} MB";
        }
        catch (Exception)
        {
            this.DBSize.Text = this.GetText("ADMIN_ADMIN", "ERROR_DATABASESIZE");
        }

        this.BindActiveUserData();

        this.DataBind();
    }

    /// <summary>
    /// Bind unverified users.
    /// </summary>
    private void BindUnverifiedUsers()
    {
        this.UnverifiedUsersHolder.Visible = !Config.IsDotNetNuke;

        if (!this.UnverifiedUsersHolder.Visible)
        {
            return;
        }

        this.PagerUnverified.PageSize = this.PageSizeUnverified.SelectedValue.ToType<int>();

        var unverifiedUsers = this.GetRepository<User>().GetUnApprovedUsers(this.PageBoardContext.PageBoardID);

        // bind list
        this.UserList.DataSource = unverifiedUsers.GetPaged(this.PagerUnverified);
        this.UserList.DataBind();
    }

    /// <summary>
    /// Gets board ID for which to show statistics.
    /// </summary>
    /// <returns>
    /// Returns ID of selected board (for host admin), ID of current board (for admin), null if all boards is selected.
    /// </returns>
    private int GetSelectedBoardId()
    {
        // check dropdown only if user is host admin
        return !this.PageBoardContext.PageUser.UserFlags.IsHostAdmin
                   ? this.PageBoardContext.PageBoardID
                   : this.BoardStatsSelect.SelectedValue.ToType<int>();
    }
}