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

using System.Text;

using YAF.Types.Extensions.Data;
using YAF.Types.Interfaces.Data;

/// <summary>
/// The Admin Database Maintenance Page
/// </summary>
public partial class ReIndex : AdminPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReIndex"/> class. 
    /// </summary>
    public ReIndex()
        : base("ADMIN_REINDEX", ForumPages.Admin_ReIndex)
    {
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.IsPostBack)
        {
            return;
        }

        this.RadioButtonList1.Items.Add(new ListItem(this.GetText("ADMIN_REINDEX", "RECOVERY1")));
        this.RadioButtonList1.Items.Add(new ListItem(this.GetText("ADMIN_REINDEX", "RECOVERY2")));
        this.RadioButtonList1.Items.Add(new ListItem(this.GetText("ADMIN_REINDEX", "RECOVERY3")));

        this.RadioButtonList1.SelectedIndex = 0;

        this.BindData();
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddRoot();
        this.PageBoardContext.PageLinks.AddAdminIndex();
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_REINDEX", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Gets the stats click.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void GetStatsClick(object sender, EventArgs e)
    {
        try
        {
            this.txtIndexStatistics.Visible = true;
            this.txtIndexStatistics.Text = this.Get<IDbAccess>().GetDatabaseFragmentationInfo();
        }
        catch (Exception ex)
        {
            this.txtIndexStatistics.Text = $@"Failure: {ex}";
        }
    }

    /// <summary>
    /// Sets the Recovery mode
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void RecoveryModeClick(object sender, EventArgs e)
    {
        var recoveryMode = this.RadioButtonList1.SelectedIndex switch
            {
                0 => "FULL",
                1 => "SIMPLE",
                2 => "BULK_LOGGED",
                _ => string.Empty
            };

        const string Result = "Done";

        this.txtIndexStatistics.Text = string.Empty;

        var stats = this.txtIndexStatistics.Text = this.Get<IDbAccess>().ChangeRecoveryMode(recoveryMode);

        this.PageBoardContext.Notify(stats.IsSet() ? stats : Result, MessageTypes.success);
    }

    /// <summary>
    /// Re-indexing the Database
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void ReindexClick(object sender, EventArgs e)
    {
        const string Result = "Done";

        var stats = this.Get<IDbAccess>().ReIndexDatabase(Config.DatabaseObjectQualifier);

        this.txtIndexStatistics.Text = string.Empty;

        this.PageBoardContext.Notify(stats.IsSet() ? stats : Result, MessageTypes.success);
    }

    /// <summary>
    /// Mod By Touradg (herman_herman) 2009/10/19
    /// Shrinking Database
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void ShrinkClick(object sender, EventArgs e)
    {
        try
        {
            var result = new StringBuilder();

            result.Append(this.Get<IDbAccess>().ShrinkDatabase());

            result.Append("&nbsp;");

            result.AppendLine(this.GetTextFormatted(
                "INDEX_SHRINK",
                this.Get<IDbAccess>().GetDatabaseSize()));

            result.Append("&nbsp;");

            this.txtIndexStatistics.Text = string.Empty;

            this.PageBoardContext.Notify(result.ToString(), MessageTypes.success);
        }
        catch (Exception error)
        {
            this.PageBoardContext.Notify(this.GetTextFormatted("INDEX_STATS_FAIL", error.Message), MessageTypes.danger);
        }
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private void BindData()
    {
        this.DataBind();
    }
}