/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2016 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

using System.Drawing;

namespace YAF.Pages.Admin
{
  #region Using

  using System;
  using System.Web.UI.WebControls;

  using YAF.Controls;
  using YAF.Core;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Extensions;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// Summary description for prune.
  /// </summary>
  public partial class taskmanager : AdminPage
  {
    #region Methods
      /// <summary>
      /// Format access mask setting color formatting.
      /// </summary>
      /// <param name="enabled">
      /// The enabled.
      /// </param>
      /// <returns>
      /// Set access mask flags are rendered green, rest black.
      /// </returns>
      protected Color GetItemColor(bool enabled)
      {
          // show enabled flag red
          return enabled ? Color.Green : Color.Black;
      }

      /// <summary>
      /// Get a user friendly item name.
      /// </summary>
      /// <param name="enabled">
      /// The enabled.
      /// </param>
      /// <returns>
      /// Item Name.
      /// </returns>
      protected string GetItemName(bool enabled)
      {
          return enabled ? this.GetText("DEFAULT", "YES") : this.GetText("DEFAULT", "NO");
      }
    /// <summary>
    /// binds data for this control
    /// </summary>
    protected void BindData()
    {
        this.lblTaskCount.Text = this.GetText("ADMIN_TASKMANAGER", "HEADER").FormatWith(this.Get<ITaskModuleManager>().TaskCount.ToString());

      this.taskRepeater.DataSource = this.Get<ITaskModuleManager>().TaskManagerSnapshot;
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

      return "{0:D2}:{1:D2}:{2:D2}:{3:D2}".FormatWith(elapsed.Days, elapsed.Hours, elapsed.Minutes, elapsed.Seconds);
    }

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
      if (!this.IsPostBack)
      {
        this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink(this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));

        this.PageLinks.AddLink(this.GetText("ADMIN_TASKMANAGER", "TITLE"), string.Empty);

        this.Page.Header.Title = "{0} - {1}".FormatWith(
            this.GetText("ADMIN_ADMIN", "Administration"),
            this.GetText("ADMIN_TASKMANAGER", "TITLE"));

      }

      this.BindData();
    }

    /// <summary>
    /// Called on a command in the task repeater.
    /// </summary>
    /// <param name="source">
    /// </param>
    /// <param name="e">
    /// </param>
    protected void taskRepeater_ItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
    {
        if (!e.CommandName.Equals("stop"))
        {
            return;
        }

        // attempt to stop a task...
        this.Get<ITaskModuleManager>().StopTask(e.CommandArgument.ToString());

        // refresh the display
        this.BindData();
    }

    #endregion
  }
}