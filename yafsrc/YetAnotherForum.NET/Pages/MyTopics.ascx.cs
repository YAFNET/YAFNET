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
namespace YAF.Pages;

using YAF.Web.Controls;
using YAF.Types.Models;

/// <summary>
/// The my topics page.
/// </summary>
public partial class MyTopics : ForumPageRegistered
{
    /// <summary>
    ///   default since date is now
    /// </summary>
    private DateTime sinceDate = DateTime.UtcNow;

    /// <summary>
    ///   default since option is "since last visit"
    /// </summary>
    private int sinceValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="MyTopics"/> class.
    /// </summary>
    public MyTopics()
        : base("MYTOPICS", ForumPages.MyTopics)
    {
    }

    /// <summary>
    /// The On PreRender event.
    /// </summary>
    /// <param name="e">
    /// the Event Arguments
    /// </param>
    protected override void OnPreRender([NotNull] EventArgs e)
    {
        this.PageBoardContext.PageElements.RegisterJsBlockStartup(
            "TopicStarterPopoverJs",
            JavaScriptBlocks.TopicLinkPopoverJs(
                $"{this.GetText("TOPIC_STARTER")}&nbsp;...",
                ".topic-starter-popover",
                "hover"));

        this.PageBoardContext.PageElements.RegisterJsBlockStartup(
            "TopicLinkPopoverJs",
            JavaScriptBlocks.TopicLinkPopoverJs(
                $"{this.GetText("LASTPOST")}&nbsp;{this.GetText("SEARCH", "BY")} ...",
                ".topic-link-popover",
                "focus hover"));

        var iconLegend = new IconLegend().RenderToString();

        this.PageBoardContext.PageElements.RegisterJsBlockStartup(
            "TopicIconLegendPopoverJs",
            JavaScriptBlocks.ForumIconLegendPopoverJs(
                iconLegend.ToJsString(),
                "topic-icon-legend-popvover"));

        base.OnPreRender(e);
    }

    /// <summary>
    /// The Page_ Load Event.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
        if (!this.IsPostBack)
        {
            this.LoadControls();
            this.BindData();
        }
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
    /// Handles the PageChange event of the Pager control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Pager_PageChange([NotNull] object sender, [NotNull] EventArgs e)
    {
        this.BindData();
    }

    /// <summary>
    /// Reloads the Topic Last Based on the Selected Since Value
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Since_SelectedIndexChanged([NotNull] object sender, [NotNull] EventArgs e)
    {
        // Set the controls' pager index to 0.
        this.PagerTop.CurrentPageIndex = 0;

        // re-bind data
        this.BindData();
    }

    protected void TopicModeSelectedIndexChanged([NotNull] object sender, [NotNull] EventArgs e)
    {
        // Set the controls' pager index to 0.
        this.PagerTop.CurrentPageIndex = 0;

        // re-bind data
        this.BindData();
    }


    /// <summary>
    /// The create topic line.
    /// </summary>
    /// <param name="item">
    /// The item.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    protected string CreateTopicLine(object item)
    {
        var topicLine = new TopicContainer { Item = item as PagedTopic };

        return topicLine.RenderToString();
    }

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddRoot();

        this.PageBoardContext.PageLinks.AddLink(this.GetText("MEMBERTITLE"), string.Empty);
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    public void BindData()
    {
        // we'll hold topics in this table
        List<PagedTopic> topicList = null;

        var basePageSize = this.PageSize.SelectedValue.ToType<int>();

        this.PagerTop.PageSize = basePageSize;

        // page index in db which is returned back  is +1 based!
        var currentPageIndex = this.PagerTop.CurrentPageIndex;


        // default since date is now
        this.sinceDate = DateTime.UtcNow;

        // default since option is "since last visit"
        this.sinceValue = 0;

        // is any "since"option selected
        if (this.Since.SelectedItem != null)
        {
            // get selected value
            this.sinceValue = int.Parse(this.Since.SelectedItem.Value);

            this.sinceDate = this.sinceValue switch
                {
                    // decrypt selected option
                    9999 => DateTimeHelper.SqlDbMinTime(),
                    > 0 => DateTime.UtcNow - TimeSpan.FromDays(this.sinceValue),
                    < 0 => DateTime.UtcNow + TimeSpan.FromHours(this.sinceValue),
                    _ => this.sinceDate
                };
        }

        // we want to filter topics since last visit
        if (this.sinceValue == 0)
        {
            this.sinceDate = this.Get<ISession>().LastVisit ?? DateTime.UtcNow;
        }

        switch (this.TopicMode.SelectedValue.ToEnum<TopicListMode>())
        {
            case TopicListMode.Active:
                this.IconHeader.LocalizedTag = "ActiveTopics";

                topicList = this.GetRepository<Topic>().ListActivePaged(
                    this.PageBoardContext.PageUserID,
                    this.sinceDate,
                    DateTime.UtcNow,
                    currentPageIndex,
                    basePageSize,
                    this.PageBoardContext.BoardSettings.UseReadTrackingByDatabase);
                break;
            case TopicListMode.Unanswered:
                this.IconHeader.LocalizedTag = "UnansweredTopics";

                topicList = this.GetRepository<Topic>().ListUnansweredPaged(
                    this.PageBoardContext.PageUserID,
                    this.sinceDate,
                    DateTime.UtcNow,
                    currentPageIndex,
                    basePageSize,
                    this.PageBoardContext.BoardSettings.UseReadTrackingByDatabase);
                break;
            case TopicListMode.Watch:
                this.IconHeader.LocalizedTag = "WatchTopics";

                topicList = this.GetRepository<Topic>().ListWatchedPaged(
                    this.PageBoardContext.PageUserID,
                    this.sinceDate,
                    DateTime.UtcNow,
                    currentPageIndex,
                    basePageSize,
                    this.PageBoardContext.BoardSettings.UseReadTrackingByDatabase);
                break;
            case TopicListMode.User:
                this.IconHeader.LocalizedTag = "MyTopics";

                topicList = this.GetRepository<Topic>().ListByUserPaged(
                    this.PageBoardContext.PageUserID,
                    this.sinceDate,
                    DateTime.UtcNow,
                    currentPageIndex,
                    basePageSize,
                    this.PageBoardContext.BoardSettings.UseReadTrackingByDatabase);
                break;
        }

        if (topicList is null)
        {
            this.PagerTop.Count = 0;
            return;
        }

        if (!topicList.Any())
        {
            this.PagerTop.Count = 0;
            this.TopicList.DataSource = null;
            this.TopicList.DataBind();
            return;
        }

        // let's page the results
        this.PagerTop.Count = topicList.FirstOrDefault().TotalRows;

        this.TopicList.DataSource = topicList;
        this.TopicList.DataBind();

        this.DataBind();
    }

    /// <summary>
    /// Mark all Topics in the List as Read
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void MarkAll_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
        this.BindData();

        if (this.TopicList.Items.Count <= 0)
        {
            return;
        }

        this.TopicList.DataSource.ToType<List<PagedTopic>>()
            .ForEach(
                item => this.Get<IReadTrackCurrentUser>().SetTopicRead(item.TopicID));

        // Rebind
        this.BindData();
    }

    /// <summary>
    /// The load and bind controls.
    /// </summary>
    private void LoadControls()
    {
        this.PageSize.DataSource = StaticDataHelper.PageEntries();
        this.PageSize.DataTextField = "Name";
        this.PageSize.DataValueField = "Value";
        this.PageSize.DataBind();

        try
        {
            this.PageSize.SelectedValue = this.PageBoardContext.PageUser.PageSize.ToString();
        }
        catch (Exception)
        {
            this.PageSize.SelectedValue = "5";
        }

        // Load Topic Mode
        this.TopicMode.DataSource = StaticDataHelper.TopicListModes();
        this.TopicMode.DataValueField = "Value";
        this.TopicMode.DataTextField = "Name";
        this.TopicMode.DataBind();

        this.InitSinceDropdown();
    }

    /// <summary>
    /// Initializes dropdown with options to filter results by date.
    /// </summary>
    private void InitSinceDropdown()
    {
        var lastVisit = this.Get<ISession>().LastVisit;

        // value 0, for since last visit
        this.Since.Items.Add(
            new ListItem(
                this.GetTextFormatted(
                    "last_visit",
                    this.Get<IDateTimeService>().FormatDateTime(
                        lastVisit.HasValue && lastVisit.Value != DateTimeHelper.SqlDbMinTime()
                            ? lastVisit.Value
                            : DateTime.UtcNow)),
                "0"));

        // negative values for hours backward
        this.Since.Items.Add(new ListItem(this.GetText("last_hour"), "-1"));
        this.Since.Items.Add(new ListItem(this.GetText("last_two_hours"), "-2"));
        this.Since.Items.Add(new ListItem(this.GetText("last_eight_hours"), "-8"));

        // positive values for days backward
        this.Since.Items.Add(new ListItem(this.GetText("last_day"), "1"));
        this.Since.Items.Add(new ListItem(this.GetText("last_two_days"), "2"));
        this.Since.Items.Add(new ListItem(this.GetText("last_week"), "7"));
        this.Since.Items.Add(new ListItem(this.GetText("last_two_weeks"), "14"));
        this.Since.Items.Add(new ListItem(this.GetText("last_month"), "31"));
            
        this.Since.Items.Add(new ListItem(this.GetText("show_all"), "9999"));
    }
}