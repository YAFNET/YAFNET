/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
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

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;

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
        protected override void Render([NotNull] HtmlTextWriter writer)
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
        protected void RenderAttachedFiles([NotNull] HtmlTextWriter writer)
        {
            string[] aImageExtensions = { "jpg", "gif", "png", "bmp" };

            string stats = this.GetText("ATTACHMENTINFO");
            string strFileIcon = this.Get<ITheme>().GetItem("ICONS", "ATTACHED_FILE");

            var session = this.Get<HttpSessionStateBase>();
            var settings = this.Get<YafBoardSettings>();

            session["imagePreviewWidth"] = settings.ImageAttachmentResizeWidth;
            session["imagePreviewHeight"] = settings.ImageAttachmentResizeHeight;
            session["imagePreviewCropped"] = settings.ImageAttachmentResizeCropped;
            session["localizationFile"] = this.Get<ILocalization>().LanguageFileName;

            using (DataTable attachListDT = this.GetRepository<Attachment>().List(this.MessageID, null, null,null,null))
            {
                // show file then image attachments...
                int tmpDisplaySort = 0;

                writer.Write(@"<div class=""fileattach smallfont ceebox"">");

                while (tmpDisplaySort <= 1)
                {
                    bool bFirstItem = true;

                    foreach (DataRow dr in attachListDT.Rows)
                    {
                        string strFilename = Convert.ToString(dr["FileName"]).ToLowerInvariant();
                        bool bShowImage = false;

                        // verify it's not too large to display
                        // Ederon : 02/17/2009 - made it board setting
                        if (dr["Bytes"].ToType<int>() <= settings.PictureAttachmentDisplayTreshold)
                        {
                            // is it an image file?
                            bShowImage = aImageExtensions.Any(t => strFilename.ToLowerInvariant().EndsWith(t));
                        }

                        if (bShowImage && tmpDisplaySort == 1)
                        {
                            if (bFirstItem)
                            {
                                writer.Write(@"<div class=""imgtitle"">");

                                writer.Write(
                                    this.GetText("IMAGE_ATTACHMENT_TEXT"),
                                    this.HtmlEncode(Convert.ToString(this.UserName)));

                                writer.Write("</div>");

                                bFirstItem = false;
                            }

                            // Ederon : download rights
                            if (this.PageContext.ForumDownloadAccess || this.PageContext.ForumModeratorAccess)
                            {
                                // user has rights to download, show him image
                                if (!settings.EnableImageAttachmentResize)
                                {
                                    writer.Write(
                                        @"<div class=""attachedimg""><img src=""{0}resource.ashx?a={1}"" alt=""{2}"" /></div>",
                                        YafForumInfo.ForumClientFileRoot,
                                        dr["AttachmentID"],
                                        this.HtmlEncode(dr["FileName"]));
                                }
                                else
                                {
                                    var attachFilesText =
                                        "{0} {1}".FormatWith(
                                            this.GetText("IMAGE_ATTACHMENT_TEXT").FormatWith(
                                                this.HtmlEncode(Convert.ToString(this.UserName))),
                                            this.HtmlEncode(dr["FileName"]));

                                    // TommyB: Start MOD: Preview Images
                                    writer.Write(
                                        @"<div class=""attachedimg"" style=""display: inline;""><a href=""{0}resource.ashx?i={1}"" title=""{2}"" title=""{2}"" date-img=""{0}resource.ashx?a={1}""><img src=""{0}resource.ashx?p={1}"" alt=""{3}"" title=""{2}"" /></a></div>",
                                        YafForumInfo.ForumClientFileRoot,
                                        dr["AttachmentID"],
                                        attachFilesText,
                                        this.HtmlEncode(dr["FileName"]));

                                    // TommyB: End MOD: Preview Images
                                }
                            }
                            else
                            {
                                int kb = (1023 + (int)dr["Bytes"]) / 1024;

                                // user doesn't have rights to download, don't show the image
                                writer.Write(@"<div class=""attachedfile"">");

                                writer.Write(
                                    @"<img border=""0"" alt="""" src=""{0}"" /> {1} <span class=""attachmentinfo"">{2}</span>",
                                    strFileIcon,
                                    dr["FileName"],
                                    stats.FormatWith(kb, dr["Downloads"]));

                                writer.Write(@"</div>");
                            }
                        }
                        else if (!bShowImage && tmpDisplaySort == 0)
                        {
                            if (bFirstItem)
                            {
                                writer.Write(@"<div class=""filetitle"">{0}</div>", this.GetText("ATTACHMENTS"));
                                bFirstItem = false;
                            }

                            // regular file attachment
                            int kb = (1023 + (int)dr["Bytes"]) / 1024;

                            writer.Write(@"<div class=""attachedfile"">");

                            // Ederon : download rights
                            if (this.PageContext.ForumDownloadAccess || this.PageContext.ForumModeratorAccess)
                            {
                                writer.Write(
                                    @"<img border=""0"" alt="""" src=""{0}"" /> <a class=""attachedImageLink {{html:false,image:false,video:false}}"" href=""{1}resource.ashx?a={2}"">{3}</a> <span class=""attachmentinfo"">{4}</span>",
                                    strFileIcon,
                                    YafForumInfo.ForumClientFileRoot,
                                    dr["AttachmentID"],
                                    dr["FileName"],
                                    stats.FormatWith(kb, dr["Downloads"]));
                            }
                            else
                            {
                                writer.Write(
                                    @"<img border=""0"" alt="""" src=""{0}"" /> {1} <span class=""attachmentinfo"">{2}</span>",
                                    strFileIcon,
                                    dr["FileName"],
                                    stats.FormatWith(kb, dr["Downloads"]));
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

                    writer.Write(
                        this.PageContext.IsGuest
                            ? this.GetText("POSTS", "CANT_DOWNLOAD_REGISTER")
                            : this.GetText("POSTS", "CANT_DOWNLOAD"));

                    writer.Write(@"</div>");
                }

                writer.Write(@"</div>");
            }
        }

        #endregion
    }
}