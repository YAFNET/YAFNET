/* Copyright (C) 2003 Bjørnar Henden
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
	/// Summary description for bannedip.
	/// </summary>
	public class bannedip : BasePage
	{
		protected Repeater list;

		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsAdmin) Response.Redirect(BaseDir);

			TopMenu = false;

			if(!IsPostBack) {
				BindData();
			}
		}

		private void BindData() {
			list.DataSource = DataManager.GetData("yaf_bannedip_list",CommandType.StoredProcedure);
			DataBind();
		}

		private void list_ItemCommand(object sender, RepeaterCommandEventArgs e) {
			if(e.CommandName == "add")
				Response.Redirect("bannedip_edit.aspx");
			else if(e.CommandName == "edit")
				Response.Redirect(String.Format("bannedip_edit.aspx?i={0}",e.CommandArgument));
			else if(e.CommandName == "delete") {
				using(SqlCommand cmd = new SqlCommand("yaf_bannedip_delete")) {
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@ID",e.CommandArgument);
					DataManager.ExecuteNonQuery(cmd);
					Cache.Remove("bannedip");
					BindData();
				}
				AddLoadMessage("Removed IP address ban.");
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			list.ItemCommand += new RepeaterCommandEventHandler(list_ItemCommand);
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
