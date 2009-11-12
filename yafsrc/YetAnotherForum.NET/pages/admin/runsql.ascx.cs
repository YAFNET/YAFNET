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
  /// The runsql.
  /// </summary>
  public partial class runsql : AdminPage
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
      if (!IsPostBack)
      {
        this.PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink("Administration", YafBuildLink.GetLink(ForumPages.admin_admin));
        this.PageLinks.AddLink("Run SQL Code", string.Empty);

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
    /// The btn run query_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void btnRunQuery_Click(object sender, EventArgs e)
    {
      this.txtResult.Text = string.Empty;
      this.ResultHolder.Visible = true;

      using (var connMan = new YafDBConnManager())
      {
        connMan.InfoMessage += connMan_InfoMessage;
        string sql = this.txtQuery.Text.Trim();

        // connMan.DBConnection.FireInfoMessageEventOnUserErrors = true;
        sql = sql.Replace("{databaseOwner}", YafDBAccess.DatabaseOwner);
        sql = sql.Replace("{objectQualifier}", YafDBAccess.ObjectQualifier);
        this.txtResult.Text = DB.db_runsql(sql, connMan);
      }
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
    private void connMan_InfoMessage(object sender, YafDBConnManager.YafDBConnInfoMessageEventArgs e)
    {
      this.txtResult.Text = "\r\n" + e.Message;
    }
  }
}