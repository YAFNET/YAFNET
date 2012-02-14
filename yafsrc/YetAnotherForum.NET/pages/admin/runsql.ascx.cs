/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
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

  using YAF.Classes;
  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Handlers;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// The runsql.
  /// </summary>
  public partial class runsql : AdminPage
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

        this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));

       this.PageLinks.AddLink(this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));
        this.PageLinks.AddLink(this.GetText("ADMIN_RUNSQL", "TITLE"), string.Empty);

        this.Page.Header.Title = "{0} - {1}".FormatWith(
            this.GetText("ADMIN_ADMIN", "Administration"),
            this.GetText("ADMIN_RUNSQL", "TITLE"));

        this.chkRunInTransaction.Text = this.GetText("ADMIN_RUNSQL", "RUN_TRANSCATION");
        this.btnRunQuery.Text = this.GetText("ADMIN_RUNSQL", "RUN_QUERY");

        this.BindData();
    }

    /// <summary>
    /// The btn run query_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void btnRunQuery_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      this.txtResult.Text = string.Empty;
      this.ResultHolder.Visible = true;

    /*  using (var connMan = new MsSqlDbConnectionManager())
      {
        connMan.InfoMessage += this.connMan_InfoMessage;
        string sql = this.txtQuery.Text.Trim();

        // connMan.DBConnection.FireInfoMessageEventOnUserErrors = true;
        sql = sql.Replace("{databaseOwner}", Config.DatabaseOwner);
        sql = sql.Replace("{objectQualifier}", Config.DatabaseObjectQualifier);

        this.txtResult.Text = LegacyDb.db_runsql(sql, connMan, this.chkRunInTransaction.Checked);
      } */
      this.txtResult.Text = LegacyDb.db_runsql_new(this.txtQuery.Text.Trim(), this.chkRunInTransaction.Checked);
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      this.DataBind();
    }

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
      this.txtResult.Text = "\r\n" + e.Message;
    }

    #endregion
  }
}