/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2010 Jaben Cargman
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
  using System.Web.UI.HtmlControls;
  using System.Web.UI.WebControls;

  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  #endregion

  /// <summary>
  /// Summary description for attachments.
  /// </summary>
  public partial class attachments : ForumPage
  {
    #region Constants and Fields

    /// <summary>
    /// The _forum.
    /// </summary>
    private DataRow _forum;

    /// <summary>
    /// The _topic.
    /// </summary>
    private DataRow _topic;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="attachments"/> class.
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
    protected void Back_Click(object sender, EventArgs e)
    {
      YafBuildLink.Redirect(ForumPages.posts, "m={0}#{0}", this.Request.QueryString["m"]);
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
    protected void Delete_Load(object sender, EventArgs e)
    {
      ((LinkButton)sender).Attributes["onclick"] = String.Format("return confirm('{0}')", this.GetText("ASK_DELETE"));
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
    protected void List_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
      switch (e.CommandName)
      {
        case "delete":
          DB.attachment_delete(e.CommandArgument);
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
    protected void Page_Load(object sender, EventArgs e)
    {
      using (DataTable dt = DB.forum_list(this.PageContext.PageBoardID, this.PageContext.PageForumID)) this._forum = dt.Rows[0];
      this._topic = DB.topic_info(this.PageContext.PageTopicID);

      if (!this.IsPostBack)
      {
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
          YafBuildLink.AccessDenied( /*"The topic is closed."*/);
        }

        if (this._forum["Flags"].BinaryAnd(ForumFlags.Flags.IsLocked))
        {
          YafBuildLink.AccessDenied( /*"The forum is closed."*/);
        }

        // Check that non-moderators only edit messages they have written
        if (!this.PageContext.ForumModeratorAccess)
        {
          using (DataTable dt = DB.message_list(this.Request.QueryString["m"]))
          {
            if ((int)dt.Rows[0]["UserID"] != this.PageContext.PageUserID)
            {
              YafBuildLink.AccessDenied( /*"You didn't post this message."*/);
            }
          }
        }

        if (this.PageContext.Settings.LockedForum == 0)
        {
          this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
          this.PageLinks.AddLink(
            this.PageContext.PageCategoryName, 
            YafBuildLink.GetLink(ForumPages.forum, "c={0}", this.PageContext.PageCategoryID));
        }

        this.PageLinks.AddForumLinks(this.PageContext.PageForumID);
        this.PageLinks.AddLink(
          this.PageContext.PageTopicName, YafBuildLink.GetLink(ForumPages.posts, "t={0}", this.PageContext.PageTopicID));
        this.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);

        this.Back.Text = this.GetText("BACK");
        this.Upload.Text = this.GetText("UPLOAD");

        // MJ : 10/14/2007 - list of allowed file extensions
        DataTable extensionTable = DB.extension_list(this.PageContext.PageBoardID);

        string types = string.Empty;
        bool bFirst = true;

        foreach (DataRow row in extensionTable.Rows)
        {
          types += String.Format("{1}*.{0}", row["Extension"].ToString(), bFirst ? string.Empty : ", ");
          if (bFirst)
          {
            bFirst = false;
          }
        }

        if (!String.IsNullOrEmpty(types))
        {
          this.ExtensionsList.Text = types;
        }

        this.BindData();
      }
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
    protected void Upload_Click(object sender, EventArgs e)
    {
      try
      {
        if (this.CheckValidFile(this.File))
        {
          this.SaveAttachment(this.Request.QueryString["m"], this.File);
        }

        this.BindData();
      }
      catch (Exception x)
      {
        DB.eventlog_create(this.PageContext.PageUserID, this, x);
        this.PageContext.AddLoadMessage(x.Message);
        return;
      }
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      DataTable dt = DB.attachment_list(this.Request.QueryString["m"], null, null);
      this.List.DataSource = dt;

      this.List.Visible = (dt.Rows.Count > 0) ? true : false;

      // show disallowed or allowed localized text depending on the Board Setting
      this.ExtensionTitle.LocalizedTag = this.PageContext.BoardSettings.FileExtensionAreAllowed
                                            ? "ALLOWED_EXTENSIONS"
                                            : "DISALLOWED_EXTENSIONS";

      if (this.PageContext.BoardSettings.MaxNumberOfAttachments > 0)
      {
        if (dt.Rows.Count > (this.PageContext.BoardSettings.MaxNumberOfAttachments - 1))
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
    /// The check valid file.
    /// </returns>
    private bool CheckValidFile(HtmlInputFile uploadedFile)
    {
      string filePath = uploadedFile.PostedFile.FileName.Trim();

      if (String.IsNullOrEmpty(filePath) || uploadedFile.PostedFile.ContentLength == 0)
      {
        return false;
      }

      string extension = Path.GetExtension(filePath).ToLower();

      // remove the "period"
      extension = extension.Replace(".", string.Empty);

      // If we don't get a match from the db, then the extension is not allowed
      DataTable dt = DB.extension_list(this.PageContext.PageBoardID, extension);

      bool bInList = dt.Rows.Count > 0;
      bool bError = false;

      if (this.PageContext.BoardSettings.FileExtensionAreAllowed && !bInList)
      {
        // since it's not in the list -- it's invalid
        bError = true;
      }
      else if (!this.PageContext.BoardSettings.FileExtensionAreAllowed && bInList)
      {
        // since it's on the list -- it's invalid
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
    /// <exception cref="Exception">
    /// </exception>
    private void SaveAttachment(object messageID, HtmlInputFile file)
    {
      if (file.PostedFile == null || file.PostedFile.FileName.Trim().Length == 0 || file.PostedFile.ContentLength == 0)
      {
        return;
      }

      string previousDirectory = this.Request.MapPath(String.Concat(BaseUrlBuilder.ServerFileRoot, YafBoardFolders.Current.Uploads));
      string filename = file.PostedFile.FileName;

      int pos = filename.LastIndexOfAny(new[] { '/', '\\' });
      if (pos >= 0)
      {
        filename = filename.Substring(pos + 1);
      }

      // filename can be only 255 characters long (due to table column)
      if (filename.Length > 255)
      {
        filename = filename.Substring(filename.Length - 255);
      }

      // verify the size of the attachment
      if (this.PageContext.BoardSettings.MaxFileSize > 0 &&
          file.PostedFile.ContentLength > this.PageContext.BoardSettings.MaxFileSize)
      {
        throw new Exception(this.GetText("ERROR_TOOBIG"));
      }

      if (this.PageContext.BoardSettings.UseFileTable)
      {
        DB.attachment_save(
          messageID, filename, file.PostedFile.ContentLength, file.PostedFile.ContentType, file.PostedFile.InputStream);
      }
      else
      {
        file.PostedFile.SaveAs(String.Format("{0}/{1}.{2}.yafupload", previousDirectory, messageID, filename));
        DB.attachment_save(messageID, filename, file.PostedFile.ContentLength, file.PostedFile.ContentType, null);
      }
    }

    #endregion
  }
}