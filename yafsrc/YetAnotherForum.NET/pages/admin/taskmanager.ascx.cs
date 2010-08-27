/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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
  using System;
  using System.Collections.Generic;
  using System.Web.UI.WebControls;

  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Utils;

  /// <summary>
  /// Summary description for prune.
  /// </summary>
  public partial class taskmanager : AdminPage
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
        this.PageLinks.AddLink("Task Manager", string.Empty);
      }

      this.BindData();
    }

    /// <summary>
    /// binds data for this control
    /// </summary>
    protected void BindData()
    {
      this.lblTaskCount.Text = YafTaskModule.Current.TaskCount.ToString();
      this.taskRepeater.DataSource = YafTaskModule.Current.TaskManagerSnapshot;
      this.taskRepeater.DataBind();      
    }

    /// <summary>
    /// The format time span.
    /// </summary>
    /// <param name="time">
    /// The time item.
    /// </param>
    /// <returns>
    /// The format time span.
    /// </returns>
    protected string FormatTimeSpan(DateTime time)
    {
      TimeSpan elapsed = DateTime.UtcNow.Subtract(time);

      return String.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}", elapsed.Days, elapsed.Hours, elapsed.Minutes, elapsed.Seconds);
    }

    /// <summary>
    /// Called on a command in the task repeater.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    protected void taskRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
      if (e.CommandName.Equals("stop"))
      {
        // attempt to stop a task...
        YafTaskModule.Current.StopTask(e.CommandArgument.ToString());

        // refresh the display
        this.BindData();
      }
    }
  }
}