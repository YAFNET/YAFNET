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
namespace YAF.Controls
{
  using System;
  using System.Data;
  using System.Web.UI.WebControls;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  /// <summary>
  /// The edit users suspend.
  /// </summary>
  public partial class EditUsersSuspend : BaseUserControl
  {
    /// <summary>
    /// Gets CurrentUserID.
    /// </summary>
    protected long? CurrentUserID
    {
      get
      {
        return PageContext.QueryIDs["u"];
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether ShowHeader.
    /// </summary>
    public bool ShowHeader
    {
      get
      {
        if (ViewState["ShowHeader"] != null)
        {
          return Convert.ToBoolean(ViewState["ShowHeader"]);
        }

        return true;
      }

      set
      {
        ViewState["ShowHeader"] = value;
      }
    }

    #region Event Handlers

    /// <summary>
    /// Handles page load event.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load(object sender, EventArgs e)
    {
      // init ids...
      PageContext.QueryIDs = new QueryStringIDHelper("u", true);

      // this needs to be done just once, not during postbacks
      if (!IsPostBack)
      {
        // add items to the dropdown
        this.SuspendUnit.Items.Add(new ListItem(PageContext.Localization.GetText("PROFILE", "DAYS"), "1"));
        this.SuspendUnit.Items.Add(new ListItem(PageContext.Localization.GetText("PROFILE", "HOURS"), "2"));
        this.SuspendUnit.Items.Add(new ListItem(PageContext.Localization.GetText("PROFILE", "MINUTES"), "3"));

        // select hours
        this.SuspendUnit.SelectedIndex = 1;

        // default number of hours to suspend user for
        this.SuspendCount.Text = "2";

        // bind data
        BindData();
      }
    }

    /// <summary>
    /// The page_ pre render.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_PreRender(object sender, EventArgs e)
    {
      this.trHeader.Visible = ShowHeader;
    }

    /// <summary>
    /// Suspends a user when clicked.
    /// </summary>
    /// <param name="sender">
    /// The object sender inherit from Page.
    /// </param>
    /// <param name="e">
    /// The System.EventArgs inherit from Page.
    /// </param>
    protected void Suspend_Click(object sender, EventArgs e)
    {
      // Admins can suspend anyone not admins
      // Forum Moderators can suspend anyone not admin or forum moderator
      using (DataTable dt = DB.user_list(PageContext.PageBoardID, CurrentUserID, null))
      {
        foreach (DataRow row in dt.Rows)
        {
          // is user to be suspended admin?
          if (row["IsAdmin"] != DBNull.Value && Convert.ToInt32(row["IsAdmin"]) > 0)
          {
            // tell user he can't suspend admin
            PageContext.AddLoadMessage(PageContext.Localization.GetText("PROFILE", "ERROR_ADMINISTRATORS"));
            return;
          }

          // is user to be suspended forum moderator, while user suspending him is not admin?
          if (!PageContext.IsAdmin && int.Parse(row["IsForumModerator"].ToString()) > 0)
          {
            // tell user he can't suspend forum moderator when he's not admin
            PageContext.AddLoadMessage(PageContext.Localization.GetText("PROFILE", "ERROR_FORUMMODERATORS"));
            return;
          }

          object isGuest = row["IsGuest"];

          // verify the user isn't guest...
          if (isGuest != DBNull.Value && Convert.ToInt32(isGuest) > 0)
          {
            PageContext.AddLoadMessage(PageContext.Localization.GetText("PROFILE", "ERROR_GUESTACCOUNT"));
            return;
          }
        }
      }

      // time until when user is suspended
      DateTime suspend = DateTime.Now;

      // number inserted by suspending user
      int count = int.Parse(this.SuspendCount.Text);

      // what time units are used for suspending
      switch (this.SuspendUnit.SelectedValue)
      {
          // days
        case "1":

          // add user inserted suspension time to current time
          suspend += new TimeSpan(count, 0, 0, 0);
          break;

          // hours
        case "2":

          // add user inserted suspension time to current time
          suspend += new TimeSpan(0, count, 0, 0);
          break;

          // minutes
        case "3":

          // add user inserted suspension time to current time
          suspend += new TimeSpan(0, 0, count, 0);
          break;
      }

      // suspend user by calling appropriate method
      DB.user_suspend(CurrentUserID, suspend);

      // re-bind data
      BindData();
    }


    /// <summary>
    /// Removes suspension from a user.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void RemoveSuspension_Click(object sender, EventArgs e)
    {
      // un-suspend user
      DB.user_suspend(CurrentUserID, null);

      // re-bind data
      BindData();
    }

    #endregion

    #region Data Binding & Formatting

    /// <summary>
    /// Bind data for this control.
    /// </summary>
    private void BindData()
    {
      // get user's info
      using (DataTable dt = DB.user_list(PageContext.PageBoardID, CurrentUserID, null))
      {
        // there is no such user
        if (dt.Rows.Count < 1)
        {
          YafBuildLink.AccessDenied( /*No such user exists*/);
        }

        // get user's data in form of data row
        DataRow user = dt.Rows[0];

        // if user is not suspended, hide row with suspend information and remove suspension button
        this.SuspendedRow.Visible = !user.IsNull("Suspended");

        // is user suspended?
        if (!user.IsNull("Suspended"))
        {
          // get time when his suspension expires to the view state
          ViewState["SuspendedUntil"] = YafServices.DateTime.FormatDateTime(user["Suspended"]);

          // localize remove suspension button
          this.RemoveSuspension.Text = PageContext.Localization.GetText("PROFILE", "REMOVESUSPENSION");
        }

        // localize suspend button
        this.Suspend.Text = PageContext.Localization.GetText("PROFILE", "SUSPEND");
      }
    }


    /// <summary>
    /// Gets the time until user is suspended.
    /// </summary>
    /// <returns>
    /// Date and time until when user is suspended. Empty string when user is not suspended.
    /// </returns>
    protected string GetSuspendedTo()
    {
      // is there suspension expiration in the viewstate?
      if (ViewState["SuspendedUntil"] != null)
      {
        // return it
        return (string) ViewState["SuspendedUntil"];
      }
      else
      {
        // return empty string
        return string.Empty;
      }
    }

    #endregion
  }
}