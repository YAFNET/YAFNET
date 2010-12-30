/* Yet Another Forum.NET
 * Copyright (C) 2006-2010 Jaben Cargman
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

  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Handlers;
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
      if (!this.PageContext.IsHostAdmin)
      {
        YafBuildLink.AccessDenied();
      }

      if (!this.IsPostBack)
      {
        // Check and see if it should make panels enable or not
        this.PanelReindex.Visible = LegacyDb.PanelReindex;
        this.PanelShrink.Visible = LegacyDb.PanelShrink;
        this.PanelRecoveryMode.Visible = LegacyDb.PanelRecoveryMode;
        this.PanelGetStats.Visible = LegacyDb.PanelGetStats;

        // Get the name of buttons
        this.btnReindex.Text = LegacyDb.btnReindexName;
        this.btnGetStats.Text = LegacyDb.btnGetStatsName;
        this.btnShrink.Text = LegacyDb.btnShrinkName;
        this.btnRecoveryMode.Text = LegacyDb.btnRecoveryModeName;

        this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink(this.GetText("ADMIN_ADMIN", "Administration"), string.Empty);
        this.PageLinks.AddLink("Reindex DB", string.Empty);

        this.BindData();
      }
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
      using (var connMan = new MsSqlDbConnectionManager())
      {
        connMan.InfoMessage += this.connMan_InfoMessage;

        // connMan.DBConnection.FireInfoMessageEventOnUserErrors = true;
        this.txtIndexStatistics.Text = LegacyDb.db_getstats_warning(connMan);
        LegacyDb.db_getstats(connMan);
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
      using (var DBName = new MsSqlDbConnectionManager())
      {
        try
        {
          string dbRecoveryMode = string.Empty;
          if (this.RadioButtonList1.SelectedIndex == 0)
          {
            dbRecoveryMode = "FULL";
          }

          if (this.RadioButtonList1.SelectedIndex == 1)
          {
            dbRecoveryMode = "SIMPLE";
          }

          if (this.RadioButtonList1.SelectedIndex == 2)
          {
            dbRecoveryMode = "BULK_LOGGED";
          }

          DBName.InfoMessage += this.connMan_InfoMessage;
          this.txtIndexStatistics.Text = LegacyDb.db_recovery_mode_warning(DBName);
          LegacyDb.db_recovery_mode(DBName, dbRecoveryMode);
          this.txtIndexStatistics.Text = "Database recovery mode was successfuly set to " + dbRecoveryMode;
        }
        catch (Exception error)
        {
          this.txtIndexStatistics.Text = "Something went wrong with this operation.The reported error is: " +
                                         error.Message;
        }
      }
    }

    // Reindexing Database
    /// <summary>
    /// The btn reindex_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void btnReindex_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      using (var connMan = new MsSqlDbConnectionManager())
      {
        connMan.InfoMessage += this.connMan_InfoMessage;
        this.txtIndexStatistics.Text = LegacyDb.db_reindex_warning(connMan);
        LegacyDb.db_reindex(connMan);
      }
    }

    // Mod By Touradg (herman_herman) 2009/10/19
    // Shrinking Database
    /// <summary>
    /// The btn shrink_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void btnShrink_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      using (var DBName = new MsSqlDbConnectionManager())
      {
        try
        {
          DBName.InfoMessage += this.connMan_InfoMessage;
          this.txtIndexStatistics.Text = LegacyDb.db_shrink_warning(DBName);
          LegacyDb.db_shrink(DBName);
          this.txtIndexStatistics.Text = "Shrink operation was Successful.Your database size is now: " + LegacyDb.DBSize +
                                         "MB";
        }
        catch (Exception error)
        {
          this.txtIndexStatistics.Text = "Something went wrong with operation.The reported error is: " + error.Message;
        }
      }
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      this.DataBind();
    }

    // Set Database Recovery Mode

    // End of MOD
    /// <summary>
    /// The conn man_ info message.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void connMan_InfoMessage([NotNull] object sender, [NotNull] YafDBConnInfoMessageEventArgs e)
    {
      this.txtIndexStatistics.Text = e.Message;
    }

    #endregion
  }
}