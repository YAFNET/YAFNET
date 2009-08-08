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
using YAF.Classes.Core.Nntp;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages.Admin
{
	/// <summary>
	/// Summary description for ranks.
	/// </summary>
	public partial class nntpretrieve : YAF.Classes.Core.AdminPage
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsPostBack) 
			{
				PageLinks.AddLink(PageContext.BoardSettings.Name,YafBuildLink.GetLink( ForumPages.forum));
				PageLinks.AddLink("Administration",YafBuildLink.GetLink( ForumPages.admin_admin));
				PageLinks.AddLink("NNTP Retrieve","");

				BindData();
			}
		}

		private void BindData()
		{
			List.DataSource = YAF.Classes.Data.DB.nntpforum_list(PageContext.PageBoardID,10,null,true);
			DataBind();
		}

		protected void Retrieve_Click(object sender, System.EventArgs e)
		{
			int nSeconds = int.Parse(Seconds.Text);
			if(nSeconds<1) nSeconds = 1;
			int nArticleCount = YafNntp.ReadArticles(PageContext.PageBoardID,10,nSeconds,PageContext.BoardSettings.CreateNntpUsers);
			PageContext.AddLoadMessage(String.Format("Retrieved {0} articles. {1:N2} articles per second.",nArticleCount,(double)nArticleCount/nSeconds));
			BindData();
		}

		protected string LastMessageNo(object _o) 
		{
			DataRowView row = (DataRowView)_o;
			return string.Format("{0:N0}",row["LastMessageNo"]);
		}
	}
}
