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
		protected System.Web.UI.WebControls.TextBox AvatarWidth, AvatarHeight, SmiliesColumns, SmiliesPerRow;
		protected System.Web.UI.WebControls.CheckBox EmailVerification, ShowMoved, BlankLinks;
		protected CheckBox AvatarUpload, AvatarRemote, ShowGroupsX, AllowRichEditX, AllowUserThemeX, AllowUserLanguageX, UseFileTableX, ShowRSSLinkX;
		protected TextBox AvatarSize, MaxFileSize;
		protected yaf.controls.AdminMenu Adminmenu1;
		protected yaf.controls.SaveScrollPos Savescrollpos1;
		protected controls.PageLinks PageLinks;
	
		private void Page_Load(object sender, System.EventArgs e) 
		{
			if(!IsHostAdmin)
				Data.AccessDenied();

			if(!IsPostBack) 
			{
				PageLinks.AddLink(BoardSettings.Name,Forum.GetLink(Pages.forum));
				PageLinks.AddLink("Administration",Forum.GetLink(Pages.admin_admin));
				PageLinks.AddLink("Host Settings",Forum.GetLink(Pages.admin_hostsettings));

				BindData();
			}

			// set widths manually since ASP.NET "forgets" to do it for browsers other then IE
			SmiliesPerRow.Attributes.Add("style","width:25px");
			SmiliesColumns.Attributes.Add("style","width:25px");
			ForumEmailEdit.Attributes.Add("style","width:200px");
			ForumSmtpServer.Attributes.Add("style","width:200px");
			ForumSmtpUserName.Attributes.Add("style","width:200px");
			ForumSmtpUserPass.Attributes.Add("style","width:200px");
		}

		private void BindData()
		{
			TimeZones.DataSource = Data.TimeZones();
			DataBind();

			// grab all the settings form the current board settings class
			SQLVersion.Text = BoardSettings.SQLVersion;
			TimeZones.Items.FindByValue(BoardSettings.TimeZoneRaw.ToString()).Selected = true;
			ForumSmtpServer.Text = BoardSettings.SmtpServer;
			ForumSmtpUserName.Text = BoardSettings.SmtpUserName;
			ForumSmtpUserPass.Text = BoardSettings.SmtpUserPass;
			ForumEmailEdit.Text = BoardSettings.ForumEmail;
			EmailVerification.Checked = BoardSettings.EmailVerification;
			ShowMoved.Checked = BoardSettings.ShowMoved;
			BlankLinks.Checked = BoardSettings.BlankLinks;
			ShowGroupsX.Checked = BoardSettings.ShowGroups;
			AvatarWidth.Text = BoardSettings.AvatarWidth.ToString();
			AvatarHeight.Text = BoardSettings.AvatarHeight.ToString();
			AvatarUpload.Checked = BoardSettings.AvatarUpload;
			AvatarRemote.Checked = BoardSettings.AvatarRemote;
			AvatarSize.Text = (BoardSettings.AvatarSize != 0) ? BoardSettings.AvatarSize.ToString() : "";
			AllowRichEditX.Checked = BoardSettings.AllowRichEdit;
			AllowUserThemeX.Checked = BoardSettings.AllowUserTheme;
			AllowUserLanguageX.Checked = BoardSettings.AllowUserLanguage;
			UseFileTableX.Checked = BoardSettings.UseFileTable;
			ShowRSSLinkX.Checked = BoardSettings.ShowRSSLink;
			MaxFileSize.Text = (BoardSettings.MaxFileSize != 0) ? BoardSettings.MaxFileSize.ToString() : "";
			SmiliesColumns.Text = BoardSettings.SmiliesColumns.ToString();
			SmiliesPerRow.Text = BoardSettings.SmiliesPerRow.ToString();
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

		private void Save_Click(object sender, System.EventArgs e)
		{
			string sUserName = ForumSmtpUserName.Text.Trim();
			string sUserPass = ForumSmtpUserPass.Text.Trim();

			if (sUserName.Length == 0) sUserName = null;			
			if (sUserPass.Length == 0) sUserPass = null;

			// write all the settings back to the settings class
			BoardSettings.TimeZoneRaw = Convert.ToInt32(TimeZones.SelectedItem.Value);
			BoardSettings.SmtpServer = ForumSmtpServer.Text;
			BoardSettings.SmtpUserName = sUserName;
			BoardSettings.SmtpUserPass = sUserPass;
			BoardSettings.ForumEmail = ForumEmailEdit.Text;
			BoardSettings.EmailVerification = EmailVerification.Checked;
			BoardSettings.ShowMoved = ShowMoved.Checked;
			BoardSettings.BlankLinks = BlankLinks.Checked;
			BoardSettings.ShowGroups = ShowGroupsX.Checked;
			BoardSettings.AvatarWidth = Convert.ToInt32(AvatarWidth.Text);
			BoardSettings.AvatarHeight = Convert.ToInt32(AvatarHeight.Text);
			BoardSettings.AvatarUpload = AvatarUpload.Checked;
			BoardSettings.AvatarRemote = AvatarRemote.Checked;
			BoardSettings.AvatarSize = (AvatarSize.Text.Trim().Length > 0) ? Convert.ToInt32(AvatarSize.Text) : 0;
			BoardSettings.AllowRichEdit = AllowRichEditX.Checked;
			BoardSettings.AllowUserTheme = AllowUserThemeX.Checked;
			BoardSettings.AllowUserLanguage = AllowUserLanguageX.Checked;
			BoardSettings.UseFileTable = UseFileTableX.Checked;
			BoardSettings.ShowRSSLink = ShowRSSLinkX.Checked;
			BoardSettings.MaxFileSize = (MaxFileSize.Text.Trim().Length > 0) ? Convert.ToInt32(MaxFileSize.Text.Trim()) : 0;
			BoardSettings.SmiliesColumns = Convert.ToInt32(SmiliesColumns.Text.Trim());
			BoardSettings.SmiliesPerRow = Convert.ToInt32(SmiliesPerRow.Text.Trim());

			// save the settings to the database
			BoardSettings.SaveRegistry();

			// reload all settings from the DB
			BoardSettings = null;

			Forum.Redirect(Pages.admin_admin);
		}
	}
}
