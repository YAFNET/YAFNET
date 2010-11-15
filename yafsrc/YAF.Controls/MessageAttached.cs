/* Yet Another Forum.NET
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
namespace YAF.Controls
{
  #region Using

  using System;
  using System.Data;
  using System.Linq;
  using System.Web;
  using System.Web.UI;

  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  #endregion

  /// <summary>
  /// The message attached.
  /// </summary>
  public class MessageAttached : BaseControl
  {
    #region Constants and Fields

    /// <summary>
    ///   The _user name.
    /// </summary>
    private string _userName = string.Empty;

    #endregion

    #region Properties

    /// <summary>
    ///   Gets or sets MessageID.
    /// </summary>
    public int MessageID { get; set; }

    /// <summary>
    ///   Gets or sets UserName.
    /// </summary>
    public string UserName
    {
      get
      {
        return this._userName;
      }

      set
      {
        this._userName = value;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected override void Render(HtmlTextWriter writer)
    {
      writer.BeginRender();
      writer.WriteBeginTag("div");
      writer.WriteAttribute("id", this.ClientID);
      writer.Write(HtmlTextWriter.TagRightChar);

      if (this.MessageID != 0)
      {
        this.RenderAttachedFiles(writer);
      }

      base.Render(writer);

      writer.WriteEndTag("div");
      writer.EndRender();
    }

    /// <summary>
    /// The render attached files.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected void RenderAttachedFiles(HtmlTextWriter writer)
    {
      string[] aImageExtensions = { "jpg", "gif", "png", "bmp" };

      string stats = this.PageContext.Localization.GetText("ATTACHMENTINFO");
      string strFileIcon = this.PageContext.Theme.GetItem("ICONS", "ATTACHED_FILE");

      string attachGroupId = Guid.NewGuid().ToString().Substring(0, 5);

      HttpContext.Current.Session["imagePreviewWidth"] = this.PageContext.BoardSettings.ImageAttachmentResizeWidth;
      HttpContext.Current.Session["imagePreviewHeight"] = this.PageContext.BoardSettings.ImageAttachmentResizeHeight;
      HttpContext.Current.Session["imagePreviewCropped"] = this.PageContext.BoardSettings.ImageAttachmentResizeCropped;
      HttpContext.Current.Session["localizationFile"] = this.PageContext.Localization.LanguageFileName;

      using (DataTable attachListDT = DB.attachment_list(this.MessageID, null, null))
      {
        // show file then image attachments...
        int tmpDisplaySort = 0;

        writer.Write(@"<div class=""fileattach smallfont"">");

        while (tmpDisplaySort <= 1)
        {
          bool bFirstItem = true;

          foreach (DataRow dr in attachListDT.Rows)
          {
            string strFilename = Convert.ToString(dr["FileName"]).ToLower();
            bool bShowImage = false;

            // verify it's not too large to display
            // Ederon : 02/17/2009 - made it board setting
            if (dr["Bytes"].ToType<int>() <= this.PageContext.BoardSettings.PictureAttachmentDisplayTreshold)
            {
              // is it an image file?
              bShowImage = aImageExtensions.Any(t => strFilename.ToLower().EndsWith(t));
            }

            if (bShowImage && tmpDisplaySort == 1)
            {
              if (bFirstItem)
              {
                writer.Write(@"<div class=""imgtitle"">");
                writer.Write(
                  this.PageContext.Localization.GetText("IMAGE_ATTACHMENT_TEXT").FormatWith(
                   this.HtmlEncode(Convert.ToString(this.UserName))));
                writer.Write("</div>");
                bFirstItem = false;
              }

              // Ederon : download rights
              if (this.PageContext.ForumDownloadAccess || this.PageContext.ForumModeratorAccess)
              {
                // user has rights to download, show him image
                if (!this.PageContext.BoardSettings.EnableImageAttachmentResize)
                {
                  writer.Write(
                    @"<div class=""attachedimg""><img src=""{0}resource.ashx?a={1}"" alt=""{2}"" /></div>".FormatWith(
                      YafForumInfo.ForumClientFileRoot, dr["AttachmentID"], this.HtmlEncode(dr["FileName"])));
                }
                else
                {
                  // TommyB: Start MOD: Preview Images
                  writer.Write(
                    @"<div class=""attachedimg"" style=""display: inline;""><a rel=""lightbox-group{3}"" href=""{0}resource.ashx?i={1}"" target=""_blank"" title=""{2}""><img src=""{0}resource.ashx?p={1}"" alt=""{2}"" /></a></div>"
                      .FormatWith(
                        YafForumInfo.ForumClientFileRoot, 
                        dr["AttachmentID"], 
                        this.HtmlEncode(dr["FileName"]), 
                        attachGroupId));

                  // TommyB: End MOD: Preview Images
                }
              }
              else
              {
                int kb = (1023 + (int)dr["Bytes"]) / 1024;

                // user doesn't have rights to download, don't show the image
                writer.Write(@"<div class=""attachedfile"">");
                writer.Write(
                  @"<img border=""0"" alt="""" src=""{0}"" /> {1} <span class=""attachmentinfo"">{2}</span>".FormatWith(
                    strFileIcon, dr["FileName"], stats.FormatWith(kb, dr["Downloads"])));
                writer.Write(@"</div>");
              }
            }
            else if (!bShowImage && tmpDisplaySort == 0)
            {
              if (bFirstItem)
              {
                writer.Write(
                  @"<div class=""filetitle"">{0}</div>".FormatWith(this.PageContext.Localization.GetText("ATTACHMENTS")));
                bFirstItem = false;
              }

              // regular file attachment
              int kb = (1023 + (int)dr["Bytes"]) / 1024;

              writer.Write(@"<div class=""attachedfile"">");

              // Ederon : download rights
              if (this.PageContext.ForumDownloadAccess || this.PageContext.ForumModeratorAccess)
              {
                writer.Write(
                  @"<img border=""0"" alt="""" src=""{0}"" /> <a class=""attachedImageLink"" href=""{1}resource.ashx?a={2}"">{3}</a> <span class=""attachmentinfo"">{4}</span>"
                    .FormatWith(
                      strFileIcon, 
                      YafForumInfo.ForumClientFileRoot, 
                      dr["AttachmentID"], 
                      dr["FileName"], 
                      stats.FormatWith(kb, dr["Downloads"])));
              }
              else
              {
                writer.Write(
                  @"<img border=""0"" alt="""" src=""{0}"" /> {1} <span class=""attachmentinfo"">{2}</span>".FormatWith(
                    strFileIcon, dr["FileName"], stats.FormatWith(kb, dr["Downloads"])));
              }

              writer.Write(@"</div>");
            }
          }

          // now show images
          tmpDisplaySort++;
        }

        if (!this.PageContext.ForumDownloadAccess)
        {
          writer.Write(@"<br /><div class=""attachmentinfo"">");
          if (this.PageContext.IsGuest)
          {
            writer.Write(this.PageContext.Localization.GetText("POSTS", "CANT_DOWNLOAD_REGISTER"));
          }
          else
          {
            writer.Write(this.PageContext.Localization.GetText("POSTS", "CANT_DOWNLOAD"));
          }

          writer.Write(@"</div>");
        }

        writer.Write(@"</div>");
      }
    }

    #endregion
  }
}