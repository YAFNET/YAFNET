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

namespace yaf.admin
{
	/// <summary>
	/// Summary description for editgroup.
	/// </summary>
	public class editrank : BasePage
	{
		protected System.Web.UI.WebControls.TextBox Name;
		protected System.Web.UI.WebControls.CheckBox IsStart;
		protected System.Web.UI.WebControls.Button Save;
		protected System.Web.UI.WebControls.CheckBox IsLadder;
		protected System.Web.UI.WebControls.TextBox MinPosts;
		protected System.Web.UI.WebControls.Button Cancel;
		protected DropDownList RankImage;
		protected HtmlImage Preview;

		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsAdmin) Response.Redirect(BaseDir);
			TopMenu = false;
			if(!IsPostBack) 
			{
				BindData();
				if(Request.QueryString["r"] != null) 
				{
					using(DataTable dt = DB.rank_list(Request.QueryString["r"]))
					{
						DataRow row = dt.Rows[0];
						Name.Text = (string)row["Name"];
						IsStart.Checked = (bool)row["IsStart"];
						IsLadder.Checked = (bool)row["IsLadder"];
						MinPosts.Text = row["MinPosts"].ToString();
						ListItem item = RankImage.Items.FindByText(row["RankImage"].ToString());
						if(item!=null) item.Selected = true;
						Preview.Src = String.Format("../images/ranks/{0}",row["RankImage"]);
					}
				}
			}
			RankImage.Attributes["onchange"] = "getElementById('Preview').src='../images/ranks/' + this.value";
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
			using(DataTable dt = new DataTable("Files")) 
			{
				dt.Columns.Add("FileID",typeof(long));
				dt.Columns.Add("FileName",typeof(string));
				DataRow dr = dt.NewRow();
				dr["FileID"] = 0;
				dr["FileName"] = "Select Rank Image";
				dt.Rows.Add(dr);
				
				System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(Request.MapPath(String.Format("{0}images/ranks",BaseDir)));
				System.IO.FileInfo[] files = dir.GetFiles("*.*");
				long nFileID = 1;
				foreach(System.IO.FileInfo file in files) 
				{
					string sExt = file.Extension.ToLower();
					if(sExt!=".png" && sExt!=".gif" && sExt!=".jpg")
						continue;
					
					dr = dt.NewRow();
					dr["FileID"] = nFileID++;
					dr["FileName"] = file.Name;
					dt.Rows.Add(dr);
				}
				
				RankImage.DataSource = dt;
				RankImage.DataValueField = "FileName";
				RankImage.DataTextField = "FileName";
			}
			DataBind();
		}

		private void Cancel_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("ranks.aspx");
		}

		private void Save_Click(object sender, System.EventArgs e)
		{
			// Group
			int RankID = 0;
			if(Request.QueryString["r"] != null) RankID = int.Parse(Request.QueryString["r"]);

			object rankImage = null;
			if(RankImage.SelectedIndex>0)
				rankImage = RankImage.SelectedValue;
			DB.rank_save(RankID,Name.Text,IsStart.Checked,IsLadder.Checked,MinPosts.Text,rankImage);	
				
			Response.Redirect("ranks.aspx");
		}
	}
}
