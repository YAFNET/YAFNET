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
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Globalization;

namespace yaf.pages
{
	/// <summary>
	/// Summary description for editprofile.
	/// </summary>
	public class cp_profile : ForumPage
	{
		protected System.Web.UI.WebControls.Label TitleUserName;
		protected System.Web.UI.WebControls.Label NumPosts;
		protected System.Web.UI.WebControls.Label Name;
		protected System.Web.UI.WebControls.Label Joined;
		protected System.Web.UI.WebControls.Label AccountEmail;
		protected Repeater Groups;
		protected HtmlImage AvatarImage;
		protected controls.PageLinks PageLinks;

		public cp_profile() : base("CP_PROFILE")
		{
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!User.IsAuthenticated)
				Forum.Redirect(Pages.login,"ReturnUrl={0}",Request.RawUrl);

			if(!IsPostBack) {
				BindData();

				PageLinks.AddLink(Config.BoardSettings.Name,Forum.GetLink(Pages.forum));
				PageLinks.AddLink(PageUserName,Request.RawUrl);
			}
		}

		private void BindData() {
			DataRow row;

			Groups.DataSource = DB.usergroup_list(PageBoardID,PageUserID);

			// Bind			
			DataBind();
			using(DataTable dt = DB.user_list(PageBoardID,PageUserID,true)) {
				row = dt.Rows[0];
			}

			TitleUserName.Text = (string)row["Name"];
			AccountEmail.Text = row["Email"].ToString();
			Name.Text = (string)row["Name"];
			Joined.Text = FormatDateTime((DateTime)row["Joined"]);
			NumPosts.Text = String.Format("{0:N0}",row["NumPosts"]);
			AvatarImage.Visible = row["AvatarImage"].ToString().Length>0;
			if(AvatarImage.Visible)
				AvatarImage.Src = String.Format("{0}image.aspx?u={1}",Data.ForumRoot,PageUserID);
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
