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
using System.Globalization;

namespace yaf.pages.admin {
	/// <summary>
	/// Summary description for settings.
	/// </summary>
	public class hostsettings : AdminPage {
		protected System.Web.UI.WebControls.Button Save;
		protected System.Web.UI.WebControls.Label SQLVersion;
		protected System.Web.UI.WebControls.DropDownList TimeZones;
		protected System.Web.UI.WebControls.TextBox ForumSmtpServer, ForumSmtpUserName, ForumSmtpUserPass;
		protected System.Web.UI.WebControls.TextBox ForumEmailEdit;
		protected System.Web.UI.WebControls.TextBox AvatarWidth, AvatarHeight;
		protected System.Web.UI.WebControls.CheckBox EmailVerification, ShowMoved, BlankLinks;
		protected CheckBox AvatarUpload, AvatarRemote, ShowGroupsX, AllowRichEditX, AllowUserThemeX, AllowUserLanguageX, UseFileTableX;
		protected TextBox AvatarSize, MaxFileSize;
		protected controls.PageLinks PageLinks;
	
		private void Page_Load(object sender, System.EventArgs e) 
		{
			if(!IsHostAdmin)
				Data.AccessDenied();

			if(!IsPostBack) 
			{
				PageLinks.AddLink(Config.BoardSettings.Name,Forum.GetLink(Pages.forum));
				PageLinks.AddLink("Administration",Forum.GetLink(Pages.admin_admin));
				PageLinks.AddLink("Host Settings",Forum.GetLink(Pages.admin_hostsettings));

				BindData();
			}
		}

		private void BindData() {
			DataRow row;
			TimeZones.DataSource = Data.TimeZones();
			using(DataTable dt = DataProvider.system_list())
				row = dt.Rows[0];

			DataBind();
			SQLVersion.Text = (string)row["SQLVersion"];
			TimeZones.Items.FindByValue(row["TimeZone"].ToString()).Selected = true;
			ForumSmtpServer.Text = (string)row["SmtpServer"];
			ForumSmtpUserName.Text = row["SmtpUserName"].ToString();
			ForumSmtpUserPass.Text = row["SmtpUserPass"].ToString();
			ForumEmailEdit.Text = (string)row["ForumEmail"];
			EmailVerification.Checked = (bool)row["EmailVerification"];
			ShowMoved.Checked = (bool)row["ShowMoved"];
			BlankLinks.Checked = (bool)row["BlankLinks"];
			ShowGroupsX.Checked = (bool)row["ShowGroups"];
			AvatarWidth.Text = row["AvatarWidth"].ToString();
			AvatarHeight.Text = row["AvatarHeight"].ToString();
			AvatarUpload.Checked = (bool)row["AvatarUpload"];
			AvatarRemote.Checked = (bool)row["AvatarRemote"];
			AvatarSize.Text = row["AvatarSize"].ToString();
			AllowRichEditX.Checked = (bool)row["AllowRichEdit"];
			AllowUserThemeX.Checked = (bool)row["AllowUserTheme"];
			AllowUserLanguageX.Checked = (bool)row["AllowUserLanguage"];
			UseFileTableX.Checked = (bool)row["UseFileTable"];
			MaxFileSize.Text = row["MaxFileSize"].ToString();
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
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void Save_Click(object sender, System.EventArgs e) {
			string sUserName = ForumSmtpUserName.Text.Trim();
			if(sUserName.Length==0)
				sUserName = null;
			string sUserPass = ForumSmtpUserPass.Text.Trim();
			if(sUserPass.Length==0)
				sUserPass = null;

			DataProvider.system_save(
				TimeZones.SelectedItem.Value,
				ForumSmtpServer.Text,
				sUserName,
				sUserPass,
				ForumEmailEdit.Text,
				EmailVerification.Checked,
				ShowMoved.Checked,
				BlankLinks.Checked,
				ShowGroupsX.Checked,
				AvatarWidth.Text,
				AvatarHeight.Text,
				AvatarUpload.Checked,
				AvatarRemote.Checked,
				AvatarSize.Text,
				AllowRichEditX.Checked,
				AllowUserThemeX.Checked,
				AllowUserLanguageX.Checked,
				UseFileTableX.Checked,
				MaxFileSize.Text.Trim().Length>0 ? MaxFileSize.Text : null);

			Config.BoardSettings = null;	/// Reload forum settings
			Forum.Redirect(Pages.admin_admin);
		}
	}
}
