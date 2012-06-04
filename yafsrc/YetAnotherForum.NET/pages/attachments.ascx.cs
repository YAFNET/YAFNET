/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2012 Jaben Cargman
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
namespace YAF.Pages
{
	// YAF.Pages
	#region Using

	using System;
	using System.Data;
	using System.IO;
	using System.Web;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;

	using YAF.Classes;
	using YAF.Classes.Data;
	using YAF.Core;
	using YAF.Types;
	using YAF.Types.Constants;
	using YAF.Types.Flags;
	using YAF.Types.Interfaces;
	using YAF.Utils;

	#endregion

	/// <summary>
	/// The attachments Page Class.
	/// </summary>
	public partial class attachments : ForumPage
	{
		#region Constants and Fields

		/// <summary>
		///   The _forum.
		/// </summary>
		private DataRow _forum;

		/// <summary>
		///   The _topic.
		/// </summary>
		private DataRow _topic;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		///   Initializes a new instance of the <see cref = "attachments" /> class.
		/// </summary>
		public attachments()
			: base("ATTACHMENTS")
		{
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
			if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("ra").IsSet())
			{
				if (Config.IsRainbow)
				{
					YafBuildLink.Redirect(ForumPages.info, "i=1");
				}

				// string poll = string.Empty;
				string lnk;
				string fullflnk = string.Empty;
				if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("f").IsSet())
				{
					lnk = this.Request.QueryString.GetFirstOrDefault("f");
					fullflnk = "f={0}&".FormatWith(lnk);
				}
				else
				{
					lnk = this.PageContext.PageForumID.ToString();
				}

				// Tell a user that his message will have to be approved by a moderator
				string url = YafBuildLink.GetLink(ForumPages.topics, "f={0}", lnk);

				// new topic variable
				if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("t").IsSet())
				{
					url = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("t");
					YafBuildLink.Redirect(ForumPages.polledit, "{0}t={1}&ra=1", fullflnk, this.Server.UrlEncode(url));
				}
				else
				{
					YafBuildLink.Redirect(ForumPages.info, "i=1&url={0}", this.Server.UrlEncode(url));
				}
			}

			// the post is already approved and we can view it
			if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("t").IsSet())
			{
				YafBuildLink.Redirect(
					ForumPages.polledit, "t={0}", this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("t"));
			}
			else
			{
				YafBuildLink.Redirect(
					ForumPages.posts, "m={0}#{0}", this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("m"));
			}
		}

		/// <summary>
		/// The delete_ load.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void Delete_Load([NotNull] object sender, [NotNull] EventArgs e)
		{
			((LinkButton)sender).Attributes["onclick"] = "return confirm('{0}')".FormatWith(this.GetText("ASK_DELETE"));
		}

		/// <summary>
		/// The list_ item command.
		/// </summary>
		/// <param name="source">
		/// The source.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void List_ItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
		{
			switch (e.CommandName)
			{
				case "delete":
					LegacyDb.attachment_delete(e.CommandArgument);
					this.BindData();
					this.uploadtitletr.Visible = true;
					this.selectfiletr.Visible = true;
					break;
			}
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
			using (DataTable dt = LegacyDb.forum_list(this.PageContext.PageBoardID, this.PageContext.PageForumID))
			{
				this._forum = dt.Rows[0];
			}

			this._topic = LegacyDb.topic_info(this.PageContext.PageTopicID);

			if (this.IsPostBack)
			{
				return;
			}

			if (!this.PageContext.ForumModeratorAccess && !this.PageContext.ForumUploadAccess)
			{
				YafBuildLink.AccessDenied();
			}

			if (!this.PageContext.ForumReadAccess)
			{
				YafBuildLink.AccessDenied();
			}

			// Ederon : 9/9/2007 - moderaotrs can attach in locked posts
			if (this._topic["Flags"].BinaryAnd(TopicFlags.Flags.IsLocked) && !this.PageContext.ForumModeratorAccess)
			{
				YafBuildLink.AccessDenied(/*"The topic is closed."*/);
			}

			if (this._forum["Flags"].BinaryAnd(ForumFlags.Flags.IsLocked))
			{
				YafBuildLink.AccessDenied(/*"The forum is closed."*/);
			}

			// Check that non-moderators only edit messages they have written
			if (!this.PageContext.ForumModeratorAccess)
			{
				using (DataTable dt = LegacyDb.message_list(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("m")))
				{
					if ((int)dt.Rows[0]["UserID"] != this.PageContext.PageUserID)
					{
						YafBuildLink.AccessDenied(/*"You didn't post this message."*/);
					}
				}
			}

			if (this.PageContext.Settings.LockedForum == 0)
			{
				this.PageLinks.AddLink(this.Get<YafBoardSettings>().Name, YafBuildLink.GetLink(ForumPages.forum));
				this.PageLinks.AddLink(
					this.PageContext.PageCategoryName, 
					YafBuildLink.GetLink(ForumPages.forum, "c={0}", this.PageContext.PageCategoryID));
			}

			this.PageLinks.AddForumLinks(this.PageContext.PageForumID);
			this.PageLinks.AddLink(
				this.PageContext.PageTopicName, YafBuildLink.GetLink(ForumPages.posts, "t={0}", this.PageContext.PageTopicID));
			this.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);

				this.Back.Text = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("t").IsNotSet()
														 ? this.GetText("BACK")
														 : this.GetText("COMMON", "CONTINUE");

			this.Upload.Text = this.GetText("UPLOAD");

			// MJ : 10/14/2007 - list of allowed file extensions
			DataTable extensionTable = LegacyDb.extension_list(this.PageContext.PageBoardID);

			string types = string.Empty;
			bool bFirst = true;

			foreach (DataRow row in extensionTable.Rows)
			{
				types += "{1}*.{0}".FormatWith(row["Extension"].ToString(), bFirst ? string.Empty : ", ");
				if (bFirst)
				{
					bFirst = false;
				}
			}

			if (types.IsSet())
			{
				this.ExtensionsList.Text = types;
			}

			if (this.Get<YafBoardSettings>().MaxFileSize > 0)
			{
				this.UploadNodePlaceHold.Visible = true;
				this.UploadNote.Text = this.GetTextFormatted(
					"UPLOAD_NOTE", (this.Get<YafBoardSettings>().MaxFileSize / 1024).ToString());
			}
			else
			{
				this.UploadNodePlaceHold.Visible = false;
			}

			this.BindData();
		}

		/// <summary>
		/// The upload_ click.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void Upload_Click([NotNull] object sender, [NotNull] EventArgs e)
		{
			try
			{
				if (this.CheckValidFile(this.File))
				{
					this.SaveAttachment(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("m"), this.File);
				}

				this.BindData();
			}
			catch (Exception x)
			{
				LegacyDb.eventlog_create(this.PageContext.PageUserID, this, x);
				this.PageContext.AddLoadMessage(x.Message);
				return;
			}
		}

		/// <summary>
		/// The bind data.
		/// </summary>
		private void BindData()
		{
      DataTable dt = LegacyDb.attachment_list(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("m"), null, null,0,1000);
			this.List.DataSource = dt;

			this.List.Visible = dt.Rows.Count > 0;

			// show disallowed or allowed localized text depending on the Board Setting
			this.ExtensionTitle.LocalizedTag = this.Get<YafBoardSettings>().FileExtensionAreAllowed
																					 ? "ALLOWED_EXTENSIONS"
																					 : "DISALLOWED_EXTENSIONS";

			if (this.Get<YafBoardSettings>().MaxNumberOfAttachments > 0)
			{
				if (dt.Rows.Count > (this.Get<YafBoardSettings>().MaxNumberOfAttachments - 1))
				{
					this.uploadtitletr.Visible = false;
					this.selectfiletr.Visible = false;
				}
			}

			this.DataBind();
		}

		/// <summary>
		/// The check valid file.
		/// </summary>
		/// <param name="uploadedFile">
		/// The uploaded file.
		/// </param>
		/// <returns>
		/// Returns if the File Is Valid
		/// </returns>
		private bool CheckValidFile([NotNull] HtmlInputFile uploadedFile)
		{
			string filePath = uploadedFile.PostedFile.FileName.Trim();

			if (filePath.IsNotSet() || uploadedFile.PostedFile.ContentLength == 0)
			{
				return false;
			}

			string extension = Path.GetExtension(filePath).ToLower();

			// remove the "period"
			extension = extension.Replace(".", string.Empty);

			// If we don't get a match from the db, then the extension is not allowed
			DataTable dt = LegacyDb.extension_list(this.PageContext.PageBoardID, extension);

			bool bInList = dt.Rows.Count > 0;
			bool bError = false;
				
			if (this.Get<YafBoardSettings>().FileExtensionAreAllowed && !bInList)
			{
				// since it's not in the list -- it's invalid
				bError = true;
			}
			else if (!this.Get<YafBoardSettings>().FileExtensionAreAllowed && bInList)
			{
				// since it's on the list -- it's invalid
				bError = true;
			}

			if (filePath.Contains(";."))
			{
					// IIS semicolon valnerabilty fix
					bError = true;
			}

			if (bError)
			{
				// just throw an error that this file is invalid...
				this.PageContext.AddLoadMessage(this.GetTextFormatted("FILEERROR", extension));
				return false;
			}

			return true;
		}

		/// <summary>
		/// The save attachment.
		/// </summary>
		/// <param name="messageID">
		/// The message id.
		/// </param>
		/// <param name="file">
		/// The file.
		/// </param>
		private void SaveAttachment([NotNull] object messageID, [NotNull] HtmlInputFile file)
		{
			if (file.PostedFile == null || file.PostedFile.FileName.IsNotSet() || file.PostedFile.ContentLength == 0)
			{
				return;
			}

			string filename = file.PostedFile.FileName;

			int pos = filename.LastIndexOfAny(new[] { '/', '\\' });
			if (pos >= 0)
			{
				filename = filename.Substring(pos + 1);
			}

			// filename can be only 255 characters long (due to table column)
			filename = filename.Truncate(255, string.Empty);

			// verify the size of the attachment
			if (this.Get<YafBoardSettings>().MaxFileSize > 0 &&
					file.PostedFile.ContentLength > this.Get<YafBoardSettings>().MaxFileSize)
			{
				this.PageContext.AddLoadMessage(
					this.GetTextFormatted(
						"UPLOAD_TOOBIG", file.PostedFile.ContentLength / 1024, this.Get<YafBoardSettings>().MaxFileSize / 1024));

				return;
			}

			if (this.Get<YafBoardSettings>().UseFileTable)
			{
				LegacyDb.attachment_save(
					messageID, filename, file.PostedFile.ContentLength, file.PostedFile.ContentType, file.PostedFile.InputStream);
			}
			else
			{
				this.Get<IFileSystem>().Save(YafFolder.Uploads, "{0}.{1}.yafupload".FormatWith(messageID, filename), file.PostedFile.InputStream.ToArray());

				LegacyDb.attachment_save(messageID, filename, file.PostedFile.ContentLength, file.PostedFile.ContentType, null);
			}
		}

		#endregion
	}
}