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
using System.Data.SqlClient;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace yaf.admin
{
	/// <summary>
	/// Summary description for main.
	/// </summary>
	public class main : BasePage
	{
		protected System.Web.UI.WebControls.Repeater ActiveList;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsAdmin) Response.Redirect(BaseDir);
			TopMenu = false;
			if(!IsPostBack) BindData();
		}

		private void BindData() {
			using(SqlCommand cmd = new SqlCommand("yaf_active_list")) {
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@Guests",true);
				ActiveList.DataSource = DataManager.GetData(cmd);
			}
			DataBind();
		}

		protected string FormatForumLink(object ForumID,object ForumName) {
			if(ForumID.ToString()=="" || ForumName.ToString()=="")
				return "";

			return String.Format("<a target=\"_top\" href=\"../topics.aspx?f={0}\">{1}</a>",ForumID,ForumName);
		}

		protected string FormatTopicLink(object TopicID,object TopicName) {
			if(TopicID.ToString()=="" || TopicName.ToString()=="")
				return "";

			return String.Format("<a target=\"_top\" href=\"../posts.aspx?t={0}\">{1}</a>",TopicID,TopicName);
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
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
	}
}
