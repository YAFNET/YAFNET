/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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
  using System.Data;
  using System.Web.UI;
  using System.Web.UI.WebControls;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  /// <summary>
  /// Summary description for attachments.
  /// </summary>
  public partial class eventlog : AdminPage
  {
    #region Event Handlers

    /// <summary>
    /// Page load event handler.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load(object sender, EventArgs e)
    {
      // do it only once, not on postbacks
      if (!IsPostBack)
      {
        // create page links
        // board index first
        this.PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));

        // administration index second
        this.PageLinks.AddLink("Administration", YafBuildLink.GetLink(ForumPages.admin_admin));

        // we are now in event log
        this.PageLinks.AddLink("Event Log", string.Empty);

        this.PagerTop.PageSize = 25;

        // bind data to controls
        BindData();
      }
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
    protected void Delete_Load(object sender, EventArgs e)
    {
      ((LinkButton) sender).Attributes["onclick"] = "return confirm('Delete this event log entry?')";
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
    protected void DeleteAll_Load(object sender, EventArgs e)
    {
      ((Button) sender).Attributes["onclick"] = "return confirm('Delete all event log entries?')";
    }


    /// <summary>
    /// Handles delete all button on click event.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void DeleteAll_Click(object sender, EventArgs e)
    {
      // delete all event log entries of this board
      DB.eventlog_delete(PageContext.PageBoardID);

      // re-bind controls
      BindData();
    }

    /// <summary>
    /// Handles single record commands in a repeater.
    /// </summary>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void List_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
      // what command are we serving?
      switch (e.CommandName)
      {
          // delete log entry
        case "delete":

          // delete just this particular log entry
          DB.eventlog_delete(e.CommandArgument);

          // re-bind controls
          BindData();
          break;

          // show/hide log entry details
        case "show":

          // get details control
          Control ctl = e.Item.FindControl("details");

          // find link button control
          var showbutton = e.Item.FindControl("showbutton") as LinkButton;

          // invert visibility
          ctl.Visible = !ctl.Visible;

          // change visibility state of detail and label of linkbutton too
          showbutton.Text = ctl.Visible ? "Hide" : "Show";

          break;
      }
    }

    /// <summary>
    /// The pager top_ page change.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void PagerTop_PageChange(object sender, EventArgs e)
    {
      // rebind
      BindData();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Populates data source and binds data to controls.
    /// </summary>
    private void BindData()
    {
      var pds = new PagedDataSource();
      pds.AllowPaging = true;
      pds.PageSize = this.PagerTop.PageSize;

      // list event for this board
      DataTable dt = DB.eventlog_list(PageContext.PageBoardID);
      DataView dv = dt.DefaultView;

      this.PagerTop.Count = dv.Count;
      pds.DataSource = dv;

      pds.CurrentPageIndex = this.PagerTop.CurrentPageIndex;
      if (pds.CurrentPageIndex >= pds.PageCount)
      {
        pds.CurrentPageIndex = pds.PageCount - 1;
      }

      this.List.DataSource = pds;

      // bind data to controls
      DataBind();
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
    protected string EventImageCode(object dataRow)
    {
      // cast object to the DataRowView
      var row = (DataRowView) dataRow;

      // set defaults
      string imageType = "Error";

      // find out of what type event log entry is
      switch ((int) row["Type"])
      {
        case 1:
          imageType = "Warning";
          break;
        case 2:
          imageType = "Information";
          break;
      }

      // return HTML code of event log entry image
      return String.Format(
        @"<img src=""{0}"" alt=""{1}"" title=""{1}"" />", YafForumInfo.GetURLToResource(String.Format("icons/{0}.png", imageType.ToLower())), imageType);
    }

    #endregion

    #region Web Form Designer generated code

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit(EventArgs e)
    {
      List.ItemCommand += new RepeaterCommandEventHandler(List_ItemCommand);

      // CODEGEN: This call is required by the ASP.NET Web Form Designer.
      InitializeComponent();
      base.OnInit(e);
    }

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
    }

    #endregion
  }
}