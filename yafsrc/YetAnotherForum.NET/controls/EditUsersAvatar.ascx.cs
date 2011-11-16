/* Yet Another Forum.NET
 * Copyright (C) 2006-2011 Jaben Cargman
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

namespace YAF.Controls
{
	#region Using

	using System;
	using System.Data;
	using System.Drawing;
	using System.IO;
	using System.Security.Cryptography;
	using System.Text;
	using System.Web;

	using YAF.Classes;
	using YAF.Classes.Data;
	using YAF.Core;
	using YAF.Types;
	using YAF.Types.Constants;
	using YAF.Types.EventProxies;
	using YAF.Types.Interfaces;
	using YAF.Utils;
	using YAF.Utils.Helpers;

	#endregion

	/// <summary>
	/// The edit users avatar.
	/// </summary>
	public partial class EditUsersAvatar : BaseUserControl
	{
		#region Constants and Fields

		/// <summary>
		///   The admin edit mode.
		/// </summary>
		private bool _adminEditMode;

		/// <summary>
		///   The current user id.
		/// </summary>
		private int _currentUserID;

		#endregion

		#region Properties

		/// <summary>
		///   Gets or sets a value indicating whether InAdminPages.
		/// </summary>
		public bool InAdminPages
		{
			get
			{
				return this._adminEditMode;
			}

			set
			{
				this._adminEditMode = value;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// The back_ click.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void Back_Click([NotNull] object sender, [NotNull] EventArgs e)
		{
			YafBuildLink.Redirect(this._adminEditMode ? ForumPages.admin_users : ForumPages.cp_profile);
		}

		/// <summary>
		/// The delete avatar_ click.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void DeleteAvatar_Click([NotNull] object sender, [NotNull] EventArgs e)
		{
			LegacyDb.user_deleteavatar(this._currentUserID);

			// clear the cache for this user...
			this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this._currentUserID));
			this.BindData();
		}

		/// <summary>
		/// The page_ load.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
		{
			this.PageContext.QueryIDs = new QueryStringIDHelper("u");

			if (this._adminEditMode && this.PageContext.IsAdmin && this.PageContext.QueryIDs.ContainsKey("u"))
			{
				this._currentUserID = (int)this.PageContext.QueryIDs["u"];
			}
			else
			{
				this._currentUserID = this.PageContext.PageUserID;
			}

			if (this.IsPostBack)
			{
				return;
			}

			// check if it's a link from the avatar picker
			if (this.Request.QueryString.GetFirstOrDefault("av") != null)
			{
				// save the avatar right now...
				LegacyDb.user_saveavatar(
						this._currentUserID,
						"{0}{1}".FormatWith(BaseUrlBuilder.BaseUrl, this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("av")),
						null,
						null);

				// clear the cache for this user...
				this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this._currentUserID));
			}

			this.UpdateRemote.Text = this.GetText("COMMON", "UPDATE");
			this.UpdateUpload.Text = this.GetText("COMMON", "UPDATE");
			this.Back.Text = this.GetText("COMMON", "BACK");

			this.NoAvatar.Text = this.GetText("CP_EDITAVATAR", "NOAVATAR");

			this.DeleteAvatar.Text = this.GetText("CP_EDITAVATAR", "AVATARDELETE");
			this.DeleteAvatar.Attributes["onclick"] =
					"return confirm('{0}?')".FormatWith(this.GetText("CP_EDITAVATAR", "AVATARDELETE"));

			string addAdminParam = string.Empty;
			if (this._adminEditMode)
			{
				addAdminParam = "u={0}".FormatWith(this._currentUserID);
			}

			this.OurAvatar.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.avatar, addAdminParam);
			this.OurAvatar.Text = this.GetText("CP_EDITAVATAR", "OURAVATAR_SELECT");

			this.noteRemote.Text = this.GetTextFormatted(
					"NOTE_REMOTE",
					this.Get<YafBoardSettings>().AvatarWidth.ToString(),
					this.Get<YafBoardSettings>().AvatarHeight.ToString());
			this.noteLocal.Text = this.GetTextFormatted(
					"NOTE_LOCAL",
					this.Get<YafBoardSettings>().AvatarWidth.ToString(),
					this.Get<YafBoardSettings>().AvatarHeight,
					(this.Get<YafBoardSettings>().AvatarSize / 1024).ToString());

			this.BindData();
		}

		/// <summary>
		/// The remote update_ click.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void RemoteUpdate_Click([NotNull] object sender, [NotNull] EventArgs e)
		{
			if (this.Avatar.Text.Length > 0 && !this.Avatar.Text.StartsWith("http://"))
			{
				this.Avatar.Text = "http://" + this.Avatar.Text;
			}

			// update
			LegacyDb.user_saveavatar(this._currentUserID, this.Avatar.Text.Trim(), null, null);

			// clear the cache for this user...
			this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this._currentUserID));

			// clear the URL out...
			this.Avatar.Text = string.Empty;

			this.BindData();
		}

		/// <summary>
		/// The upload update_ click.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void UploadUpdate_Click([NotNull] object sender, [NotNull] EventArgs e)
		{
			if (this.File.PostedFile == null || this.File.PostedFile.FileName.Trim().Length <= 0 ||
					this.File.PostedFile.ContentLength <= 0)
			{
				return;
			}

			long x = this.Get<YafBoardSettings>().AvatarWidth;
			long y = this.Get<YafBoardSettings>().AvatarHeight;
			int nAvatarSize = this.Get<YafBoardSettings>().AvatarSize;

			byte[] resized = null;

			try
			{
				using (Image img = Image.FromStream(this.File.PostedFile.InputStream))
				{
					if (img.Width > x || img.Height > y)
					{
						this.PageContext.AddLoadMessage(
								this.GetText("CP_EDITAVATAR", "WARN_TOOBIG").FormatWith(x, y));
						this.PageContext.AddLoadMessage(
								this.GetText("CP_EDITAVATAR", "WARN_SIZE").FormatWith(img.Width, img.Height));
						this.PageContext.AddLoadMessage(this.GetText("CP_EDITAVATAR", "WARN_RESIZED"));

						resized = ImageHelper.GetResizedImageStreamFromImage(img, x, y).ToArray();
					}
				}

				// Delete old first...
				LegacyDb.user_deleteavatar(this._currentUserID);

				LegacyDb.user_saveavatar(this._currentUserID, null, resized ?? this.File.PostedFile.InputStream.ToArray(), this.File.PostedFile.ContentType);

				// clear the cache for this user...
				this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this._currentUserID));

				if (nAvatarSize > 0 && this.File.PostedFile.ContentLength >= nAvatarSize && resized == null)
				{
					this.PageContext.AddLoadMessage(
						this.GetText("CP_EDITAVATAR", "WARN_BIGFILE").FormatWith(nAvatarSize));
					this.PageContext.AddLoadMessage(
						this.GetText("CP_EDITAVATAR", "WARN_FILESIZE").FormatWith(
							this.File.PostedFile.ContentLength));
				}

				this.AvatarImg.ImageUrl = "{0}resource.ashx?u={1}&upd={2}".FormatWith(
					YafForumInfo.ForumClientFileRoot, this._currentUserID, DateTime.Now.Ticks);

				if (this.AvatarImg.ImageUrl.IsSet())
				{
					this.NoAvatar.Visible = false;
				}
			}
			catch (Exception)
			{
				// image is probably invalid...
				this.PageContext.AddLoadMessage(this.GetText("CP_EDITAVATAR", "INVALID_FILE"));
			}

			// this.BindData();
		}

		/// <summary>
		/// The bind data.
		/// </summary>
		private void BindData()
		{
			DataRow row;

			using (DataTable dt = LegacyDb.user_list(this.PageContext.PageBoardID, this._currentUserID, null))
			{
				row = dt.Rows[0];
			}

			this.AvatarImg.Visible = true;
			this.Avatar.Text = string.Empty;
			this.DeleteAvatar.Visible = false;
			this.NoAvatar.Visible = false;

			if (this.Get<YafBoardSettings>().AvatarUpload && row["HasAvatarImage"] != null &&
					long.Parse(row["HasAvatarImage"].ToString()) > 0)
			{
				this.AvatarImg.ImageUrl = "{0}resource.ashx?u={1}".FormatWith(
					YafForumInfo.ForumClientFileRoot, this._currentUserID);
				this.Avatar.Text = string.Empty;
				this.DeleteAvatar.Visible = true;
			}
			else if (row["Avatar"].ToString().Length > 0)
			{
				// Took out PageContext.BoardSettings.AvatarRemote
				this.AvatarImg.ImageUrl =
					"{3}resource.ashx?url={0}&width={1}&height={2}".FormatWith(
						this.Server.UrlEncode(row["Avatar"].ToString()),
						this.Get<YafBoardSettings>().AvatarWidth,
						this.Get<YafBoardSettings>().AvatarHeight,
						YafForumInfo.ForumClientFileRoot);

				this.Avatar.Text = row["Avatar"].ToString();
				this.DeleteAvatar.Visible = true;
			}
			else if (this.Get<YafBoardSettings>().AvatarGravatar)
			{
				var x = new MD5CryptoServiceProvider();
				byte[] bs = Encoding.UTF8.GetBytes(this.PageContext.User.Email);
				bs = x.ComputeHash(bs);
				var s = new StringBuilder();
				foreach (byte b in bs)
				{
					s.Append(b.ToString("x2").ToLower());
				}

				string emailHash = s.ToString();

				string gravatarUrl = "http://www.gravatar.com/avatar/{0}.jpg?r={1}".FormatWith(
					emailHash, this.Get<YafBoardSettings>().GravatarRating);

				this.AvatarImg.ImageUrl =
					"{3}resource.ashx?url={0}&width={1}&height={2}".FormatWith(
						this.Server.UrlEncode(gravatarUrl),
						this.Get<YafBoardSettings>().AvatarWidth,
						this.Get<YafBoardSettings>().AvatarHeight,
						YafForumInfo.ForumClientFileRoot);

				this.NoAvatar.Text = "Gravatar Image";
				this.NoAvatar.Visible = true;
			}
			else
			{
				this.AvatarImg.ImageUrl = "../images/noavatar.gif";
				this.NoAvatar.Visible = true;
			}

			int rowSpan = 2;

			this.AvatarUploadRow.Visible = this._adminEditMode ? true : this.Get<YafBoardSettings>().AvatarUpload;
			this.AvatarRemoteRow.Visible = this._adminEditMode ? true : this.Get<YafBoardSettings>().AvatarRemote;
			this.AvatarOurs.Visible = this._adminEditMode ? true : this.Get<YafBoardSettings>().AvatarGallery;

			if (this._adminEditMode || this.Get<YafBoardSettings>().AvatarUpload)
			{
				rowSpan++;
			}

			if (this._adminEditMode || this.Get<YafBoardSettings>().AvatarRemote)
			{
				rowSpan++;
			}

			this.avatarImageTD.RowSpan = rowSpan;
		}

		#endregion
	}
}