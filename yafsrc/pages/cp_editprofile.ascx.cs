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
using System.Web.Security;
using System.Globalization;

namespace yaf.pages
{
	/// <summary>
	/// Summary description for cp_editprofile.
	/// </summary>
	public class cp_editprofile : BasePage
	{
		protected System.Web.UI.WebControls.TextBox Location;
		protected System.Web.UI.WebControls.TextBox HomePage;
		protected System.Web.UI.WebControls.DropDownList TimeZones;
		protected System.Web.UI.WebControls.TextBox Avatar;
		protected System.Web.UI.WebControls.TextBox OldPassword;
		protected System.Web.UI.WebControls.TextBox NewPassword1;
		protected System.Web.UI.WebControls.TextBox NewPassword2;
		protected System.Web.UI.WebControls.Button UpdateProfile;
		protected System.Web.UI.WebControls.TextBox Email;
		protected System.Web.UI.HtmlControls.HtmlInputFile File;
		protected HtmlTableRow AvatarRow, AvatarUploadRow, AvatarDeleteRow, AvatarRemoteRow;
		protected Button DeleteAvatar;
		protected DropDownList Theme, Language;
		protected PlaceHolder ForumSettingsRows;
		protected HtmlTableRow UserThemeRow, UserLanguageRow;
		protected controls.PageLinks PageLinks;
		protected TextBox Realname, Occupation, Interests, Weblog, MSN, YIM, AIM, ICQ;
		protected DropDownList Gender;
		
		private bool bUpdateEmail = false;

		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!Page.User.Identity.IsAuthenticated)
				Forum.Redirect(Pages.login,"ReturnUrl={0}",Request.RawUrl);
			
			if(!IsPostBack) {
				// Begin Modifications for enhanced profile
				Gender.Items.Add(GetText("gender0"));
				Gender.Items.Add(GetText("gender1"));
				Gender.Items.Add(GetText("gender2"));
				// End Modifications for enhanced profile

				BindData();

				PageLinks.AddLink(ForumName,Forum.GetLink(Pages.forum));
				PageLinks.AddLink(PageUserName,Forum.GetLink(Pages.cp_profile));
				PageLinks.AddLink(GetText("TITLE"),Request.RawUrl);

				DeleteAvatar.Text = GetText("delete_avatar");
				UpdateProfile.Text = GetText("Save");

				ForumSettingsRows.Visible = AllowUserTheme || AllowUserLanguage;
				UserThemeRow.Visible = AllowUserTheme;
				UserLanguageRow.Visible = AllowUserLanguage;
			}
		}

		private void BindData() {
			DataRow row;
			TimeZones.DataSource = Data.TimeZones();
			Theme.DataSource = Data.Themes();
			Theme.DataTextField = "Theme";
			Theme.DataValueField = "FileName";
			Language.DataSource = Data.Languages();
			Language.DataTextField = "Language";
			Language.DataValueField = "FileName";
			DataBind();

			using(DataTable dt = DB.user_list(PageUserID,true)) 
			{
				row = dt.Rows[0];
			}

			Location.Text = row["Location"].ToString();
			HomePage.Text = row["HomePage"].ToString();
			TimeZones.Items.FindByValue(row["TimeZone"].ToString()).Selected = true;
			Avatar.Text = row["Avatar"].ToString();
			Email.Text = row["Email"].ToString();
			Realname.Text = row["RealName"].ToString();
			Occupation.Text = row["Occupation"].ToString();
			Interests.Text = row["Interests"].ToString();
			Weblog.Text = row["Weblog"].ToString();
			MSN.Text = row["MSN"].ToString();
			YIM.Text = row["YIM"].ToString();
			AIM.Text = row["AIM"].ToString();
			ICQ.Text = row["ICQ"].ToString();
			Gender.SelectedIndex = Convert.ToInt32(row["Gender"]);

			string themeFile = Config.ConfigSection["theme"];
			string languageFile = Config.ConfigSection["language"];
			if(!row.IsNull("ThemeFile"))
				themeFile = (string)row["ThemeFile"];
			if(!row.IsNull("LanguageFile"))
				languageFile = (string)row["LanguageFile"];

			Theme.Items.FindByValue(themeFile).Selected = true;
			Language.Items.FindByValue(languageFile).Selected = true;

			AvatarDeleteRow.Visible = row["AvatarImage"].ToString().Length>0;
			using(DataTable dt = DB.system_list()) 
			{
				foreach(DataRow row2 in dt.Rows) 
				{
					AvatarUploadRow.Visible = (bool)row2["AvatarUpload"];
					AvatarRemoteRow.Visible = (bool)row2["AvatarRemote"];
				}
				AvatarRow.Visible = AvatarUploadRow.Visible || AvatarRemoteRow.Visible || AvatarDeleteRow.Visible;
			}
		}

		private void DeleteAvatar_Click(object sender, System.EventArgs e) 
		{
			DB.user_deleteavatar(PageUserID);
			BindData();
		}

		private void UpdateProfile_Click(object sender, System.EventArgs e) 
		{
			if(File.PostedFile!=null && File.PostedFile.FileName.Trim().Length>0 && File.PostedFile.ContentLength>0) 
			{
				long x,y;
				int nAvatarSize = 50000;
				using(DataTable dt = DB.system_list())
				{
					x = long.Parse(dt.Rows[0]["AvatarWidth"].ToString());
					y = long.Parse(dt.Rows[0]["AvatarHeight"].ToString());
					if(dt.Rows[0]["AvatarSize"]!=DBNull.Value)
						nAvatarSize = (int)dt.Rows[0]["AvatarSize"];
				}

#if true
				System.IO.Stream resized = null;

				using(System.Drawing.Image img = System.Drawing.Image.FromStream(File.PostedFile.InputStream))
				{
					if(img.Width>x || img.Height>y)
					{
						AddLoadMessage(String.Format(GetText("WARN_TOOBIG"),x,y));
						AddLoadMessage(String.Format(GetText("WARN_SIZE"),img.Width,img.Height));
						AddLoadMessage(GetText("WARN_RESIZED"));

						double newWidth		= img.Width;
						double newHeight	= img.Height;
						if(newWidth>x)
						{
							newHeight = newHeight * x / newWidth;
							newWidth = x;
						}
						if(newHeight>y) 
						{
							newWidth = newWidth * y / newHeight;
							newHeight = y;
						}

						using(System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(img, new System.Drawing.Size((int)newWidth, (int)newHeight)))
						{
							resized = new System.IO.MemoryStream();
							bitmap.Save(resized,System.Drawing.Imaging.ImageFormat.Jpeg);
						}
					}
					if (File.PostedFile.ContentLength>=nAvatarSize && resized == null)
					{
						AddLoadMessage(String.Format(GetText("WARN_BIGFILE"),nAvatarSize));
						AddLoadMessage(String.Format(GetText("WARN_FILESIZE"),File.PostedFile.ContentLength));
						return;
					}

					if(resized == null)
						DB.user_saveavatar(PageUserID,File.PostedFile.InputStream);
					else
						DB.user_saveavatar(PageUserID, resized);
				}			
#else
				System.Drawing.Image img = System.Drawing.Image.FromStream(File.PostedFile.InputStream);
				if(img.Width>x || img.Height>y) 
				{
					AddLoadMessage(String.Format("Image size can't be larger than {0}x{1} pixels.",x,y));
					AddLoadMessage(String.Format("The size of your image was {0}x{1} pixels.",img.Width,img.Height));
					return;
				}
				if(File.PostedFile.ContentLength>=nAvatarSize) 
				{
					AddLoadMessage(String.Format("The size of your image can't be more than {0} bytes.",nAvatarSize));
					AddLoadMessage(String.Format("The size of your image was {0} bytes.",File.PostedFile.ContentLength));
					return;
				}
				DB.user_saveavatar(PageUserID,File.PostedFile.InputStream);
#endif
			}

			if(bUpdateEmail && UseEmailVerification) 
			{
				string hashinput = DateTime.Now.ToString() + Email.Text + register.CreatePassword(20);
				string hash = FormsAuthentication.HashPasswordForStoringInConfigFile(hashinput,"md5");

				// Email Body
				string msg = ReadTemplate("changeemail.txt");
				msg = msg.Replace("{user}",PageUserName);
				msg = msg.Replace("{link}",String.Format("{1}{0}\r\n\r\n",Forum.GetLink(Pages.approve,"k={0}",hash),ServerURL));
				msg = msg.Replace("{newemail}",Email.Text);
				msg = msg.Replace("{key}",hash);
				msg = msg.Replace("{forumname}",ForumName);
				msg = msg.Replace("{forumlink}",ForumURL);

				DB.checkemail_save(PageUserID,hash,Email.Text);
				//  Build a MailMessage
				SendMail(ForumEmail,Email.Text,"Changed email",msg);
				AddLoadMessage(String.Format(GetText("mail_sent"),Email.Text));
			}

			if(OldPassword.Text.Length > 0) {
				if(NewPassword1.Text.Length==0 || NewPassword2.Text.Length==0) {
					AddLoadMessage(GetText("no_empty_password"));
					return;
				}
				if(NewPassword1.Text != NewPassword2.Text) {
					AddLoadMessage(GetText("no_password_match"));
					return;
				}

				string oldpw = FormsAuthentication.HashPasswordForStoringInConfigFile(OldPassword.Text,"md5");
				string newpw = FormsAuthentication.HashPasswordForStoringInConfigFile(NewPassword1.Text,"md5");

				if(!DB.user_changepassword(PageUserID,oldpw,newpw)) {
					AddLoadMessage(GetText("old_password_wrong"));
				}
			}

			object email = null;
			if(!UseEmailVerification)
				email = Email.Text;

			DB.user_save(PageUserID,null,null,email,null,Location.Text,HomePage.Text,TimeZones.SelectedValue,Avatar.Text,Language.SelectedValue,Theme.SelectedValue,null,
				MSN.Text,YIM.Text,AIM.Text,ICQ.Text,Realname.Text,Occupation.Text,Interests.Text,Gender.SelectedIndex,Weblog.Text);
			Forum.Redirect(Pages.cp_profile);
		}

		private void Email_TextChanged(object sender, System.EventArgs e) {
			bUpdateEmail = true;
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			DeleteAvatar.Click += new System.EventHandler(DeleteAvatar_Click);
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
			this.UpdateProfile.Click += new System.EventHandler(this.UpdateProfile_Click);
			this.Email.TextChanged += new System.EventHandler(this.Email_TextChanged);
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
}
