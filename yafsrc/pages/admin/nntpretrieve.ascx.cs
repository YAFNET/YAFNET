/* Yet Another Forum.net
 * Copyright (C) 2003 Bjørnar Henden
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

namespace YAF.Pages.Admin
{
	/// <summary>
	/// Summary description for ranks.
	/// </summary>
	public partial class nntpretrieve : AdminPage
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsPostBack) 
			{
				PageLinks.AddLink(BoardSettings.Name,Forum.GetLink( ForumPages.forum));
				PageLinks.AddLink("Administration",Forum.GetLink( ForumPages.admin_admin));
				PageLinks.AddLink("NNTP Retrieve","");

				BindData();
			}
		}

		private void BindData()
		{
			List.DataSource = YAF.Classes.Data.DB.nntpforum_list(PageBoardID,10,null,true);
			DataBind();
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			Retrieve.Click += new EventHandler(Retrieve_Click);
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

		private void Retrieve_Click(object sender, System.EventArgs e)
		{
			int nSeconds = int.Parse(Seconds.Text);
			if(nSeconds<1) nSeconds = 1;
			int nArticleCount = Classes.Nntp.ReadArticles(PageBoardID,10,nSeconds,BoardSettings.CreateNntpUsers);
			AddLoadMessage(String.Format("Retrieved {0} articles. {1:N2} articles per second.",nArticleCount,(double)nArticleCount/nSeconds));
			BindData();
		}

		protected string LastMessageNo(object _o) 
		{
			DataRowView row = (DataRowView)_o;
			return string.Format("{0:N0}",row["LastMessageNo"]);
		}
	}
}
