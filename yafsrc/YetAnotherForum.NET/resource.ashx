<%@ WebHandler Language="C#" Class="YAF.resource" %>
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
namespace YAF
{
  #region Using

  using System;
  using System.Data;
  using System.Drawing;
  using System.Drawing.Drawing2D;
  using System.Drawing.Imaging;
  using System.Drawing.Text;
  using System.IO;
  using System.Net;
  using System.Web;
  using System.Web.Security;
  using System.Web.Services;
  using System.Web.SessionState;

  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.UI;
  using YAF.Classes.Utils;

  #endregion

  /// <summary>
  /// Summary description for YAF Resource Handler
  /// </summary>
  [WebService(Namespace = "http://www.yetanotherforum.net/")]
  [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
  public class resource : IHttpHandler, IReadOnlySessionState
  {
    #region Properties

    /// <summary>
    /// Gets a value indicating whether IsReusable.
    /// </summary>
    public bool IsReusable
    {
      get
      {
        return false;
      }
    }

    #endregion

    #region Implemented Interfaces

    #region IHttpHandler

    /// <summary>
    /// The process request.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    public void ProcessRequest(HttpContext context)
    {
      // resource no longer works with dynamic compile...
      if (context.Request.QueryString["r"] != null)
      {
        // resource request
        this.GetResource(context);
      }
      else if (context.Session["lastvisit"] != null)
      {
        if (context.Request.QueryString["u"] != null)
        {
          this.GetResponseLocalAvatar(context);
        }
        else if (context.Request.QueryString["url"] != null && context.Request.QueryString["width"] != null &&
                 context.Request.QueryString["height"] != null)
        {
          this.GetResponseRemoteAvatar(context);
        }
        else if (context.Request.QueryString["a"] != null)
        {
          this.GetResponseAttachment(context);
        }
          
          // TommyB: Start MOD: Preview Images   ##########
        else if (context.Request.QueryString["i"] != null)
        {
          this.GetResponseImage(context);
        }
        else if (context.Request.QueryString["p"] != null)
        {
          this.GetResponseImagePreview(context);
        }
          
          // TommyB: End MOD: Preview Images   ##########
        else if (context.Request.QueryString["c"] != null && context.Session["CaptchaImageText"] != null)
        {
          // captcha					
          this.GetResponseCaptcha(context);
        }
        else if (context.Request.QueryString["cover"] != null && context.Request.QueryString["album"] != null)
        {
          // album cover		
          this.GetAlbumCover(context);
        }
        else if (context.Request.QueryString["imgprv"] != null)
        {
          // album image preview		
          this.GetAlbumImagePreview(context);
        }
        else if (context.Request.QueryString["image"] != null)
        {
          // album image		
          this.GetAlbumImage(context);
        }
        else if (context.Request.QueryString["s"] != null && context.Request.QueryString["lang"] != null)
        {
          this.GetResponseGoogleSpell(context);
        }
      }
      else
      {
        // they don't have a session...
        context.Response.Write("Please do not link directly to this resource. You must have a session in the forum.");
      }
    }

    #endregion

    #endregion

    #region Methods

    /// <summary>
    /// Check if the ETag that sent from the client is match to the current ETag.
    /// If so, set the status code to 'Not Modified' and stop the response.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    /// <param name="eTagCode">
    /// The e Tag Code.
    /// </param>
    /// <returns>
    /// The check e tag.
    /// </returns>
    private static bool CheckETag(HttpContext context, string eTagCode)
    {
      string ifNoneMatch = context.Request.Headers["If-None-Match"];
      if (eTagCode.Equals(ifNoneMatch, StringComparison.Ordinal))
      {
        context.Response.AppendHeader("Content-Length", "0");
        context.Response.StatusCode = (int)HttpStatusCode.NotModified;
        context.Response.StatusDescription = "Not modified";
        context.Response.SuppressContent = true;
        context.Response.Cache.SetCacheability(HttpCacheability.Public);
        context.Response.Cache.SetETag(eTagCode);
        context.Response.Flush();
        return true;
      }

      return false;
    }

    /// <summary>
    /// The check access rights.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="messageID">
    /// The message id.
    /// </param>
    /// <returns>
    /// The check access rights.
    /// </returns>
    private bool CheckAccessRights(object boardID, object messageID)
    {
      // Find user name
      MembershipUser user = Membership.GetUser();

      string browser = String.Format(
        "{0} {1}", HttpContext.Current.Request.Browser.Browser, HttpContext.Current.Request.Browser.Version);
      string platform = HttpContext.Current.Request.Browser.Platform;
      bool isSearchEngine = false;
      bool isIgnoredForDisplay = false;
      string userAgent = HttpContext.Current.Request.UserAgent;

      if (!string.IsNullOrEmpty(userAgent))
      {
          if (userAgent.IndexOf("Windows NT 5.2") >= 0)
        {
          platform = "Win2003";
        }
          else if (userAgent.IndexOf("Windows NT 6.0") >= 0)
        {
          platform = "Vista";
        }
          else if (userAgent.IndexOf("Windows NT 6.1") >= 0)
        {
            platform = "Win7";
        }
          else if (userAgent.IndexOf("Linux") >= 0)
        {
            platform = "Linux";
        }
          else if (userAgent.IndexOf("FreeBSD") >= 0)
        {
            platform = "FreeBSD";
        }
        else
        {
            // check if it's a search engine spider or an ignored UI string...
            isSearchEngine = !HttpContext.Current.Request.Browser.Crawler ? UserAgentHelper.IsSearchEngineSpider(userAgent) : true;
            isIgnoredForDisplay = UserAgentHelper.IsIgnoredForDisplay(userAgent) | isSearchEngine;
          }
      }
      else
      {
          
          // It'll show that no UserAgent string was received. 
          platform = "?";
      }

      YafServices.InitializeDb.Run();

      object userKey = DBNull.Value;

      if (user != null)
      {
        userKey = user.ProviderUserKey;
      }

      DataRow pageRow = DB.pageload(
        HttpContext.Current.Session.SessionID, 
        boardID, 
        userKey, 
        HttpContext.Current.Request.UserHostAddress, 
        HttpContext.Current.Request.FilePath, 
        HttpContext.Current.Request.QueryString.ToString(), 
        browser, 
        platform, 
        null, 
        null, 
        null, 
        messageID, 
        // don't track if this is a search engine
        isIgnoredForDisplay, 
        YafContext.Current.BoardSettings.EnableBuddyList, 
        YafContext.Current.BoardSettings.AllowPrivateMessages, 
        YafContext.Current.BoardSettings.UseStyledNicks);

      return General.BinaryAnd(pageRow["DownloadAccess"], AccessFlags.Flags.DownloadAccess) ||
             General.BinaryAnd(pageRow["ModeratorAccess"], AccessFlags.Flags.ModeratorAccess);
    }

    /// <summary>
    /// The copy stream.
    /// </summary>
    /// <param name="input">
    /// The input.
    /// </param>
    /// <param name="output">
    /// The output.
    /// </param>
    private void CopyStream(Stream input, Stream output)
    {
      var buffer = new byte[1024];
      int count = buffer.Length;

      while (count > 0)
      {
        count = input.Read(buffer, 0, count);
        if (count > 0)
        {
          output.Write(buffer, 0, count);
        }
      }
    }

    /// <summary>
    /// The get album cover.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    private void GetAlbumCover(HttpContext context)
    {
      // default is 200x200
      int previewMaxWidth = 200;
      int previewMaxHeight = 200;
      string localizationFile = "english.xml";

      if (context.Session["imagePreviewWidth"] != null && context.Session["imagePreviewWidth"] is int)
      {
        previewMaxWidth = (int)context.Session["imagePreviewWidth"];
      }

      if (context.Session["imagePreviewHeight"] != null && context.Session["imagePreviewHeight"] is int)
      {
        previewMaxHeight = (int)context.Session["imagePreviewHeight"];
      }

      if (context.Session["localizationFile"] != null && context.Session["localizationFile"] is string)
      {
        localizationFile = context.Session["localizationFile"].ToString();
      }

      string eTag = String.Format(
        @"""{0}""", context.Request.QueryString["cover"] + localizationFile.GetHashCode().ToString());

      if (CheckETag(context, eTag))
      {
        // found eTag... no need to resend/create this image...
        return;
      }

      try
      {
        {
          // CoverID
          string fileName = string.Empty;
          var data = new MemoryStream();
          if (context.Request.QueryString["cover"] == "0")
          {
            fileName = context.Server.MapPath(String.Format("{0}/images/{1}", YafForumInfo.ForumClientFileRoot, "noCover.png"));
          }
          else
          {
            using (DataTable dt = DB.album_image_list(null, context.Request.QueryString["cover"]))
            {
              if (dt.Rows.Count > 0)
              {
                DataRow row = dt.Rows[0];
                string sUpDir = YafBoardFolders.Current.Uploads;

                string oldFileName =
                  context.Server.MapPath(
                    String.Format("{0}/{1}.{2}.{3}", sUpDir, row["UserID"], row["AlbumID"], row["FileName"]));
                string newFileName =
                  context.Server.MapPath(
                    String.Format("{0}/{1}.{2}.{3}.yafalbum", sUpDir, row["UserID"], row["AlbumID"], row["FileName"]));

                // use the new fileName (with extension) if it exists...
                fileName = File.Exists(newFileName) ? newFileName : oldFileName;
              }
            }
          }

          using (var input = new FileStream(fileName, FileMode.Open))
          {
            var buffer = new byte[input.Length];
            input.Read(buffer, 0, buffer.Length);
            data.Write(buffer, 0, buffer.Length);
            input.Close();
          }

          // reset position...
          data.Position = 0;
          string imagesNumber = DB.album_getstats(null, context.Request.QueryString["album"])[1].ToString();
          MemoryStream ms = this.GetCoverResized(
            data, previewMaxWidth, previewMaxHeight, localizationFile, imagesNumber);

          context.Response.ContentType = "image/png";

          // output stream...
          context.Response.OutputStream.Write(ms.ToArray(), 0, (int)ms.Length);
          context.Response.Cache.SetCacheability(HttpCacheability.Public);
          context.Response.Cache.SetExpires(DateTime.UtcNow.AddHours(2));
          context.Response.Cache.SetETag(eTag);

          data.Dispose();
          ms.Dispose();
        }
      }
      catch (Exception x)
      {
        DB.eventlog_create(null, this.GetType().ToString(), x, 1);
        context.Response.Write("Error: Resource has been moved or is unavailable. Please contact the forum admin.");
      }
    }

    /// <summary>
    /// The get album image.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    private void GetAlbumImage(HttpContext context)
    {
      try
      {
        string eTag = String.Format(@"""{0}""", context.Request.QueryString["image"]);

        if (CheckETag(context, eTag))
        {
          // found eTag... no need to resend/create this image -- just mark another view?
          // YAF.Classes.Data.DB.album_image_download(context.Request.QueryString["image"]);
          return;
        }

        // ImageID
        using (DataTable dt = DB.album_image_list(null, context.Request.QueryString["image"]))
        {
          foreach (DataRow row in dt.Rows)
          {
            byte[] data = null;

            string sUpDir = YafBoardFolders.Current.Uploads;

            string oldFileName =
              context.Server.MapPath(
                String.Format("{0}/{1}.{2}.{3}", sUpDir, row["UserID"], row["AlbumID"], row["FileName"]));
            string newFileName =
              context.Server.MapPath(
                String.Format("{0}/{1}.{2}.{3}.yafalbum", sUpDir, row["UserID"], row["AlbumID"], row["FileName"]));

            string fileName = string.Empty;

            // use the new fileName (with extension) if it exists...
            if (File.Exists(newFileName))
            {
              fileName = newFileName;
            }
            else
            {
              fileName = oldFileName;
            }

            using (var input = new FileStream(fileName, FileMode.Open))
            {
              data = new byte[input.Length];
              input.Read(data, 0, data.Length);
              input.Close();
            }

            context.Response.ContentType = row["ContentType"].ToString();

            // context.Response.Cache.SetCacheability(HttpCacheability.Public);
            // context.Response.Cache.SetETag(eTag);
            context.Response.OutputStream.Write(data, 0, data.Length);

            // add a download count...
            DB.album_image_download(context.Request.QueryString["image"]);
            break;
          }
        }
      }
      catch (Exception x)
      {
        DB.eventlog_create(null, this.GetType().ToString(), x, 1);
        context.Response.Write("Error: Resource has been moved or is unavailable. Please contact the forum admin.");
      }
    }

    /// <summary>
    /// The get album image preview.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    private void GetAlbumImagePreview(HttpContext context)
    {
      // default is 200x200
      int previewMaxWidth = 200;
      int previewMaxHeight = 200;
      string localizationFile = "english.xml";

      if (context.Session["imagePreviewWidth"] != null && context.Session["imagePreviewWidth"] is int)
      {
        previewMaxWidth = (int)context.Session["imagePreviewWidth"];
      }

      if (context.Session["imagePreviewHeight"] != null && context.Session["imagePreviewHeight"] is int)
      {
        previewMaxHeight = (int)context.Session["imagePreviewHeight"];
      }

      if (context.Session["localizationFile"] != null && context.Session["localizationFile"] is string)
      {
        localizationFile = context.Session["localizationFile"].ToString();
      }

      string eTag = String.Format(
        @"""{0}""", context.Request.QueryString["imgprv"] + localizationFile.GetHashCode().ToString());

      if (CheckETag(context, eTag))
      {
        // found eTag... no need to resend/create this image...
        return;
      }

      try
      {
        // ImageID
        using (DataTable dt = DB.album_image_list(null, context.Request.QueryString["imgprv"]))
        {
          foreach (DataRow row in dt.Rows)
          {
            var data = new MemoryStream();

            string sUpDir = YafBoardFolders.Current.Uploads;

            string oldFileName =
              context.Server.MapPath(
                String.Format("{0}/{1}.{2}.{3}", sUpDir, row["UserID"], row["AlbumID"], row["FileName"]));
            string newFileName =
              context.Server.MapPath(
                String.Format("{0}/{1}.{2}.{3}.yafalbum", sUpDir, row["UserID"], row["AlbumID"], row["FileName"]));

            string fileName = string.Empty;

            // use the new fileName (with extension) if it exists...
            if (File.Exists(newFileName))
            {
              fileName = newFileName;
            }
            else
            {
              fileName = oldFileName;
            }

            using (var input = new FileStream(fileName, FileMode.Open))
            {
              var buffer = new byte[input.Length];
              input.Read(buffer, 0, buffer.Length);
              data.Write(buffer, 0, buffer.Length);
              input.Close();
            }

            // reset position...
            data.Position = 0;

            MemoryStream ms = this.GetImageResized(
              data, previewMaxWidth, previewMaxHeight, (int)row["Downloads"], localizationFile);

            context.Response.ContentType = "image/png";

            // output stream...
            context.Response.OutputStream.Write(ms.ToArray(), 0, (int)ms.Length);
            context.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.Response.Cache.SetExpires(DateTime.UtcNow.AddHours(2));
            context.Response.Cache.SetETag(eTag);

            data.Dispose();
            ms.Dispose();

            break;
          }
        }
      }
      catch (Exception x)
      {
        DB.eventlog_create(null, this.GetType().ToString(), x, 1);
        context.Response.Write("Error: Resource has been moved or is unavailable. Please contact the forum admin.");
      }
    }

    /// <summary>
    /// The get cover resized.
    /// </summary>
    /// <param name="data">
    /// The data.
    /// </param>
    /// <param name="previewWidth">
    /// The preview width.
    /// </param>
    /// <param name="previewHeight">
    /// The preview height.
    /// </param>
    /// <param name="localizationFile">
    /// The localization file.
    /// </param>
    /// <param name="ImagesNumber">
    /// The images number.
    /// </param>
    /// <returns>
    /// </returns>
    private MemoryStream GetCoverResized(
      MemoryStream data, int previewWidth, int previewHeight, string localizationFile, string ImagesNumber)
    {
      const int paddingY = 6;
      const int paddingX = 6;
      const int bottomSize = 13;
      const int topSize = 0;
      using (var src = new Bitmap(data))
      {
        // default to width-based resizing...
        int width = previewWidth;
        var height = (int)(previewWidth / ((double)src.Width / (double)src.Height));

        if (src.Width <= previewWidth && src.Height <= previewHeight)
        {
          // no resizing necessary...
          width = src.Width;
          height = src.Height;
        }
        else if (height > previewHeight)
        {
          // aspect is based on the height, not the width...
          width = (int)(previewHeight / ((double)src.Height / (double)src.Width));
          height = previewHeight;
        }

        using (
          var dst = new Bitmap(width + paddingX, height + topSize + bottomSize + paddingY, PixelFormat.Format24bppRgb))
        {
          var rSrcImg = new Rectangle(0, 0, src.Width, src.Height);
          var rDstImg = new Rectangle(
            3, 3 + topSize, dst.Width - paddingX, dst.Height - topSize - paddingY - bottomSize);
          var rDstTxt = new Rectangle(3, rDstImg.Height + 3 + topSize, previewWidth, bottomSize);

          // Rectangle rDstTitle = new Rectangle(3, 3, previewWidth, topSize);
          using (Graphics g = Graphics.FromImage(dst))
          {
            g.Clear(Color.FromArgb(64, 64, 64));
            g.FillRectangle(Brushes.White, rDstImg);

            g.CompositingMode = CompositingMode.SourceOver;
            g.CompositingQuality = CompositingQuality.GammaCorrected;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.DrawImage(src, rDstImg, rSrcImg, GraphicsUnit.Pixel);

            using (var f = new Font("Arial", 10, FontStyle.Regular, GraphicsUnit.Pixel))
            {
              using (var brush = new SolidBrush(Color.FromArgb(191, 191, 191)))
              {
                var localization = new YafLocalization("ALBUM");
                localization.LoadTranslation(localizationFile);
                var sf = new StringFormat();

                sf.Alignment = StringAlignment.Far;
                sf.LineAlignment = StringAlignment.Near;
                g.DrawString(localization.GetText("ALBUM_VIEW"), f, brush, rDstTxt, sf);

                sf.Alignment = StringAlignment.Near;
                sf.LineAlignment = StringAlignment.Near;
                g.DrawString(
                  String.Format(localization.GetText("ALBUM_IMAGES_NUMBER"), ImagesNumber), f, brush, rDstTxt, sf);
              }
            }
          }

          var ms = new MemoryStream();

          // save the bitmap to the stream...
          dst.Save(ms, ImageFormat.Png);
          ms.Position = 0;

          return ms;
        }
      }
    }

    /// <summary>
    /// The get image resized.
    /// </summary>
    /// <param name="data">
    /// The data.
    /// </param>
    /// <param name="previewWidth">
    /// The preview width.
    /// </param>
    /// <param name="previewHeight">
    /// The preview height.
    /// </param>
    /// <param name="downloads">
    /// The downloads.
    /// </param>
    /// <param name="localizationFile">
    /// The localization file.
    /// </param>
    /// <returns>
    /// </returns>
    private MemoryStream GetImageResized(
      MemoryStream data, int previewWidth, int previewHeight, int downloads, string localizationFile)
    {
      const int pixelPadding = 6;
      const int bottomSize = 13;

      using (var src = new Bitmap(data))
      {
        // default to width-based resizing...
        int width = previewWidth;
        var height = (int)(previewWidth / ((double)src.Width / (double)src.Height));

        if (src.Width <= previewWidth && src.Height <= previewHeight)
        {
          // no resizing necessary...
          width = src.Width;
          height = src.Height;
        }
        else if (height > previewHeight)
        {
          // aspect is based on the height, not the width...
          width = (int)(previewHeight / ((double)src.Height / (double)src.Width));
          height = previewHeight;
        }

        using (
          var dst = new Bitmap(width + pixelPadding, height + bottomSize + pixelPadding, PixelFormat.Format24bppRgb))
        {
          var rSrcImg = new Rectangle(0, 0, src.Width, src.Height);
          var rDstImg = new Rectangle(3, 3, dst.Width - pixelPadding, dst.Height - pixelPadding - bottomSize);
          var rDstTxt = new Rectangle(3, rDstImg.Height + 3, previewWidth, bottomSize);
          using (Graphics g = Graphics.FromImage(dst))
          {
            g.Clear(Color.FromArgb(64, 64, 64));
            g.FillRectangle(Brushes.White, rDstImg);

            g.CompositingMode = CompositingMode.SourceOver;
            g.CompositingQuality = CompositingQuality.GammaCorrected;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.DrawImage(src, rDstImg, rSrcImg, GraphicsUnit.Pixel);

            using (var f = new Font("Arial", 10, FontStyle.Regular, GraphicsUnit.Pixel))
            {
              using (var brush = new SolidBrush(Color.FromArgb(191, 191, 191)))
              {
                var localization = new YafLocalization("POSTS");
                localization.LoadTranslation(localizationFile);

                var sf = new StringFormat();

                sf.Alignment = StringAlignment.Near;
                sf.LineAlignment = StringAlignment.Center;
                g.DrawString(localization.GetText("IMAGE_RESIZE_ENLARGE"), f, brush, rDstTxt, sf);

                sf.Alignment = StringAlignment.Far;
                g.DrawString(
                  string.Format(localization.GetText("IMAGE_RESIZE_VIEWS"), downloads.ToString()), f, brush, rDstTxt, sf);
              }
            }
          }

          var ms = new MemoryStream();

          // save the bitmap to the stream...
          dst.Save(ms, ImageFormat.Png);
          ms.Position = 0;

          return ms;
        }
      }
    }

    /// <summary>
    /// The get resource.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    private void GetResource(HttpContext context)
    {
      // redirect to the resource?
      context.Response.Redirect("resources/" + context.Request.QueryString["r"]);

      /*string resourceName = "YAF.App_GlobalResources." + context.Request.QueryString ["r"];
			int lastIndex = resourceName.LastIndexOf( '.' );
			string extension = resourceName.Substring( lastIndex, resourceName.Length - lastIndex ).ToLower();

			string resourceType = "text/plain";

			switch ( extension )
			{
				case ".js":
					resourceType = "application/x-javascript";
					break;
				case ".css":
					resourceType = "text/css";
					break;
			}

			if ( resourceType != string.Empty )
			{
				context.Response.Clear();
				context.Response.ContentType = resourceType ;

				try
				{
					// attempt to load the resource...
					byte [] data = null;

					Stream input = this.GetType().Assembly.GetManifestResourceStream( resourceName );

					data = new byte [input.Length];
					input.Read( data, 0, data.Length );
					input.Close();

					context.Response.OutputStream.Write( data, 0, data.Length );
				}
				catch
				{
					YAF.Classes.Data.DB.eventlog_create( null, this.GetType().ToString(), "Attempting to access invalid resource: " + resourceName, 1 );
					context.Response.Write( "Error: Invalid forum resource. Please contact the forum admin." );
				}
			}
			*/
    }

    /// <summary>
    /// The get response attachment.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    private void GetResponseAttachment(HttpContext context)
    {
      try
      {
        // AttachmentID
        using (DataTable dt = DB.attachment_list(null, context.Request.QueryString["a"], null))
        {
          foreach (DataRow row in dt.Rows)
          {
            // TODO : check download permissions here					
            if (!this.CheckAccessRights(row["BoardID"], row["MessageID"]))
            {
              // tear it down
              // no permission to download
              context.Response.Write(
                "You have insufficient rights to download this resource. Contact forum administrator for further details.");
              return;
            }

            byte[] data = null;

            if (row.IsNull("FileData"))
            {
              string sUpDir = YafBoardFolders.Current.Uploads;

              string oldFileName =
                context.Server.MapPath(String.Format("{0}/{1}.{2}", sUpDir, row["MessageID"], row["FileName"]));
              string newFileName =
                context.Server.MapPath(
                  String.Format("{0}/{1}.{2}.yafupload", sUpDir, row["MessageID"], row["FileName"]));

              string fileName = string.Empty;

              // use the new fileName (with extension) if it exists...
              if (File.Exists(newFileName))
              {
                fileName = newFileName;
              }
              else
              {
                fileName = oldFileName;
              }

              using (var input = new FileStream(fileName, FileMode.Open))
              {
                data = new byte[input.Length];
                input.Read(data, 0, data.Length);
                input.Close();
              }
            }
            else
            {
              data = (byte[])row["FileData"];
            }

            context.Response.ContentType = row["ContentType"].ToString();
            context.Response.AppendHeader(
              "Content-Disposition", 
              String.Format(
                "attachment; filename={0}", HttpUtility.UrlEncode(row["FileName"].ToString()).Replace("+", "%20")));
            context.Response.OutputStream.Write(data, 0, data.Length);
            DB.attachment_download(context.Request.QueryString["a"]);
            break;
          }
        }
      }
      catch (Exception x)
      {
        DB.eventlog_create(null, this.GetType().ToString(), x, 1);
        context.Response.Write("Error: Resource has been moved or is unavailable. Please contact the forum admin.");
      }
    }

    /// <summary>
    /// The get response captcha.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    private void GetResponseCaptcha(HttpContext context)
    {
      try
      {
        var captchaImage = new CaptchaImage(
          context.Session["CaptchaImageText"].ToString(), 200, 50, "Century Schoolbook");
        context.Response.Clear();
        context.Response.ContentType = "image/jpeg";
        captchaImage.Image.Save(context.Response.OutputStream, ImageFormat.Jpeg);
      }
      catch (Exception x)
      {
        DB.eventlog_create(null, this.GetType().ToString(), x, 1);
        context.Response.Write("Error: Resource has been moved or is unavailable. Please contact the forum admin.");
      }
    }

    /// <summary>
    /// The get response google spell.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    private void GetResponseGoogleSpell(HttpContext context)
    {
      string url = string.Format("https://www.google.com/tbproxy/spell?lang={0}", context.Request.QueryString["lang"]);

      var webRequest = (HttpWebRequest)WebRequest.Create(url);
      webRequest.KeepAlive = true;
      webRequest.Timeout = 100000;
      webRequest.Method = "POST";
      webRequest.ContentType = "application/x-www-form-urlencoded";
      webRequest.ContentLength = context.Request.InputStream.Length;

      Stream requestStream = webRequest.GetRequestStream();

      this.CopyStream(context.Request.InputStream, requestStream);

      requestStream.Close();

      var httpWebResponse = (HttpWebResponse)webRequest.GetResponse();
      Stream responseStream = httpWebResponse.GetResponseStream();

      this.CopyStream(responseStream, context.Response.OutputStream);
    }

    // TommyB: Start MOD: PreviewImages   ##########
    /// <summary>
    /// The get response image.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    private void GetResponseImage(HttpContext context)
    {
      try
      {
        string eTag = String.Format(@"""{0}""", context.Request.QueryString["i"]);

        if (CheckETag(context, eTag))
        {
          // found eTag... no need to resend/create this image -- just mark another view?
          DB.attachment_download(context.Request.QueryString["i"]);
          return;
        }

        // AttachmentID
        using (DataTable dt = DB.attachment_list(null, context.Request.QueryString["i"], null))
        {
          foreach (DataRow row in dt.Rows)
          {
            // TODO : check download permissions here					
            if (!this.CheckAccessRights(row["BoardID"], row["MessageID"]))
            {
              // tear it down
              // no permission to download
              context.Response.Write(
                "You have insufficient rights to download this resource. Contact forum administrator for further details.");
              return;
            }

            byte[] data = null;

            if (row.IsNull("FileData"))
            {
              string sUpDir = YafBoardFolders.Current.Uploads;

              string oldFileName =
                context.Server.MapPath(String.Format("{0}/{1}.{2}", sUpDir, row["MessageID"], row["FileName"]));
              string newFileName =
                context.Server.MapPath(
                  String.Format("{0}/{1}.{2}.yafupload", sUpDir, row["MessageID"], row["FileName"]));

              string fileName = string.Empty;

              // use the new fileName (with extension) if it exists...
              if (File.Exists(newFileName))
              {
                fileName = newFileName;
              }
              else
              {
                fileName = oldFileName;
              }

              using (var input = new FileStream(fileName, FileMode.Open))
              {
                data = new byte[input.Length];
                input.Read(data, 0, data.Length);
                input.Close();
              }
            }
            else
            {
              data = (byte[])row["FileData"];
            }

            context.Response.ContentType = row["ContentType"].ToString();
            context.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.Response.Cache.SetETag(eTag);
            context.Response.OutputStream.Write(data, 0, data.Length);

            // add a download count...
            DB.attachment_download(context.Request.QueryString["i"]);
            break;
          }
        }
      }
      catch (Exception x)
      {
        DB.eventlog_create(null, this.GetType().ToString(), x, 1);
        context.Response.Write("Error: Resource has been moved or is unavailable. Please contact the forum admin.");
      }
    }

    /// <summary>
    /// The get response image preview.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    private void GetResponseImagePreview(HttpContext context)
    {
      // default is 200x200
      int previewMaxWidth = 200;
      int previewMaxHeight = 200;
      string localizationFile = "english.xml";

      if (context.Session["imagePreviewWidth"] != null && context.Session["imagePreviewWidth"] is int)
      {
        previewMaxWidth = (int)context.Session["imagePreviewWidth"];
      }

      if (context.Session["imagePreviewHeight"] != null && context.Session["imagePreviewHeight"] is int)
      {
        previewMaxHeight = (int)context.Session["imagePreviewHeight"];
      }

      if (context.Session["localizationFile"] != null && context.Session["localizationFile"] is string)
      {
        localizationFile = context.Session["localizationFile"].ToString();
      }

      string eTag = String.Format(
        @"""{0}""", context.Request.QueryString["p"] + localizationFile.GetHashCode().ToString());

      if (CheckETag(context, eTag))
      {
        // found eTag... no need to resend/create this image...
        return;
      }

      try
      {
        // AttachmentID
        using (DataTable dt = DB.attachment_list(null, context.Request.QueryString["p"], null))
        {
          foreach (DataRow row in dt.Rows)
          {
            // TODO : check download permissions here					
            if (!this.CheckAccessRights(row["BoardID"], row["MessageID"]))
            {
              // tear it down
              // no permission to download
              context.Response.Write(
                "You have insufficient rights to download this resource. Contact forum administrator for further details.");
              return;
            }

            var data = new MemoryStream();

            if (row.IsNull("FileData"))
            {
              string sUpDir = YafBoardFolders.Current.Uploads;

              string oldFileName =
                context.Server.MapPath(String.Format("{0}/{1}.{2}", sUpDir, row["MessageID"], row["FileName"]));
              string newFileName =
                context.Server.MapPath(
                  String.Format("{0}/{1}.{2}.yafupload", sUpDir, row["MessageID"], row["FileName"]));

              string fileName = string.Empty;

              // use the new fileName (with extension) if it exists...
              if (File.Exists(newFileName))
              {
                fileName = newFileName;
              }
              else
              {
                fileName = oldFileName;
              }

              using (var input = new FileStream(fileName, FileMode.Open))
              {
                var buffer = new byte[input.Length];
                input.Read(buffer, 0, buffer.Length);
                data.Write(buffer, 0, buffer.Length);
                input.Close();
              }
            }
            else
            {
              var buffer = (byte[])row["FileData"];
              data.Write(buffer, 0, buffer.Length);
            }

            // reset position...
            data.Position = 0;

            MemoryStream ms = this.GetImageResized(
              data, previewMaxWidth, previewMaxHeight, (int)row["Downloads"], localizationFile);

            context.Response.ContentType = "image/png";

            // output stream...
            context.Response.OutputStream.Write(ms.ToArray(), 0, (int)ms.Length);
            context.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.Response.Cache.SetExpires(DateTime.UtcNow.AddHours(2));
            context.Response.Cache.SetETag(eTag);

            data.Dispose();
            ms.Dispose();

            break;
          }
        }
      }
      catch (Exception x)
      {
        DB.eventlog_create(null, this.GetType().ToString(), x, 1);
        context.Response.Write("Error: Resource has been moved or is unavailable. Please contact the forum admin.");
      }
    }

    /// <summary>
    /// The get response local avatar.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    private void GetResponseLocalAvatar(HttpContext context)
    {
      try
      {
        // string eTag = String.Format( @"""{0}""", context.Request.QueryString["u"] );

        // if ( CheckETag( context, eTag ) )
        // {
        // found eTag... no need to resend/create this image -- just mark another view?
        // return;
        // }

        using (DataTable dt = DB.user_avatarimage(context.Request.QueryString["u"]))
        {
          foreach (DataRow row in dt.Rows)
          {
            var data = (byte[])row["AvatarImage"];
            string contentType = row["AvatarImageType"].ToString();

            context.Response.Clear();
            if (String.IsNullOrEmpty(contentType))
            {
              contentType = "image/jpeg";
            }

            context.Response.ContentType = contentType;
            context.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.Response.Cache.SetExpires(DateTime.UtcNow.AddHours(2));
            // context.Response.Cache.SetETag( eTag );
            context.Response.OutputStream.Write(data, 0, data.Length);
            break;
          }
        }
      }
      catch (Exception)
      {
      }
    }

    /// <summary>
    /// The get response remote avatar.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    private void GetResponseRemoteAvatar(HttpContext context)
    {
      var wc = new WebClient();
      Image img = null;
      Bitmap bmp = null;
      Graphics gfx = null;

      if (General.GetCurrentTrustLevel() < AspNetHostingPermissionLevel.High)
      {
        // don't bother... not supported.
        DB.eventlog_create(
          null, 
          this.GetType().ToString(), 
          "Remote Avatar is NOT supported on your Hosting Permission Level (must be High)", 
          0);
        return;
      }

      string wb = context.Request.QueryString["url"];

      try
      {
        int maxwidth = int.Parse(context.Request.QueryString["width"]);
        int maxheight = int.Parse(context.Request.QueryString["height"]);

        string eTag = String.Format(
          @"""{0}""", (context.Request.QueryString["url"] + maxheight.ToString() + maxwidth.ToString()).GetHashCode());

        if (CheckETag(context, eTag))
        {
          // found eTag... no need to download this image...
          return;
        }

        Stream input = wc.OpenRead(wb);
        img = new Bitmap(input);
        input.Close();
        int width = img.Width;
        int height = img.Height;

        if (width <= maxwidth && height <= maxheight)
        {
          context.Response.Redirect(wb);
        }

        if (width > maxwidth)
        {
          height = Convert.ToInt32((double)height / (double)width * (double)maxwidth);
          width = maxwidth;
        }

        if (height > maxheight)
        {
          width = Convert.ToInt32((double)width / (double)height * (double)maxheight);
          height = maxheight;
        }

        // Create the target bitmap
        bmp = new Bitmap(width, height);

        // Create the graphics object to do the high quality resizing
        gfx = Graphics.FromImage(bmp);
        gfx.CompositingQuality = CompositingQuality.HighQuality;
        gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
        gfx.SmoothingMode = SmoothingMode.HighQuality;

        // Draw the source image
        gfx.DrawImage(img, new Rectangle(new Point(0, 0), new Size(width, height)));

        // Output the data
        context.Response.ContentType = "image/jpeg";
        context.Response.Cache.SetCacheability(HttpCacheability.Public);
        context.Response.Cache.SetExpires(DateTime.UtcNow.AddHours(2));
        context.Response.Cache.SetETag(eTag);
        bmp.Save(context.Response.OutputStream, ImageFormat.Jpeg);
      }
      finally
      {
        if (gfx != null)
        {
          gfx.Dispose();
        }

        if (img != null)
        {
          img.Dispose();
        }

        if (bmp != null)
        {
          bmp.Dispose();
        }
      }
    }

    #endregion

    // TommyB: End MOD: Preview Images

    // TommyB: End MOD: Preview Images                     
  }
}