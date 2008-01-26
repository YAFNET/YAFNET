/* Yet Another Forum.NET
 * Copyright (C) 2006-2008 Jaben Cargman
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
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using YAF.Classes.Utils;

namespace YAF.Controls
{
	public partial class EditUsersSuspend : YAF.Classes.Base.BaseUserControl
	{
		#region Event Handlers

		/// <summary>
		/// Handles page load event.
		/// </summary>
		protected void Page_Load(object sender, EventArgs e)
		{
			// this needs to be done just once, not during postbacks
			if (!IsPostBack)
			{
				// add items to the dropdown
				SuspendUnit.Items.Add(new ListItem(PageContext.Localization.GetText("PROFILE", "DAYS"), "1"));
				SuspendUnit.Items.Add(new ListItem(PageContext.Localization.GetText("PROFILE", "HOURS"), "2"));
				SuspendUnit.Items.Add(new ListItem(PageContext.Localization.GetText("PROFILE", "MINUTES"), "3"));
				// select hours
				SuspendUnit.SelectedIndex = 1;
				// default number of hours to suspend user for
				SuspendCount.Text = "2";

				// bind data
				BindData();
			}
		}


		/// <summary>
		/// Suspends a user when clicked.
		/// </summary>
		/// <param name="sender">The object sender inherit from Page.</param>
		/// <param name="e">The System.EventArgs inherit from Page.</param>
		protected void Suspend_Click(object sender, System.EventArgs e)
		{
			// Admins can suspend anyone not admins
			// Forum Moderators can suspend anyone not admin or forum moderator
			using (DataTable dt = YAF.Classes.Data.DB.user_list(PageContext.PageBoardID, Request.QueryString["u"], null))
			{
				foreach (DataRow row in dt.Rows)
				{
					// is user to be suspended admin?
					if (int.Parse(row["IsAdmin"].ToString()) > 0)
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
				}
			}

			// time until when user is suspended
			DateTime suspend = DateTime.Now;
			// number inserted by suspending user
			int count = int.Parse(SuspendCount.Text);

			// what time units are used for suspending
			switch (SuspendUnit.SelectedValue)
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
			YAF.Classes.Data.DB.user_suspend(Request.QueryString["u"], suspend);
			// re-bind data
			BindData();
		}


		/// <summary>
		/// Removes suspension from a user.
		/// </summary>
		protected void RemoveSuspension_Click(object sender, System.EventArgs e)
		{
			// un-suspend user
			YAF.Classes.Data.DB.user_suspend(Request.QueryString["u"], null);
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
			using (DataTable dt = YAF.Classes.Data.DB.user_list(PageContext.PageBoardID, Request.QueryString["u"], true))
			{
				// there is no such user
				if (dt.Rows.Count < 1) YafBuildLink.AccessDenied(/*No such user exists*/);

				// get user's data in form of data row
				DataRow user = dt.Rows[0];

				// if user is not suspended, hide row with suspend information and remove suspension button
				SuspendedRow.Visible = !user.IsNull("Suspended");

				// is user suspended?
				if (!user.IsNull("Suspended"))
				{
					// get time when his suspension expires to the view state
					ViewState["SuspendedUntil"] = YafDateTime.FormatDateTime(user["Suspended"]);

					// localize remove suspension button
					RemoveSuspension.Text = PageContext.Localization.GetText("PROFILE", "REMOVESUSPENSION");
				}

				// localize suspend button
				Suspend.Text = PageContext.Localization.GetText("PROFILE", "SUSPEND");
			}
		}


		/// <summary>
		/// Gets the time until user is suspended.
		/// </summary>
		/// <returns>Date and time until when user is suspended. Empty string when user is not suspended.</returns>
		protected string GetSuspendedTo()
		{
			// is there suspension expiration in the viewstate?
			if (ViewState["SuspendedUntil"] != null)
			{
				// return it
				return (string)ViewState["SuspendedUntil"];
			}
			else
			{
				// return empty string
				return "";
			}
		}

		#endregion
	}
}