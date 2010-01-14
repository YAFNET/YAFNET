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
using System;
using YAF.Classes.Utils;

namespace YAF.Controls
{
	public partial class EditUsersPoints : YAF.Classes.Core.BaseUserControl
	{
		/// <summary>
		/// Gets user ID of edited user.
		/// </summary>
		protected int CurrentUserID
		{
			get
			{
				return (int)this.PageContext.QueryIDs["u"];
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			PageContext.QueryIDs = new QueryStringIDHelper("u", true);

			if (!IsPostBack)
			{
				BindData();
			}
		}

		private void BindData()
		{
			ltrCurrentPoints.Text = YAF.Classes.Data.DB.user_getpoints(CurrentUserID).ToString();
		}

		protected void AddPoints_Click(object sender, EventArgs e)
		{
			if (Page.IsValid)
			{
				YAF.Classes.Data.DB.user_addpoints(CurrentUserID, txtAddPoints.Text);
				BindData();
			}
		}

		protected void RemovePoints_Click(object sender, EventArgs e)
		{
			if (Page.IsValid)
			{
				YAF.Classes.Data.DB.user_removepoints(CurrentUserID, txtRemovePoints.Text);
				BindData();
			}
		}

		protected void SetUserPoints_Click(object sender, EventArgs e)
		{
			if (Page.IsValid)
			{
				YAF.Classes.Data.DB.user_setpoints(CurrentUserID, txtUserPoints.Text);
				BindData();
			}
		}
	}
}