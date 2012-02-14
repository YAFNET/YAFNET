/* YetAnotherForum.NET
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
  using System.Web.SessionState;

  using YAF.Classes;
  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Core.Services;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;
  using YAF.Utils;
  using YAF.Utils.Helpers;

  #endregion

    /// <summary>
    /// Yaf Resource Handler for all kind of Stuff (Avatars, Attachments, Albums, etc.)
    /// </summary>
    public class YafResourceHandler : IHttpHandler, IReadOnlySessionState, IHaveServiceLocator
  {
    #region Properties

    /// <summary>
    ///   Gets a value indicating whether IsReusable.
    /// </summary>
    public bool IsReusable
    {
      get
      {
        return false;
      }
    }

      public IServiceLocator ServiceLocator
      {
        get
        {
          return YafContext.Current.ServiceLocator;
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
    public void ProcessRequest([NotNull] HttpContext context)
    {
      // resource no longer works with dynamic compile...
      if (context.Request.QueryString.GetFirstOrDefault("r") != null)
      {
        // resource request
        GetResource(context);
      }
      else if (context.Session["lastvisit"] != null || context.Request.UrlReferrer != null && context.Request.UrlReferrer.AbsoluteUri.Contains(BaseUrlBuilder.BaseUrl))
      {
        if (context.Request.QueryString.GetFirstOrDefault("u") != null)
        {
          GetResponseLocalAvatar(context);
        }
        else if (context.Request.QueryString.GetFirstOrDefault("url") != null &&
                 context.Request.QueryString.GetFirstOrDefault("width") != null &&
                 context.Request.QueryString.GetFirstOrDefault("height") != null)
        {
          this.GetResponseRemoteAvatar(context);
        }
        else if (context.Request.QueryString.GetFirstOrDefault("a") != null)
        {
          this.GetResponseAttachment(context);
        }
        else if (context.Request.QueryString.GetFirstOrDefault("i") != null)
        {
          // TommyB: Start MOD: Preview Images   ##########
          this.GetResponseImage(context);
        }
        else if (context.Request.QueryString.GetFirstOrDefault("p") != null)
        {
          this.GetResponseImagePreview(context);
        }
        else if (context.Request.QueryString.GetFirstOrDefault("c") != null)
        {
          // TommyB: End MOD: Preview Images   ##########
          // captcha
          this.GetResponseCaptcha(context);
        }
        else if (context.Request.QueryString.GetFirstOrDefault("cover") != null &&
                 context.Request.QueryString.GetFirstOrDefault("album") != null)
        {
          // album cover
          this.GetAlbumCover(context);
        }
        else if (context.Request.QueryString.GetFirstOrDefault("imgprv") != null)
        {
          // album image preview
          this.GetAlbumImagePreview(context);
        }
        else if (context.Request.QueryString.GetFirstOrDefault("image") != null)
        {
          // album image
          this.GetAlbumImage(context);
        }
        else if (context.Request.QueryString.GetFirstOrDefault("s") != null &&
                 context.Request.QueryString.GetFirstOrDefault("lang") != null)
        {
          GetResponseGoogleSpell(context);
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
    ///   If so, set the status code to 'Not Modified' and stop the response.
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
    private static bool CheckETag([NotNull] HttpContext context, [NotNull] string eTagCode)
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
    /// <param name="previewCropped">
    /// Use Image Cropping</param>
    /// <param name="localizationFile">
    /// The localization file.
    /// </param>
    /// <param name="imagesNumber">
    /// Number of images in the album.
    /// </param>
    /// <returns>
    /// Resized Cover
    /// </returns>
    [NotNull]
    private static MemoryStream GetCoverResized([NotNull] MemoryStream data, int previewWidth, int previewHeight, bool previewCropped, [NotNull] string localizationFile, [NotNull] string imagesNumber)
    {
      const int PixelPadding = 6;
      const int BottomSize = 26;

      var localization = new YafLocalization("ALBUM");
      localization.LoadTranslation(localizationFile);

      using (var src = new Bitmap(data))
      {
        var ms = new MemoryStream();

        // Cropped Image
        int size = previewWidth;

        var newImgSize = new Size(previewWidth, previewHeight);
        int x = 0;
        int y = 0;

        if (previewCropped)
        {
          // Determine dimensions of resized version of the image 
          if (newImgSize.Width > newImgSize.Height)
          {
            newImgSize.Width = decimal.Round((size.ToType<decimal>() * (newImgSize.Width.ToType<decimal>() / newImgSize.Height.ToType<decimal>())).ToType<decimal>(), 0).ToType<int>();
            newImgSize.Height = size;
          }
          else if (newImgSize.Height > newImgSize.Width)
          {
            newImgSize.Height = decimal.Round((size.ToType<decimal>() * (newImgSize.Height.ToType<decimal>() / newImgSize.Width.ToType<decimal>())).ToType<decimal>(), 0).ToType<int>();
            newImgSize.Width = size;
          }
          else
          {
            newImgSize.Width = size;
            newImgSize.Height = size;
          }

          newImgSize.Width = newImgSize.Width - PixelPadding;
          newImgSize.Height = newImgSize.Height - BottomSize - PixelPadding;

          // moves cursor so that crop is more centered 
          x = newImgSize.Width / 2;
          y = newImgSize.Height / 2;
        }
        else
        {
          var finalHeight = Math.Abs(src.Height * newImgSize.Width / src.Width);

          // Height resize if necessary
          if (finalHeight > newImgSize.Height)
          {
            newImgSize.Width = src.Width * newImgSize.Height / src.Height;
            finalHeight = newImgSize.Height;
          }

          newImgSize.Height = finalHeight;
          newImgSize.Width = newImgSize.Width - PixelPadding;
          newImgSize.Height = newImgSize.Height - BottomSize - PixelPadding;
        }

        using (
          var dst = new Bitmap(newImgSize.Width + PixelPadding, newImgSize.Height + BottomSize + PixelPadding, PixelFormat.Format24bppRgb))
        {
          if (previewCropped)
          {
            var rSrcImg = new Rectangle(x, y, newImgSize.Width, newImgSize.Height);
            var rDstImg = new Rectangle(3, 3, dst.Width - PixelPadding, dst.Height - PixelPadding - BottomSize);
            var rDstTxt1 = new Rectangle(3, rDstImg.Height + 3, newImgSize.Width, BottomSize - 13);
            var rDstTxt2 = new Rectangle(3, rDstImg.Height + 16, newImgSize.Width, BottomSize - 13);

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
                  var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };

                  g.DrawString(localization.GetText("ALBUM_VIEW"), f, brush, rDstTxt1, sf);

                  sf.Alignment = StringAlignment.Far;
                  g.DrawString(localization.GetText("ALBUM_IMAGES_NUMBER").FormatWith(imagesNumber), f, brush, rDstTxt2, sf);
                }
              }
            }
          }
          else
          {
            var rSrcImg = new Rectangle(0, 0, src.Width, src.Height);
            var rDstImg = new Rectangle(3, 3, dst.Width - PixelPadding, dst.Height - PixelPadding - BottomSize);
            var rDstTxt1 = new Rectangle(3, rDstImg.Height + 3, newImgSize.Width, BottomSize - 13);
            var rDstTxt2 = new Rectangle(3, rDstImg.Height + 16, newImgSize.Width, BottomSize - 13);

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
                  var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };

                  g.DrawString(localization.GetText("ALBUM_VIEW"), f, brush, rDstTxt1, sf);

                  sf.Alignment = StringAlignment.Far;
                  g.DrawString(localization.GetText("ALBUM_IMAGES_NUMBER").FormatWith(imagesNumber), f, brush, rDstTxt2, sf);
                }
              }
            }
          }

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
    /// <param name="previewCropped">
    /// The preview Cropped</param>
    /// <param name="downloads">
    /// The downloads.
    /// </param>
    /// <param name="localizationFile">
    /// The localization file.
    /// </param>
    /// <returns>
    /// Resized Image Stream
    /// </returns>
    [NotNull]
    private static MemoryStream GetImageResized([NotNull] Stream data, int previewWidth, int previewHeight, bool previewCropped, int downloads, [NotNull] string localizationFile)
    {
      const int PixelPadding = 6;
      const int BottomSize = 26;

      var localization = new YafLocalization("POSTS");
      localization.LoadTranslation(localizationFile);

      using (var src = new Bitmap(data))
      {
        var ms = new MemoryStream();

        // Cropped Image
        int size = previewWidth;

        var newImgSize = new Size(previewWidth, previewHeight);
        int x = 0;
        int y = 0;

        if (previewCropped)
        {
          // Determine dimensions of resized version of the image 
          if (newImgSize.Width > newImgSize.Height)
          {
            newImgSize.Width = decimal.Round((size.ToType<decimal>() * (newImgSize.Width.ToType<decimal>() / newImgSize.Height.ToType<decimal>())).ToType<decimal>(), 0).ToType<int>();
            newImgSize.Height = size;
          }
          else if (newImgSize.Height > newImgSize.Width)
          {
            newImgSize.Height = decimal.Round((size.ToType<decimal>() * (newImgSize.Height.ToType<decimal>() / newImgSize.Width.ToType<decimal>())).ToType<decimal>(), 0).ToType<int>();
            newImgSize.Width = size;
          }
          else
          {
            newImgSize.Width = size;
            newImgSize.Height = size;
          }

          newImgSize.Width = newImgSize.Width - PixelPadding;
          newImgSize.Height = newImgSize.Height - BottomSize - PixelPadding;

          // moves cursor so that crop is more centered 
          x = newImgSize.Width / 2;
          y = newImgSize.Height / 2;
        }
        else
        {
          var finalHeight = Math.Abs(src.Height * newImgSize.Width / src.Width);

          // Height resize if necessary
          if (finalHeight > newImgSize.Height)
          {
            newImgSize.Width = src.Width * newImgSize.Height / src.Height;
            finalHeight = newImgSize.Height;
          }

          newImgSize.Height = finalHeight;
          newImgSize.Width = newImgSize.Width - PixelPadding;
          newImgSize.Height = newImgSize.Height - BottomSize - PixelPadding;
        }

        using (
          var dst = new Bitmap(newImgSize.Width + PixelPadding, newImgSize.Height + BottomSize + PixelPadding, PixelFormat.Format24bppRgb))
        {
          if (previewCropped)
          {
            var rSrcImg = new Rectangle(x, y, newImgSize.Width, newImgSize.Height);
            var rDstImg = new Rectangle(3, 3, dst.Width - PixelPadding, dst.Height - PixelPadding - BottomSize);
            var rDstTxt1 = new Rectangle(3, rDstImg.Height + 3, newImgSize.Width, BottomSize - 13);
            var rDstTxt2 = new Rectangle(3, rDstImg.Height + 16, newImgSize.Width, BottomSize - 13);

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
                  var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };

                  g.DrawString(localization.GetText("IMAGE_RESIZE_ENLARGE"), f, brush, rDstTxt1, sf);

                  sf.Alignment = StringAlignment.Far;
                  g.DrawString(localization.GetText("IMAGE_RESIZE_VIEWS").FormatWith(downloads), f, brush, rDstTxt2, sf);
                }
              }
            }
          }
          else
          {
            var rSrcImg = new Rectangle(0, 0, src.Width, src.Height);
            var rDstImg = new Rectangle(3, 3, dst.Width - PixelPadding, dst.Height - PixelPadding - BottomSize);
            var rDstTxt1 = new Rectangle(3, rDstImg.Height + 3, newImgSize.Width, BottomSize - 13);
            var rDstTxt2 = new Rectangle(3, rDstImg.Height + 16, newImgSize.Width, BottomSize - 13);

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
                  var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };

                  g.DrawString(localization.GetText("IMAGE_RESIZE_ENLARGE"), f, brush, rDstTxt1, sf);

                  sf.Alignment = StringAlignment.Far;
                  g.DrawString(localization.GetText("IMAGE_RESIZE_VIEWS").FormatWith(downloads), f, brush, rDstTxt2, sf);
                }
              }
            }
          }

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
    private static void GetResource([NotNull] HttpContext context)
    {
      // redirect to the resource?
      context.Response.Redirect("resources/{0}".FormatWith(context.Request.QueryString.GetFirstOrDefault("r")));

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

					Stream input = GetType().Assembly.GetManifestResourceStream( resourceName );

					data = new byte [input.Length];
					input.Read( data, 0, data.Length );
					input.Close();

					context.Response.OutputStream.Write( data, 0, data.Length );
				}
				catch
				{
					YAF.Classes.Data.DB.eventlog_create( null, GetType().ToString(), "Attempting to access invalid resource: " + resourceName, 1 );
					context.Response.Write( "Error: Invalid forum resource. Please contact the forum admin." );
				}
			}
			*/
    }

    /// <summary>
    /// The get response google spell.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    private static void GetResponseGoogleSpell([NotNull] HttpContext context)
    {
      string url =
        "https://www.google.com/tbproxy/spell?lang={0}".FormatWith(
          context.Request.QueryString.GetFirstOrDefault("lang"));

      var webRequest = (HttpWebRequest)WebRequest.Create(url);
      webRequest.KeepAlive = true;
      webRequest.Timeout = 100000;
      webRequest.Method = "POST";
      webRequest.ContentType = "application/x-www-form-urlencoded";
      webRequest.ContentLength = context.Request.InputStream.Length;

      Stream requestStream = webRequest.GetRequestStream();

      context.Request.InputStream.CopyTo(requestStream);

      requestStream.Close();

      var httpWebResponse = (HttpWebResponse)webRequest.GetResponse();
      Stream responseStream = httpWebResponse.GetResponseStream();

      responseStream.CopyTo(context.Response.OutputStream);
    }

    /// <summary>
    /// The get response local avatar.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    private static void GetResponseLocalAvatar([NotNull] HttpContext context)
    {
      try
      {
        // string eTag = String.Format( @"""{0}""", context.Request.QueryString.GetFirstOrDefault("u") );

        // if ( CheckETag( context, eTag ) )
        // {
        // found eTag... no need to resend/create this image -- just mark another view?
        // return;
        // }

        using (DataTable dt = LegacyDb.user_avatarimage(context.Request.QueryString.GetFirstOrDefault("u")))
        {
          foreach (DataRow row in dt.Rows)
          {
            var data = (byte[])row["AvatarImage"];
            string contentType = row["AvatarImageType"].ToString();

            context.Response.Clear();
            if (contentType.IsNotSet())
            {
              contentType = "image/jpeg";
            }

            context.Response.ContentType = contentType;
            context.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.Response.Cache.SetExpires(DateTime.UtcNow.AddHours(2));
            context.Response.Cache.SetLastModified(DateTime.UtcNow);
            
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
    /// The check access rights.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="boardUid">
    /// The board Uid.
    /// </param>
    /// <param name="messageID">
    /// The message id.
    /// </param>
    /// <returns>
    /// The check access rights.
    /// </returns>
    private bool CheckAccessRights([NotNull] object boardID, [NotNull] object messageID)
    {
      // Find user name
      MembershipUser user = Membership.GetUser();

      string browser = "{0} {1}".FormatWith(
        HttpContext.Current.Request.Browser.Browser, HttpContext.Current.Request.Browser.Version);
      string platform = HttpContext.Current.Request.Browser.Platform;
      bool isMobileDevice = HttpContext.Current.Request.Browser.IsMobileDevice;
      bool isSearchEngine;
      bool dontTrack;
      string userAgent = HttpContext.Current.Request.UserAgent;

      // try and get more verbose platform name by ref and other parameters             
      UserAgentHelper.Platform(
        userAgent, HttpContext.Current.Request.Browser.Crawler, ref platform, ref browser, out isSearchEngine, out dontTrack);

      this.Get<StartupInitializeDb>().Run();

      object userKey = DBNull.Value;

      if (user != null)
      {
        userKey = user.ProviderUserKey;
      }

      DataRow pageRow = LegacyDb.pageload(
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
        isSearchEngine,
        isMobileDevice,
          dontTrack);
    
      return pageRow["DownloadAccess"].ToType<bool>() ||
             pageRow["ModeratorAccess"].ToType<bool>();
    }

    /// <summary>
    /// The get album cover.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    private void GetAlbumCover([NotNull] HttpContext context)
    {
      // default is 200x200
      int previewMaxWidth = 200;
      int previewMaxHeight = 200;
      bool previewCropped = false;
      string localizationFile = "english.xml";

      if (context.Session["imagePreviewWidth"] != null && context.Session["imagePreviewWidth"] is int)
      {
        previewMaxWidth = (int)context.Session["imagePreviewWidth"];
      }

      if (context.Session["imagePreviewHeight"] != null && context.Session["imagePreviewHeight"] is int)
      {
        previewMaxHeight = (int)context.Session["imagePreviewHeight"];
      }

      if (context.Session["imagePreviewCropped"] != null && context.Session["imagePreviewCropped"] is bool)
      {
        previewCropped = (bool)context.Session["imagePreviewCropped"];
      }

      if (context.Session["localizationFile"] != null && context.Session["localizationFile"] is string)
      {
        localizationFile = context.Session["localizationFile"].ToString();
      }

      string eTag =
        @"""{0}""".FormatWith(context.Request.QueryString.GetFirstOrDefault("cover") + localizationFile.GetHashCode());

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
          if (context.Request.QueryString.GetFirstOrDefault("cover") == "0")
          {
            fileName =
              context.Server.MapPath("{0}/images/{1}".FormatWith(YafForumInfo.ForumClientFileRoot, "noCover.png"));
          }
          else
          {
            using (DataTable dt = LegacyDb.album_image_list(null, context.Request.QueryString.GetFirstOrDefault("cover")))
            {
              if (dt.Rows.Count > 0)
              {
                DataRow row = dt.Rows[0];
                string sUpDir = YafBoardFolders.Current.Uploads;

                string oldFileName =
                  context.Server.MapPath(
                    "{0}/{1}.{2}.{3}".FormatWith(sUpDir, row["UserID"], row["AlbumID"], row["FileName"]));
                string newFileName =
                  context.Server.MapPath(
                    "{0}/{1}.{2}.{3}.yafalbum".FormatWith(sUpDir, row["UserID"], row["AlbumID"], row["FileName"]));

                // use the new fileName (with extension) if it exists...
                fileName = File.Exists(newFileName) ? newFileName : oldFileName;
              }
            }
          }

          using (var input = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
          {
            var buffer = new byte[input.Length];
            input.Read(buffer, 0, buffer.Length);
            data.Write(buffer, 0, buffer.Length);
            input.Close();
          }

          // reset position...
          data.Position = 0;
          string imagesNumber =
            LegacyDb.album_getstats(null, context.Request.QueryString.GetFirstOrDefault("album"))[1].ToString();
          MemoryStream ms = GetCoverResized(data, previewMaxWidth, previewMaxHeight, previewCropped, localizationFile, imagesNumber);

          context.Response.ContentType = "image/png";

          // output stream...
          context.Response.OutputStream.Write(ms.ToArray(), 0, (int)ms.Length);
          context.Response.Cache.SetCacheability(HttpCacheability.Public);
          context.Response.Cache.SetExpires(DateTime.UtcNow.AddHours(2));
          context.Response.Cache.SetLastModified(DateTime.UtcNow);
          context.Response.Cache.SetETag(eTag);

          data.Dispose();
          ms.Dispose();
        }
      }
      catch (Exception x)
      {
          LegacyDb.eventlog_create(null, this.GetType().ToString(), x, EventLogTypes.Information);
        context.Response.Write("Error: Resource has been moved or is unavailable. Please contact the forum admin.");
      }
    }

    /// <summary>
    /// The get album image.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    private void GetAlbumImage([NotNull] HttpContext context)
    {
      try
      {
        string eTag = @"""{0}""".FormatWith(context.Request.QueryString.GetFirstOrDefault("image"));

        if (CheckETag(context, eTag))
        {
          // found eTag... no need to resend/create this image -- just mark another view?
          // YAF.Classes.Data.DB.album_image_download(context.Request.QueryString.GetFirstOrDefault("image"));
          return;
        }

        // ImageID
        using (DataTable dt = LegacyDb.album_image_list(null, context.Request.QueryString.GetFirstOrDefault("image")))
        {
          foreach (DataRow row in dt.Rows)
          {
            byte[] data;

            string sUpDir = YafBoardFolders.Current.Uploads;

            string oldFileName =
              context.Server.MapPath(
                "{0}/{1}.{2}.{3}".FormatWith(sUpDir, row["UserID"], row["AlbumID"], row["FileName"]));
            string newFileName =
              context.Server.MapPath(
                "{0}/{1}.{2}.{3}.yafalbum".FormatWith(sUpDir, row["UserID"], row["AlbumID"], row["FileName"]));

            // use the new fileName (with extension) if it exists...
            string fileName = File.Exists(newFileName) ? newFileName : oldFileName;

            using (var input = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
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
            LegacyDb.album_image_download(context.Request.QueryString.GetFirstOrDefault("image"));
            break;
          }
        }
      }
      catch (Exception x)
      {
          LegacyDb.eventlog_create(null, this.GetType().ToString(), x, EventLogTypes.Information);
        context.Response.Write("Error: Resource has been moved or is unavailable. Please contact the forum admin.");
      }
    }

    /// <summary>
    /// The get album image preview.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    private void GetAlbumImagePreview([NotNull] HttpContext context)
    {
      // default is 200x200
      int previewMaxWidth = 200;
      int previewMaxHeight = 200;
      bool previewCropped = false;
      string localizationFile = "english.xml";

      if (context.Session["imagePreviewWidth"] != null && context.Session["imagePreviewWidth"] is int)
      {
        previewMaxWidth = (int)context.Session["imagePreviewWidth"];
      }

      if (context.Session["imagePreviewHeight"] != null && context.Session["imagePreviewHeight"] is int)
      {
        previewMaxHeight = (int)context.Session["imagePreviewHeight"];
      }

      if (context.Session["imagePreviewCropped"] != null && context.Session["imagePreviewCropped"] is bool)
      {
        previewCropped = (bool)context.Session["imagePreviewCropped"];
      }

      if (context.Session["localizationFile"] != null && context.Session["localizationFile"] is string)
      {
        localizationFile = context.Session["localizationFile"].ToString();
      }

      string eTag =
        @"""{0}""".FormatWith(context.Request.QueryString.GetFirstOrDefault("imgprv") + localizationFile.GetHashCode());

      if (CheckETag(context, eTag))
      {
        // found eTag... no need to resend/create this image...
        return;
      }

      try
      {
        // ImageID
        using (DataTable dt = LegacyDb.album_image_list(null, context.Request.QueryString.GetFirstOrDefault("imgprv")))
        {
          foreach (DataRow row in dt.Rows)
          {
            var data = new MemoryStream();

            string sUpDir = YafBoardFolders.Current.Uploads;

            string oldFileName =
              context.Server.MapPath(
                "{0}/{1}.{2}.{3}".FormatWith(sUpDir, row["UserID"], row["AlbumID"], row["FileName"]));
            string newFileName =
              context.Server.MapPath(
                "{0}/{1}.{2}.{3}.yafalbum".FormatWith(sUpDir, row["UserID"], row["AlbumID"], row["FileName"]));

            // use the new fileName (with extension) if it exists...
            string fileName = File.Exists(newFileName) ? newFileName : oldFileName;

            using (var input = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
              var buffer = new byte[input.Length];
              input.Read(buffer, 0, buffer.Length);
              data.Write(buffer, 0, buffer.Length);
              input.Close();
            }

            // reset position...
            data.Position = 0;

            MemoryStream ms = GetImageResized(
              data, previewMaxWidth, previewMaxHeight, previewCropped, (int)row["Downloads"], localizationFile);

            context.Response.ContentType = "image/png";

            // output stream...
            context.Response.OutputStream.Write(ms.ToArray(), 0, (int)ms.Length);
            context.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.Response.Cache.SetExpires(DateTime.UtcNow.AddHours(2));
            context.Response.Cache.SetLastModified(DateTime.UtcNow);
            context.Response.Cache.SetETag(eTag);

            data.Dispose();
            ms.Dispose();

            break;
          }
        }
      }
      catch (Exception x)
      {
          LegacyDb.eventlog_create(null, this.GetType().ToString(), x, EventLogTypes.Information);
        context.Response.Write("Error: Resource has been moved or is unavailable. Please contact the forum admin.");
      }
    }

    /// <summary>
    /// The get response attachment.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    private void GetResponseAttachment([NotNull] HttpContext context)
    {
      try
      {
        // AttachmentID
        using (DataTable dt = LegacyDb.attachment_list(null, context.Request.QueryString.GetFirstOrDefault("a"), null))
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

            byte[] data;

            if (row.IsNull("FileData"))
            {
              string sUpDir = YafBoardFolders.Current.Uploads;

              string oldFileName =
                context.Server.MapPath("{0}/{1}.{2}".FormatWith(sUpDir, row["MessageID"], row["FileName"]));
              string newFileName =
                context.Server.MapPath("{0}/{1}.{2}.yafupload".FormatWith(sUpDir, row["MessageID"], row["FileName"]));

              // use the new fileName (with extension) if it exists...
              string fileName = File.Exists(newFileName) ? newFileName : oldFileName;

              using (var input = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
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
              "attachment; filename={0}".FormatWith(
                HttpUtility.UrlPathEncode(row["FileName"].ToString()).Replace("+", "_")));
            context.Response.OutputStream.Write(data, 0, data.Length);
            LegacyDb.attachment_download(context.Request.QueryString.GetFirstOrDefault("a"));
            break;
          }
        }
      }
      catch (Exception x)
      {
          LegacyDb.eventlog_create(null, this.GetType().ToString(), x, EventLogTypes.Information);
        context.Response.Write("Error: Resource has been moved or is unavailable. Please contact the forum admin.");
      }
    }

    /// <summary>
    /// The get response captcha.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    private void GetResponseCaptcha([NotNull] HttpContext context)
    {
#if (!DEBUG)
      try
      {
#endif

      var captchaImage = new CaptchaImage(
        CaptchaHelper.GetCaptchaText(new HttpSessionStateWrapper(context.Session), context.Cache, true), 250, 50, "Century Schoolbook");
      context.Response.Clear();
      context.Response.ContentType = "image/jpeg";
      captchaImage.Image.Save(context.Response.OutputStream, ImageFormat.Jpeg);
#if (!DEBUG)
      }
      catch (Exception x)
      {
        LegacyDb.eventlog_create(null, this.GetType().ToString(), x, 1);
        context.Response.Write("Error: Resource has been moved or is unavailable. Please contact the forum admin.");
      }

#endif
    }

    // TommyB: Start MOD: PreviewImages   ##########

    /// <summary>
    /// The get response image.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    private void GetResponseImage([NotNull] HttpContext context)
    {
      try
      {
        string eTag = @"""{0}""".FormatWith(context.Request.QueryString.GetFirstOrDefault("i"));

        if (CheckETag(context, eTag))
        {
          // found eTag... no need to resend/create this image -- just mark another view?
          LegacyDb.attachment_download(context.Request.QueryString.GetFirstOrDefault("i"));
          return;
        }

        // AttachmentID
        using (DataTable dt = LegacyDb.attachment_list(null, context.Request.QueryString.GetFirstOrDefault("i"), null))
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

            byte[] data;

            if (row.IsNull("FileData"))
            {
              string sUpDir = YafBoardFolders.Current.Uploads;

              string oldFileName =
                context.Server.MapPath("{0}/{1}.{2}".FormatWith(sUpDir, row["MessageID"], row["FileName"]));
              string newFileName =
                context.Server.MapPath("{0}/{1}.{2}.yafupload".FormatWith(sUpDir, row["MessageID"], row["FileName"]));

              // use the new fileName (with extension) if it exists...
              string fileName = File.Exists(newFileName) ? newFileName : oldFileName;

              using (var input = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
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
            LegacyDb.attachment_download(context.Request.QueryString.GetFirstOrDefault("i"));
            break;
          }
        }
      }
      catch (Exception x)
      {
          LegacyDb.eventlog_create(null, this.GetType().ToString(), x, EventLogTypes.Information);
        context.Response.Write("Error: Resource has been moved or is unavailable. Please contact the forum admin.");
      }
    }

    /// <summary>
    /// The get response image preview.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    private void GetResponseImagePreview([NotNull] HttpContext context)
    {
      // default is 200x200
      int previewMaxWidth = 200;
      int previewMaxHeight = 200;
      bool previewCropped = false;
      string localizationFile = "english.xml";

      if (context.Session["imagePreviewWidth"] != null && context.Session["imagePreviewWidth"] is int)
      {
        previewMaxWidth = (int)context.Session["imagePreviewWidth"];
      }

      if (context.Session["imagePreviewHeight"] != null && context.Session["imagePreviewHeight"] is int)
      {
        previewMaxHeight = (int)context.Session["imagePreviewHeight"];
      }

      if (context.Session["imagePreviewCropped"] != null && context.Session["imagePreviewCropped"] is bool)
      {
        previewCropped = (bool)context.Session["imagePreviewCropped"];
      }

      if (context.Session["localizationFile"] != null && context.Session["localizationFile"] is string)
      {
        localizationFile = context.Session["localizationFile"].ToString();
      }

      string eTag =
        @"""{0}""".FormatWith(context.Request.QueryString.GetFirstOrDefault("p") + localizationFile.GetHashCode());

      if (CheckETag(context, eTag))
      {
        // found eTag... no need to resend/create this image...
        return;
      }

      try
      {
        // AttachmentID
        using (DataTable dt = LegacyDb.attachment_list(null, context.Request.QueryString.GetFirstOrDefault("p"), null))
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
                context.Server.MapPath("{0}/{1}.{2}".FormatWith(sUpDir, row["MessageID"], row["FileName"]));
              string newFileName =
                context.Server.MapPath("{0}/{1}.{2}.yafupload".FormatWith(sUpDir, row["MessageID"], row["FileName"]));

              // use the new fileName (with extension) if it exists...
              string fileName = File.Exists(newFileName) ? newFileName : oldFileName;

              using (var input = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
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

            MemoryStream ms = GetImageResized(
              data, previewMaxWidth, previewMaxHeight, previewCropped, (int)row["Downloads"], localizationFile);

            context.Response.ContentType = "image/png";

            // output stream...
            context.Response.OutputStream.Write(ms.ToArray(), 0, (int)ms.Length);
            context.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.Response.Cache.SetExpires(DateTime.UtcNow.AddHours(2));
            context.Response.Cache.SetLastModified(DateTime.UtcNow);
            context.Response.Cache.SetETag(eTag);

            data.Dispose();
            ms.Dispose();

            break;
          }
        }
      }
      catch (Exception x)
      {
          LegacyDb.eventlog_create(null, this.GetType().ToString(), x, EventLogTypes.Information);
        context.Response.Write("Error: Resource has been moved or is unavailable. Please contact the forum admin.");
      }
    }

    /// <summary>
    /// The get response remote avatar.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    private void GetResponseRemoteAvatar([NotNull] HttpContext context)
    {
      if (General.GetCurrentTrustLevel() < AspNetHostingPermissionLevel.Medium)
      {
        // don't bother... not supported.
        LegacyDb.eventlog_create(
          null,
          this.GetType().ToString(),
          "Remote Avatar is NOT supported on your Hosting Permission Level (must be High)",
          EventLogTypes.Error);
        return;
      }

      string avatarUrl = context.Request.QueryString.GetFirstOrDefault("url");

      int maxwidth = int.Parse(context.Request.QueryString.GetFirstOrDefault("width"));
      int maxheight = int.Parse(context.Request.QueryString.GetFirstOrDefault("height"));

      string eTag =
        @"""{0}""".FormatWith(
          (context.Request.QueryString.GetFirstOrDefault("url") + maxheight + maxwidth).GetHashCode());

      if (CheckETag(context, eTag))
      {
        // found eTag... no need to download this image...
        return;
      }

      var webClient = new WebClient();

      try
      {
        using (var avatarStream = webClient.OpenRead(avatarUrl))
        {
          if (avatarStream == null)
          {
            return;
          }

          using (var img = new Bitmap(avatarStream))
          {
            int width = img.Width;
            int height = img.Height;

            if (width <= maxwidth && height <= maxheight)
            {
              context.Response.Redirect(avatarUrl);
            }

            if (width > maxwidth)
            {
              height = (height / (double)width * maxwidth).ToType<int>();
              width = maxwidth;
            }

            if (height > maxheight)
            {
              width = (width / (double)height * maxheight).ToType<int>();
              height = maxheight;
            }

            // Create the target bitmap
            using (var bmp = new Bitmap(width, height))
            {
              // Create the graphics object to do the high quality resizing
              using (var gfx = Graphics.FromImage(bmp))
              {
                gfx.CompositingQuality = CompositingQuality.HighQuality;
                gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gfx.SmoothingMode = SmoothingMode.HighQuality;

                // Draw the source image
                gfx.DrawImage(img, new Rectangle(new Point(0, 0), new Size(width, height)));
              }

              // Output the data
              context.Response.ContentType = "image/jpeg";
              context.Response.Cache.SetCacheability(HttpCacheability.Public);
              context.Response.Cache.SetExpires(DateTime.UtcNow.AddHours(2));
              context.Response.Cache.SetLastModified(DateTime.UtcNow);
              context.Response.Cache.SetETag(eTag);
              bmp.Save(context.Response.OutputStream, ImageFormat.Jpeg);
            }
          }
        }
      }
      catch (System.Net.WebException)
      {
        // issue getting access to the avatar...
      }
    }

    #endregion              
  }
}