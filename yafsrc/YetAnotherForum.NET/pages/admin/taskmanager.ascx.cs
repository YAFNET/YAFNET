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

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages.Admin
{
	/// <summary>
	/// Summary description for prune.
	/// </summary>
	public partial class taskmanager : YAF.Classes.Core.AdminPage
	{
		protected void Page_Load( object sender, EventArgs e )
		{
			if ( !IsPostBack )
			{
				PageLinks.AddLink( PageContext.BoardSettings.Name, YafBuildLink.GetLink( ForumPages.forum ) );
				PageLinks.AddLink( "Administration", YafBuildLink.GetLink( ForumPages.admin_admin ) );
				PageLinks.AddLink( "Task Manager", "" );
			}

			lblTaskCount.Text = YafTaskModule.Current.TaskCount.ToString();
			taskRepeater.DataSource = YafTaskModule.Current.TaskManagerSnapshot;
			taskRepeater.DataBind();
		}

		protected string FormatTimeSpan(object item)
		{
			KeyValuePair<string, IBackgroundTask> task = (KeyValuePair<string, IBackgroundTask>) item;
			
			TimeSpan elapsed = DateTime.Now.Subtract( task.Value.Started ); 

			return String.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}", elapsed.Days, elapsed.Hours, elapsed.Minutes, elapsed.Seconds);
		}
	}
}