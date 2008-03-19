/* Yet Another Forum.NET
 * Copyright (C) 2006-2008 Jaben Cargman
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
using System.Collections.Specialized;
using YAF.Classes.Utils;

namespace YAF.Controls
{
	public partial class EditUsersAvatar : YAF.Classes.Base.BaseUserControl
	{
		private int CurrentUserID;
		private bool AdminEditMode = false;

		protected void Page_Load( object sender, EventArgs e )
		{
			if ( AdminEditMode && PageContext.IsAdmin && Request.QueryString ["u"] != null )
			{
				CurrentUserID = Convert.ToInt32( Request.QueryString ["u"] );
			}
			else
			{
				CurrentUserID = PageContext.PageUserID;
			}

			if ( !IsPostBack )
			{
				// check if it's a link from the avatar picker
				if ( Request.QueryString ["av"] != null )
				{
					// save the avatar right now...
					YAF.Classes.Data.DB.user_saveavatar( CurrentUserID, string.Format( "{2}{0}images/avatars/{1}", YafForumInfo.ForumRoot, Request.QueryString ["av"], YafForumInfo.ServerURL ), null );
				}

				UpdateRemote.Text = PageContext.Localization.GetText( "COMMON", "UPDATE" );
				UpdateUpload.Text = PageContext.Localization.GetText( "COMMON", "UPDATE" );
				Back.Text = PageContext.Localization.GetText( "COMMON", "BACK" );

				NoAvatar.Text = PageContext.Localization.GetText( "CP_EDITAVATAR", "NOAVATAR" );

				DeleteAvatar.Text = PageContext.Localization.GetText( "CP_EDITAVATAR", "AVATARDELETE" );
				DeleteAvatar.Attributes ["onclick"] = string.Format( "return confirm('{0}?')", PageContext.Localization.GetText( "CP_EDITAVATAR", "AVATARDELETE" ) );

				string addAdminParam = "";
				if ( AdminEditMode ) addAdminParam = "u=" + CurrentUserID.ToString();

				OurAvatar.NavigateUrl = YAF.Classes.Utils.YafBuildLink.GetLinkNotEscaped( YAF.Classes.Utils.ForumPages.avatar, addAdminParam );
				OurAvatar.Text = PageContext.Localization.GetText( "CP_EDITAVATAR", "OURAVATAR_SELECT" );				
			}

			BindData();
		}

		private void BindData()
		{
			DataRow row;

			using (DataTable dt = YAF.Classes.Data.DB.user_list(PageContext.PageBoardID, CurrentUserID, null))
			{
				row = dt.Rows [0];
			}

			AvatarImg.Visible = true;
			Avatar.Text = "";
			DeleteAvatar.Visible = false;
			NoAvatar.Visible = false;

			if ( PageContext.BoardSettings.AvatarUpload && row ["HasAvatarImage"] != null && long.Parse( row ["HasAvatarImage"].ToString() ) > 0 )
			{
				AvatarImg.ImageUrl = String.Format( "{0}resource.ashx?u={1}", YafForumInfo.ForumRoot, CurrentUserID );
				Avatar.Text = "";
				DeleteAvatar.Visible = true;
			}
			else if ( row ["Avatar"].ToString().Length > 0 ) // Took out PageContext.BoardSettings.AvatarRemote
			{
				AvatarImg.ImageUrl = String.Format( "{3}resource.ashx?url={0}&width={1}&height={2}",
					Server.UrlEncode( row ["Avatar"].ToString() ),
					PageContext.BoardSettings.AvatarWidth,
					PageContext.BoardSettings.AvatarHeight,
					YafForumInfo.ForumRoot );

				Avatar.Text = row ["Avatar"].ToString();
				DeleteAvatar.Visible = true;
			}
			else
			{
				AvatarImg.ImageUrl = "../images/noavatar.gif";
				NoAvatar.Visible = true;
			}

			int rowSpan = 2;

			AvatarUploadRow.Visible = (AdminEditMode ? true : PageContext.BoardSettings.AvatarUpload);
			AvatarRemoteRow.Visible = (AdminEditMode ? true : PageContext.BoardSettings.AvatarRemote);

			if ( AdminEditMode || PageContext.BoardSettings.AvatarUpload ) rowSpan++;
			if ( AdminEditMode || PageContext.BoardSettings.AvatarRemote ) rowSpan++;

			avatarImageTD.RowSpan = rowSpan;
		}

		protected void DeleteAvatar_Click( object sender, System.EventArgs e )
		{
			YAF.Classes.Data.DB.user_deleteavatar( CurrentUserID );
			BindData();
		}

		protected void Back_Click( object sender, System.EventArgs e )
		{
			if ( AdminEditMode )
				YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_users );
			else
				YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.cp_profile );
		}

		protected void RemoteUpdate_Click( object sender, System.EventArgs e )
		{
			if ( Avatar.Text.Length > 0 && !Avatar.Text.StartsWith( "http://" ) )
				Avatar.Text = "http://" + Avatar.Text;
		
			// update
			YAF.Classes.Data.DB.user_saveavatar( CurrentUserID, Avatar.Text.Trim(), null );

			// clear the URL out...
			Avatar.Text = "";

			BindData();
		}

		protected void UploadUpdate_Click( object sender, System.EventArgs e )
		{
			if ( File.PostedFile != null && File.PostedFile.FileName.Trim().Length > 0 && File.PostedFile.ContentLength > 0 )
			{
				long x = PageContext.BoardSettings.AvatarWidth;
				long y = PageContext.BoardSettings.AvatarHeight;
				int nAvatarSize = PageContext.BoardSettings.AvatarSize;

				System.IO.Stream resized = null;

				try
				{
					using ( System.Drawing.Image img = System.Drawing.Image.FromStream( File.PostedFile.InputStream ) )
					{
						if ( img.Width > x || img.Height > y )
						{
							PageContext.AddLoadMessage( String.Format( PageContext.Localization.GetText( "CP_EDITAVATAR", "WARN_TOOBIG" ), x, y ) );
							PageContext.AddLoadMessage( String.Format( PageContext.Localization.GetText( "CP_EDITAVATAR", "WARN_SIZE" ), img.Width, img.Height ) );
							PageContext.AddLoadMessage( PageContext.Localization.GetText( "CP_EDITAVATAR", "WARN_RESIZED" ) );

							double newWidth = img.Width;
							double newHeight = img.Height;
							if ( newWidth > x )
							{
								newHeight = newHeight * x / newWidth;
								newWidth = x;
							}
							if ( newHeight > y )
							{
								newWidth = newWidth * y / newHeight;
								newHeight = y;
							}

							using ( System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap( img, new System.Drawing.Size( ( int )newWidth, ( int )newHeight ) ) )
							{
								resized = new System.IO.MemoryStream();
								bitmap.Save( resized, System.Drawing.Imaging.ImageFormat.Jpeg );
							}
						}
						if ( nAvatarSize > 0 && File.PostedFile.ContentLength >= nAvatarSize && resized == null )
						{
							PageContext.AddLoadMessage( String.Format( PageContext.Localization.GetText( "CP_EDITAVATAR", "WARN_BIGFILE" ), nAvatarSize ) );
							PageContext.AddLoadMessage( String.Format( PageContext.Localization.GetText( "CP_EDITAVATAR", "WARN_FILESIZE" ), File.PostedFile.ContentLength ) );
							return;
						}

						if ( resized == null )
							YAF.Classes.Data.DB.user_saveavatar( CurrentUserID, null, File.PostedFile.InputStream );
						else
							YAF.Classes.Data.DB.user_saveavatar( CurrentUserID, null, resized );
					}
				}
				catch
				{
					// image is probably invalid...
					PageContext.AddLoadMessage( PageContext.Localization.GetText( "CP_EDITAVATAR", "INVALID_FILE" ) );
				}

				BindData();
			}
		}

		public bool InAdminPages
		{
			get
			{
				return AdminEditMode;
			}
			set
			{
				AdminEditMode = value;
			}
		}
	}
}