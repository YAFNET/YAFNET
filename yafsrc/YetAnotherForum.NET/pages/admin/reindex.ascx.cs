/* Yet Another Forum.NET
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

using System.Collections.Generic;
using YAF.Types.Interfaces.Data;

namespace YAF.Pages.Admin
{
  #region Using

  using System;
  using System.Web.UI.WebControls;

  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Core.Helpers;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Extensions;
  using YAF.Types.Handlers;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// The reindex.
  /// </summary>
  public partial class reindex : AdminPage
  {
    #region Methods

    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
        if (this.IsPostBack)
        {
            return;
        }

        // Check and see if it should make panels enable or not
        this.PanelReindex.Visible = LegacyDb.PanelReindex;
        this.PanelShrink.Visible = LegacyDb.PanelShrink;
        this.PanelRecoveryMode.Visible = LegacyDb.PanelRecoveryMode;
        this.PanelGetStats.Visible = LegacyDb.PanelGetStats;

        // Get the name of buttons
        this.btnReindex.Text = this.GetText("ADMIN_REINDEX", "REINDEXTBL_BTN");
        this.btnGetStats.Text = this.GetText("ADMIN_REINDEX", "TBLINDEXSTATS_BTN");
        this.btnShrink.Text = this.GetText("ADMIN_REINDEX", "SHRINK_BTN");
        this.btnRecoveryMode.Text = this.GetText("ADMIN_REINDEX", "SETRECOVERY_BTN");

        this.btnShrink.OnClientClick =
            "return confirm('{0}');".FormatWith(this.GetText("ADMIN_REINDEX", "CONFIRM_SHRINK"));
        
        this.btnReindex.OnClientClick =
            "return confirm('{0}');".FormatWith(this.GetText("ADMIN_REINDEX", "CONFIRM_REINDEX"));
        
        this.btnRecoveryMode.OnClientClick =
            "return confirm('{0}');".FormatWith(this.GetText("ADMIN_REINDEX", "CONFIRM_RECOVERY"));

        this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink(this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));
        this.PageLinks.AddLink(this.GetText("ADMIN_REINDEX", "TITLE"), string.Empty);

        this.Page.Header.Title = "{0} - {1}".FormatWith(
              this.GetText("ADMIN_ADMIN", "Administration"),
              this.GetText("ADMIN_REINDEX", "TITLE"));

        this.RadioButtonList1.Items.Add(new ListItem(this.GetText("ADMIN_REINDEX", "RECOVERY1")));
        this.RadioButtonList1.Items.Add(new ListItem(this.GetText("ADMIN_REINDEX", "RECOVERY2")));
        this.RadioButtonList1.Items.Add(new ListItem(this.GetText("ADMIN_REINDEX", "RECOVERY3")));

        this.RadioButtonList1.SelectedIndex = 0;

        this.BindData();
    }

    /// <summary>
    /// The btn get stats_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void btnGetStats_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
        try
        {
            this.txtIndexStatistics.Text = (string)this.Get<IDbFunction>().Query.getstats();
        }
        catch (Exception ex)
        {
            this.txtIndexStatistics.Text = string.Format("Failure: {0}", ex);
        }
    }

    /// <summary>
    /// The btn recovery mode_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void btnRecoveryMode_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
        string dbRecoveryMode = string.Empty;
        switch (this.RadioButtonList1.SelectedIndex)
        {
            case 0: dbRecoveryMode = "FULL"; break;
            case 1: dbRecoveryMode = "SIMPLE"; break;
            case 2: dbRecoveryMode = "BULK_LOGGED"; break;
        }
        string error = LegacyDb.db_recovery_mode_new(dbRecoveryMode);
        if (error.IsSet())
        {
            this.txtIndexStatistics.Text = LegacyDb.db_recovery_mode_warning() + this.GetText("ADMIN_REINDEX", "INDEX_STATS_FAIL").FormatWith(error);
        }
        else
        {
            this.txtIndexStatistics.Text = this.GetText("ADMIN_REINDEX", "INDEX_STATS").FormatWith(dbRecoveryMode);
            this.txtIndexStatistics.Text = LegacyDb.db_recovery_mode_warning() + "\r\n{0}".FormatWith(LegacyDb.db_recovery_mode_new(dbRecoveryMode));
        }
    }

    /// <summary>
    /// Reindexing Database
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void btnReindex_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
     /* using (var connMan = new MsSqlDbConnectionManager())
      {
        connMan.InfoMessage += this.connMan_InfoMessage;
        this.txtIndexStatistics.Text = LegacyDb.db_reindex_warning();
        LegacyDb.db_reindex(connMan);
      } */

      this.txtIndexStatistics.Text = LegacyDb.db_reindex_warning() + LegacyDb.db_reindex_new();
    }

    /// <summary>
    /// Mod By Touradg (herman_herman) 2009/10/19
    /// Shrinking Database
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void btnShrink_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
        try
        {
            this.txtIndexStatistics.Text = LegacyDb.db_shrink_warning() + @"\r\n\{0}\r\n\".FormatWith(LegacyDb.db_shrink_new());
            this.txtIndexStatistics.Text =
                this.GetText("ADMIN_REINDEX", "INDEX_SHRINK").FormatWith(this.Get<IDbFunction>().GetDBSize());
        }
        catch (Exception error)
        {
            this.txtIndexStatistics.Text += this.GetText("ADMIN_REINDEX", "INDEX_STATS_FAIL").FormatWith(error.Message);
        }
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      this.DataBind();
    }

    #endregion
  }
}