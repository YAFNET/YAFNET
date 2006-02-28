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

namespace yaf.pages.admin
{
	/// <summary>
	/// Summary description for editgroup.
	/// </summary>
	public class editnntpserver : AdminPage
	{
		protected TextBox Name, Address, Port, UserName, UserPass;
		protected Button Save, Cancel;
		protected controls.PageLinks PageLinks;

		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsPostBack) 
			{
				PageLinks.AddLink(BoardSettings.Name,Forum.GetLink(Pages.forum));
				PageLinks.AddLink("Administration",Forum.GetLink(Pages.admin_admin));
				PageLinks.AddLink("NNTP Servers","");

				BindData();
				if(Request.QueryString["s"] != null) 
				{
					using(DataTable dt = DB.nntpserver_list(PageBoardID,Request.QueryString["s"]))
					{
						DataRow row = dt.Rows[0];
						Name.Text		= row["Name"].ToString();
						Address.Text	= row["Address"].ToString();
						Port.Text		= row["Port"].ToString();
						UserName.Text	= row["UserName"].ToString();
						UserPass.Text	= row["UserPass"].ToString();
					}
				}
				else
				{
					Port.Text = "119";
				}
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
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
			this.Save.Click += new System.EventHandler(this.Save_Click);
			this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void BindData() {
			DataBind();
		}

		private void Cancel_Click(object sender, System.EventArgs e)
		{
			Forum.Redirect(Pages.admin_nntpservers);
		}

		private void Save_Click(object sender, System.EventArgs e)
		{
			if(Name.Text.Trim().Length==0) 
			{
				AddLoadMessage("Missing server name.");
				return;
			}
			if(Address.Text.Trim().Length==0) 
			{
				AddLoadMessage("Missing server address.");
				return;
			}

			object nntpServerID = null;
			if(Request.QueryString["s"]!=null) nntpServerID = Request.QueryString["s"];
			DB.nntpserver_save(nntpServerID,PageBoardID,Name.Text,Address.Text,Port.Text.Length>0 ? Port.Text : null,UserName.Text.Length>0 ? UserName.Text : null,UserPass.Text.Length>0 ? UserPass.Text : null);
			Forum.Redirect(Pages.admin_nntpservers);
		}
	}
}
