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

namespace yaf
{
	/// <summary>
	/// Summary description for profile.
	/// </summary>
	public class profile : BasePage
	{
		protected System.Web.UI.WebControls.Label Name;
		protected System.Web.UI.WebControls.Label Group;
		protected System.Web.UI.WebControls.Label Joined;
		protected System.Web.UI.WebControls.HyperLink HomeLink;
		protected System.Web.UI.WebControls.Label Email;
		protected System.Web.UI.HtmlControls.HtmlTableRow EmailRow;
		protected System.Web.UI.WebControls.Label LastVisit;
		protected System.Web.UI.WebControls.Label NumPosts;
		protected System.Web.UI.WebControls.Label UserName;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			if(Request.QueryString["u"] == null)
				Response.Redirect(BaseDir);

			HomeLink.Text = ForumName;
			HomeLink.NavigateUrl = BaseDir;

			using(SqlCommand cmd = new SqlCommand("yaf_user_list")) {
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",Request.QueryString["u"]);
				cmd.Parameters.Add("@Approved",true);
				using(DataTable dt = DataManager.GetData(cmd)) {
					DataRow user = dt.Rows[0];

					UserName.Text = (string)user["Name"];
					Name.Text = (string)user["Name"];
					Group.Text = (string)user["GroupName"];
					Joined.Text = String.Format(CustomCulture,"{0}",FormatDateLong((DateTime)user["Joined"]));
					Email.Text = user["Email"].ToString();
					LastVisit.Text = FormatDateTime((DateTime)user["LastVisit"]);
					NumPosts.Text = user["NumPosts"].ToString();
				}
			}

			if(!IsPostBack) {
				if((bool)pageinfo["IsAdmin"]) {
					EmailRow.Visible = true;
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
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

	}
}
