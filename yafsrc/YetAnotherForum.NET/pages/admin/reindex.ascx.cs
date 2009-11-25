/* Yet Another Forum.NET
 * Copyright (C) 2006-2009 Jaben Cargman
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
  using System;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  /// <summary>
  /// The reindex.
  /// </summary>
  public partial class reindex : AdminPage
  {
    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load(object sender, EventArgs e)
    {
      if (!PageContext.IsHostAdmin)
      {
        YafBuildLink.AccessDenied();
      }

      if (!IsPostBack)
      {
        // Check and see if it should make panels enable or not
        this.PanelReindex.Visible = DB.PanelReindex;
        this.PanelShrink.Visible = DB.PanelShrink;
        this.PanelRecoveryMode.Visible = DB.PanelRecoveryMode;
        this.PanelGetStats.Visible = DB.PanelGetStats;

        // Get the name of buttons
        this.btnReindex.Text = DB.btnReindexName;
        this.btnGetStats.Text = DB.btnGetStatsName;
        this.btnShrink.Text = DB.btnShrinkName;
        this.btnRecoveryMode.Text = DB.btnRecoveryModeName;

        this.PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink("Administration", YafBuildLink.GetLink(ForumPages.admin_admin));
        this.PageLinks.AddLink("Reindex DB", string.Empty);

        BindData();
      }
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      DataBind();
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
    protected void btnGetStats_Click(object sender, EventArgs e)
    {
      using (var connMan = new YafDBConnManager())
      {
        connMan.InfoMessage += connMan_InfoMessage;

        // connMan.DBConnection.FireInfoMessageEventOnUserErrors = true;
        this.txtIndexStatistics.Text = DB.db_getstats_warning(connMan);
        DB.db_getstats(connMan);
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
    protected void btnReindex_Click(object sender, EventArgs e)
    {
      using (var connMan = new YafDBConnManager())
      {
        connMan.InfoMessage += connMan_InfoMessage;
        this.txtIndexStatistics.Text = DB.db_reindex_warning(connMan);
        DB.db_reindex(connMan);
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
    protected void btnShrink_Click(object sender, EventArgs e)
    {
      using (var DBName = new YafDBConnManager())
      {
        try
        {
          DBName.InfoMessage += connMan_InfoMessage;
          this.txtIndexStatistics.Text = DB.db_shrink_warning(DBName);
          DB.db_shrink(DBName);
          this.txtIndexStatistics.Text = "Shrink operation was Successful.Your database size is now: " + DB.DBSize + "MB";
        }
        catch (Exception error)
        {
          this.txtIndexStatistics.Text = "Something went wrong with operation.The reported error is: " + error.Message;
        }
      }
    }

    // Set Database Recovery Mode
    /// <summary>
    /// The btn recovery mode_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void btnRecoveryMode_Click(object sender, EventArgs e)
    {
      using (var DBName = new YafDBConnManager())
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

          DBName.InfoMessage += connMan_InfoMessage;
          this.txtIndexStatistics.Text = DB.db_recovery_mode_warning(DBName);
          DB.db_recovery_mode(DBName, dbRecoveryMode);
          this.txtIndexStatistics.Text = "Database recovery mode was successfuly set to " + dbRecoveryMode;
        }
        catch (Exception error)
        {
          this.txtIndexStatistics.Text = "Something went wrong with this operation.The reported error is: " + error.Message;
        }
      }
    }

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
    private void connMan_InfoMessage(object sender, YafDBConnInfoMessageEventArgs e)
    {
      this.txtIndexStatistics.Text = e.Message;
    }
  }
}