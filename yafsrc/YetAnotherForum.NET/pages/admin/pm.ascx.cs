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
	public partial class pm : YAF.Classes.Core.AdminPage
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsPostBack) {
				PageLinks.AddLink(PageContext.BoardSettings.Name,YafBuildLink.GetLink( ForumPages.forum));
				PageLinks.AddLink("Administration",YafBuildLink.GetLink( ForumPages.admin_admin));
				PageLinks.AddLink("PM Maintenance","");

				Days1.Text = "60";
				Days2.Text = "180";
				BindData();
			}
		}

		private void BindData()
		{
			using (DataTable dt = YAF.Classes.Data.DB.pmessage_info())
				Count.Text = dt.Rows[0]["NumTotal"].ToString();
		}

		private void commit_Click(object sender, EventArgs e)
		{
			YAF.Classes.Data.DB.pmessage_prune(Days1.Text, Days2.Text);
			BindData();
		}

		protected void DeleteButton_Load(object sender, System.EventArgs e)
		{
			((Button)sender).Attributes["onclick"] = String.Format("return confirm('{0}')", "Do you really want to delete private messages? This process is irreversible.");
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			commit.Click += new EventHandler(commit_Click);
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
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
