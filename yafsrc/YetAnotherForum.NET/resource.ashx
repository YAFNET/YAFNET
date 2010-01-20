<%@ WebHandler Language="C#" Class="YAF.resource" %>
using System;
using System.Collections;
using System.Data;
using System.Net;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Configuration;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Utils;

namespace YAF
{
	/// <summary>
	/// Summary description for $Codefileclassname$
	/// </summary>
	[WebService(Namespace="http://www.yetanotherforum.net/")]
	[WebServiceBinding(ConformsTo=WsiProfiles.BasicProfile1_1)]
	public class resource : IHttpHandler, IReadOnlySessionState
	{
		public void ProcessRequest(HttpContext context)
		{
			// resource no longer works with dynamic compile...
			if (context.Request.QueryString["r"] != null)
			{
				// resource request
				GetResource(context);

			}
			else if (context.Session["lastvisit"] != null)
			{
				if (context.Request.QueryString["u"] != null)
				{
					GetResponseLocalAvatar(context);
				}
				else if (context.Request.QueryString["url"] != null && context.Request.QueryString["width"] != null && context.Request.QueryString["height"] != null)
				{
					GetResponseRemoteAvatar(context);
				}
				else if (context.Request.QueryString["a"] != null)
				{
					GetResponseAttachment(context);
				}
				// TommyB: Start MOD: Preview Images   ##########
				else if (context.Request.QueryString["i"] != null)
				{
					GetResponseImage(context);
				}
				else if (context.Request.QueryString["p"] != null)
				{
					GetResponseImagePreview(context);
				}
				// TommyB: End MOD: Preview Images   ##########
				else if (context.Request.QueryString["c"] != null && context.Session["CaptchaImageText"] != null)
				{
					// captcha					
					GetResponseCaptcha(context);
				}
                else if (context.Request.QueryString["cover"] != null && context.Request.QueryString["album"] != null)
                {
                    // album cover		
                    GetAlbumCover(context);
                }
                else if (context.Request.QueryString["imgprv"] != null)
                {
                    // album image preview		
                    GetAlbumImagePreview(context);
                }
                else if (context.Request.QueryString["image"] != null)
                {
                    // album image		
                    GetAlbumImage(context);
                }                             
				else if (context.Request.QueryString["s"] != null && context.Request.QueryString["lang"] != null)
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

		private void GetResponseLocalAvatar(HttpContext context)
		{
			try
			{
				//string eTag = String.Format( @"""{0}""", context.Request.QueryString["u"] );

				//if ( CheckETag( context, eTag ) )
				//{
					// found eTag... no need to resend/create this image -- just mark another view?
					//return;
				//}
	
				using ( DataTable dt = YAF.Classes.Data.DB.user_avatarimage( context.Request.QueryString["u"] ) )
				{
					foreach ( DataRow row in dt.Rows )
					{
						byte[] data = (byte[]) row["AvatarImage"];
						string contentType = row["AvatarImageType"].ToString();

						context.Response.Clear();
						if ( String.IsNullOrEmpty( contentType ) )
						{
							contentType = "image/jpeg";
						}
						context.Response.ContentType = contentType;
						context.Response.Cache.SetCacheability( HttpCacheability.Public );
						context.Response.Cache.SetExpires( DateTime.Now.AddHours( 2 ) );
						//context.Response.Cache.SetETag( eTag );
						context.Response.OutputStream.Write( data, 0, data.Length );
						break;
					}
				}
			}
			catch ( Exception )
			{
				
			}
		}

		private void GetResponseRemoteAvatar(HttpContext context)
		{
			System.Net.WebClient wc = new System.Net.WebClient();
			Image img = null;
			Bitmap bmp = null;
			Graphics gfx = null;

			if ( General.GetCurrentTrustLevel() < AspNetHostingPermissionLevel.High )
			{
				// don't bother... not supported.
				YAF.Classes.Data.DB.eventlog_create( null, this.GetType().ToString(),
				                                     "Remote Avatar is NOT supported on your Hosting Permission Level (must be High)",
				                                     0 );
				return;
			}

			string wb = context.Request.QueryString["url"];

			try
			{
				int maxwidth = int.Parse( context.Request.QueryString["width"] );
				int maxheight = int.Parse( context.Request.QueryString["height"] );

				string eTag = String.Format( @"""{0}""", (context.Request.QueryString["url"]+maxheight.ToString()+maxwidth.ToString()).GetHashCode() );

				if ( CheckETag( context, eTag ) )
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
					context.Response.Redirect(wb);
				
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
				context.Response.Cache.SetCacheability( HttpCacheability.Public );
				context.Response.Cache.SetExpires( DateTime.Now.AddHours( 2 ) );
				context.Response.Cache.SetETag( eTag );
				bmp.Save(context.Response.OutputStream, ImageFormat.Jpeg);
			}
			finally
			{
				if (gfx != null) gfx.Dispose();
				if (img != null) img.Dispose();
				if (bmp != null) bmp.Dispose();
			}
		}

		private void GetResponseAttachment(HttpContext context)
		{
			try
			{
				// AttachmentID
				using (DataTable dt = YAF.Classes.Data.DB.attachment_list(null, context.Request.QueryString["a"], null))
				{
					foreach (DataRow row in dt.Rows)
					{
						/// TODO : check download permissions here					
						if (!CheckAccessRights(row["BoardID"], row["MessageID"]))
						{
							// tear it down
							// no permission to download
							context.Response.Write("You have insufficient rights to download this resource. Contact forum administrator for further details.");
							return;
						}

						byte[] data = null;

						if (row.IsNull("FileData"))
						{
                            string sUpDir = YafBoardFolders.Current.Uploads;

							string oldFileName = context.Server.MapPath(String.Format("{0}/{1}.{2}", sUpDir, row["MessageID"], row["FileName"]));
							string newFileName = context.Server.MapPath(String.Format("{0}/{1}.{2}.yafupload", sUpDir, row["MessageID"], row["FileName"]));

							string fileName = string.Empty;

							// use the new fileName (with extension) if it exists...
							if (File.Exists(newFileName)) fileName = newFileName;
							else fileName = oldFileName;

							using (System.IO.FileStream input = new System.IO.FileStream(fileName, System.IO.FileMode.Open))
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
						context.Response.AppendHeader("Content-Disposition", String.Format("attachment; filename={0}", HttpUtility.UrlEncode(row["FileName"].ToString()).Replace("+", "%20")));
						context.Response.OutputStream.Write(data, 0, data.Length);
						YAF.Classes.Data.DB.attachment_download(context.Request.QueryString["a"]);
						break;
					}
				}
			}
			catch (Exception x)
			{
				YAF.Classes.Data.DB.eventlog_create(null, this.GetType().ToString(), x, 1);
				context.Response.Write("Error: Resource has been moved or is unavailable. Please contact the forum admin.");
			}
		}

		// TommyB: Start MOD: PreviewImages   ##########
		private void GetResponseImage(HttpContext context)
		{
			try
			{
				string eTag = String.Format( @"""{0}""", context.Request.QueryString["i"] );

				if ( CheckETag( context, eTag ) )
				{
					// found eTag... no need to resend/create this image -- just mark another view?
					YAF.Classes.Data.DB.attachment_download( context.Request.QueryString["i"] );
					return;
				}						
				
				// AttachmentID
				using (DataTable dt = YAF.Classes.Data.DB.attachment_list(null, context.Request.QueryString["i"], null))
				{
					foreach (DataRow row in dt.Rows)
					{
						/// TODO : check download permissions here					
						if (!CheckAccessRights(row["BoardID"], row["MessageID"]))
						{
							// tear it down
							// no permission to download
							context.Response.Write("You have insufficient rights to download this resource. Contact forum administrator for further details.");
							return;
						}

						byte[] data = null;

						if (row.IsNull("FileData"))
						{
              string sUpDir = YafBoardFolders.Current.Uploads;

							string oldFileName = context.Server.MapPath(String.Format("{0}/{1}.{2}", sUpDir, row["MessageID"], row["FileName"]));
							string newFileName = context.Server.MapPath(String.Format("{0}/{1}.{2}.yafupload", sUpDir, row["MessageID"], row["FileName"]));

							string fileName = string.Empty;

							// use the new fileName (with extension) if it exists...
							if (File.Exists(newFileName)) fileName = newFileName;
							else fileName = oldFileName;
							using (System.IO.FileStream input = new System.IO.FileStream(fileName, System.IO.FileMode.Open))
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
						context.Response.Cache.SetCacheability( HttpCacheability.Public );
						context.Response.Cache.SetETag( eTag );						
						context.Response.OutputStream.Write(data, 0, data.Length);
						
						// add a download count...
						YAF.Classes.Data.DB.attachment_download(context.Request.QueryString["i"]);
						break;
					}
				}
			}
			catch (Exception x)
			{
				YAF.Classes.Data.DB.eventlog_create(null, this.GetType().ToString(), x, 1);
				context.Response.Write("Error: Resource has been moved or is unavailable. Please contact the forum admin.");
			}
		}

		private void GetResponseImagePreview(HttpContext context)
		{
			// default is 200x200
			int previewMaxWidth = 200;
			int previewMaxHeight = 200;
			string localizationFile = "english.xml";

			if (context.Session["imagePreviewWidth"] != null && context.Session["imagePreviewWidth"] is int )
			{
				previewMaxWidth = (int)context.Session["imagePreviewWidth"];
			}

			if ( context.Session["imagePreviewHeight"] != null && context.Session["imagePreviewHeight"] is int )
			{
				previewMaxHeight = (int)context.Session["imagePreviewHeight"];
			}			
			
			if ( context.Session["localizationFile"] != null && context.Session["localizationFile"] is string )
			{
				localizationFile = context.Session["localizationFile"].ToString();
			}

			string eTag = String.Format( @"""{0}""",
			                             (context.Request.QueryString["p"] + localizationFile.GetHashCode().ToString()) );
			
			if ( CheckETag( context, eTag ))
			{
				// found eTag... no need to resend/create this image...
				return;
			}

			try
			{
				// AttachmentID
				using (DataTable dt = YAF.Classes.Data.DB.attachment_list(null, context.Request.QueryString["p"], null))
				{
					foreach (DataRow row in dt.Rows)
					{
						/// TODO : check download permissions here					
						if (!CheckAccessRights(row["BoardID"], row["MessageID"]))
						{
							// tear it down
							// no permission to download
							context.Response.Write("You have insufficient rights to download this resource. Contact forum administrator for further details.");
							return;
						}

						MemoryStream data = new MemoryStream();

						if (row.IsNull("FileData"))
						{
                            string sUpDir = YafBoardFolders.Current.Uploads;

							string oldFileName = context.Server.MapPath(String.Format("{0}/{1}.{2}", sUpDir, row["MessageID"], row["FileName"]));
							string newFileName = context.Server.MapPath(String.Format("{0}/{1}.{2}.yafupload", sUpDir, row["MessageID"], row["FileName"]));

							string fileName = string.Empty;

							// use the new fileName (with extension) if it exists...
							if (File.Exists(newFileName)) fileName = newFileName;
							else fileName = oldFileName;

							using (System.IO.FileStream input = new System.IO.FileStream(fileName, System.IO.FileMode.Open))
							{
								byte[] buffer = new byte[input.Length];
								input.Read(buffer, 0, buffer.Length);
								data.Write(buffer, 0, buffer.Length);
								input.Close();
							}
						}
						else
						{
							byte[] buffer = (byte[])row["FileData"];
							data.Write(buffer, 0, buffer.Length);
						}

						// reset position...
						data.Position = 0;

						MemoryStream ms = GetImageResized( data, previewMaxWidth, previewMaxHeight, (int)row["Downloads"], localizationFile);

						context.Response.ContentType = "image/png";
						
						// output stream...
						context.Response.OutputStream.Write( ms.ToArray(), 0, (int)ms.Length );
						context.Response.Cache.SetCacheability( HttpCacheability.Public );
						context.Response.Cache.SetExpires( DateTime.Now.AddHours( 2 ) );
						context.Response.Cache.SetETag( eTag );

						data.Dispose();
						ms.Dispose();
						
						break;
					}
				}
			}
			catch (Exception x)
			{
				YAF.Classes.Data.DB.eventlog_create(null, this.GetType().ToString(), x, 1);
				context.Response.Write("Error: Resource has been moved or is unavailable. Please contact the forum admin.");
			}
		}
		// TommyB: End MOD: Preview Images

        // TommyB: End MOD: Preview Images                     

        private MemoryStream GetImageResized(MemoryStream data, int previewWidth, int previewHeight, int downloads, string localizationFile)
        {
            const int pixelPadding = 6;
            const int bottomSize = 13;

            using (Bitmap src = new Bitmap(data))
            {
                // default to width-based resizing...
                int width = previewWidth;
                int height = (int)(previewWidth / ((double)src.Width / (double)src.Height));

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

                using (Bitmap dst = new Bitmap(width + pixelPadding, height + bottomSize + pixelPadding, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
                {
                    Rectangle rSrcImg = new Rectangle(0, 0, src.Width, src.Height);
                    Rectangle rDstImg = new Rectangle(3, 3, dst.Width - pixelPadding, dst.Height - pixelPadding - bottomSize);
                    Rectangle rDstTxt = new Rectangle(3, rDstImg.Height + 3, previewWidth, bottomSize);
                    using (Graphics g = Graphics.FromImage(dst))
                    {
                        g.Clear(Color.FromArgb(64, 64, 64));
                        g.FillRectangle(Brushes.White, rDstImg);

                        g.CompositingMode = CompositingMode.SourceOver;
                        g.CompositingQuality = CompositingQuality.GammaCorrected;
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                        g.DrawImage(src, rDstImg, rSrcImg, GraphicsUnit.Pixel);

                        using (Font f = new Font("Arial", 10, FontStyle.Regular, GraphicsUnit.Pixel))
                        {
                            using (SolidBrush brush = new SolidBrush(Color.FromArgb(191, 191, 191)))
                            {
                                YafLocalization localization = new YafLocalization("POSTS");
                                localization.LoadTranslation(localizationFile);

                                StringFormat sf = new StringFormat();

                                sf.Alignment = StringAlignment.Near;
                                sf.LineAlignment = StringAlignment.Center;
                                g.DrawString(localization.GetText("IMAGE_RESIZE_ENLARGE"), f, brush, rDstTxt, sf);

                                sf.Alignment = StringAlignment.Far;
                                g.DrawString(string.Format(localization.GetText("IMAGE_RESIZE_VIEWS"), downloads.ToString()), f, brush, rDstTxt, sf);
                            }
                        }
                    }

                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    // save the bitmap to the stream...
                    dst.Save(ms, ImageFormat.Png);
                    ms.Position = 0;

                    return ms;
                }
            }
        }
                
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

            string eTag = String.Format(@"""{0}""",
                                         (context.Request.QueryString["cover"] + localizationFile.GetHashCode().ToString()));

            if (CheckETag(context, eTag))
            {
                // found eTag... no need to resend/create this image...
                return;
            }

            try
            {
                // CoverID
                {
                    string fileName = string.Empty;
                    var data = new MemoryStream();
                    if (context.Request.QueryString["cover"] == "0")
                    {
                        fileName = context.Server.MapPath(
                                        String.Format("{0}/images/{1}", YafForumInfo.ForumRoot, "noCover.png"));
                    }
                    else
                    {
                        using (
                            DataTable dt = YAF.Classes.Data.DB.album_image_list(
                                null, context.Request.QueryString["cover"]))
                        {
                            if (dt.Rows.Count > 0)
                            {
                                DataRow row = dt.Rows[0];
                                string sUpDir = YafBoardFolders.Current.Uploads;

                                string oldFileName =
                                    context.Server.MapPath(
                                        String.Format(
                                            "{0}/{1}.{2}.{3}", sUpDir, row["UserID"], row["AlbumID"], row["FileName"]));
                                string newFileName =
                                    context.Server.MapPath(
                                        String.Format(
                                            "{0}/{1}.{2}.{3}.yafalbum",
                                            sUpDir,
                                            row["UserID"],
                                            row["AlbumID"],
                                            row["FileName"]));
                                // use the new fileName (with extension) if it exists...
                                fileName = File.Exists(newFileName) ? newFileName : oldFileName;
                            }
                        }
                    }
                    using (var input = new System.IO.FileStream(fileName, System.IO.FileMode.Open))
                    {
                        byte[] buffer = new byte[input.Length];
                        input.Read(buffer, 0, buffer.Length);
                        data.Write(buffer, 0, buffer.Length);
                        input.Close();
                    }

                    // reset position...
                    data.Position = 0;
                    string imagesNumber =
                        YAF.Classes.Data.DB.album_getstats(null, context.Request.QueryString["album"])[1].ToString();
                    MemoryStream ms = GetCoverResized(
                        data, previewMaxWidth, previewMaxHeight, localizationFile, imagesNumber);

                    context.Response.ContentType = "image/png";

                    // output stream...
                    context.Response.OutputStream.Write(ms.ToArray(), 0, (int)ms.Length);
                    context.Response.Cache.SetCacheability(HttpCacheability.Public);
                    context.Response.Cache.SetExpires(DateTime.Now.AddHours(2));
                    context.Response.Cache.SetETag(eTag);

                    data.Dispose();
                    ms.Dispose();
                }
            }
            catch (Exception x)
            {
                YAF.Classes.Data.DB.eventlog_create(null, this.GetType().ToString(), x, 1);
                context.Response.Write("Error: Resource has been moved or is unavailable. Please contact the forum admin.");
            }
        }
        private MemoryStream GetCoverResized(MemoryStream data, int previewWidth, int previewHeight, string localizationFile, string ImagesNumber)
        {
            const int paddingY = 6;
            const int paddingX = 6;
            const int bottomSize = 13;
            const int topSize = 0;
            using (Bitmap src = new Bitmap(data))
            {
                // default to width-based resizing...
                int width = previewWidth;
                int height = (int)(previewWidth / ((double)src.Width / (double)src.Height));

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

				using ( Bitmap dst = new Bitmap( width + paddingX, height + topSize + bottomSize + paddingY, System.Drawing.Imaging.PixelFormat.Format24bppRgb ) )
				{
					Rectangle rSrcImg = new Rectangle( 0, 0, src.Width, src.Height );
					Rectangle rDstImg = new Rectangle( 3 , 3 + topSize, dst.Width - paddingX, dst.Height - topSize - paddingY - bottomSize );
					Rectangle rDstTxt = new Rectangle( 3, rDstImg.Height + 3 + topSize, previewWidth, bottomSize );
                    //Rectangle rDstTitle = new Rectangle(3, 3, previewWidth, topSize);
                    using (Graphics g = Graphics.FromImage(dst))
                    {

                        g.Clear(Color.FromArgb(64, 64, 64));
                        g.FillRectangle(Brushes.White, rDstImg);

                        g.CompositingMode = CompositingMode.SourceOver;
                        g.CompositingQuality = CompositingQuality.GammaCorrected;
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                        g.DrawImage(src, rDstImg, rSrcImg, GraphicsUnit.Pixel);

                        using (Font f = new Font("Arial", 10, FontStyle.Regular, GraphicsUnit.Pixel))
                        {
                            using (SolidBrush brush = new SolidBrush(Color.FromArgb(191, 191, 191)))
                            {
                                YafLocalization localization = new YafLocalization("ALBUM");
                                localization.LoadTranslation(localizationFile);
                                StringFormat sf = new StringFormat();

                                sf.Alignment = StringAlignment.Far;
                                sf.LineAlignment = StringAlignment.Near;
                                g.DrawString(localization.GetText("ALBUM_VIEW"), f, brush, rDstTxt, sf);

                                sf.Alignment = StringAlignment.Near;
                                sf.LineAlignment = StringAlignment.Near;
                                g.DrawString(String.Format(localization.GetText("ALBUM_IMAGES_NUMBER"), ImagesNumber), f, brush, rDstTxt, sf);                                                                
                            }
                        }
                    }

                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    // save the bitmap to the stream...
                    dst.Save(ms, ImageFormat.Png);
                    ms.Position = 0;

                    return ms;
                }
            }
        }

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

            string eTag = String.Format(@"""{0}""",
                                         (context.Request.QueryString["imgprv"] + localizationFile.GetHashCode().ToString()));

            if (CheckETag(context, eTag))
            {
                // found eTag... no need to resend/create this image...
                return;
            }

            try
            {
                // ImageID
                using (DataTable dt = YAF.Classes.Data.DB.album_image_list(null, context.Request.QueryString["imgprv"]))
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        MemoryStream data = new MemoryStream();

                            string sUpDir = YafBoardFolders.Current.Uploads;

                            string oldFileName = context.Server.MapPath(String.Format("{0}/{1}.{2}.{3}", sUpDir, row["UserID"], row["AlbumID"], row["FileName"]));
                            string newFileName = context.Server.MapPath(String.Format("{0}/{1}.{2}.{3}.yafalbum", sUpDir, row["UserID"], row["AlbumID"], row["FileName"]));

                            string fileName = string.Empty;

                            // use the new fileName (with extension) if it exists...
                            if (File.Exists(newFileName)) fileName = newFileName;
                            else fileName = oldFileName;

                            using (System.IO.FileStream input = new System.IO.FileStream(fileName, System.IO.FileMode.Open))
                            {
                                byte[] buffer = new byte[input.Length];
                                input.Read(buffer, 0, buffer.Length);
                                data.Write(buffer, 0, buffer.Length);
                                input.Close();
                            }
                        
                        // reset position...
                        data.Position = 0;

                        MemoryStream ms = GetImageResized(data, previewMaxWidth, previewMaxHeight, (int)row["Downloads"], localizationFile);

                        context.Response.ContentType = "image/png";

                        // output stream...
                        context.Response.OutputStream.Write(ms.ToArray(), 0, (int)ms.Length);
                        context.Response.Cache.SetCacheability(HttpCacheability.Public);
                        context.Response.Cache.SetExpires(DateTime.Now.AddHours(2));
                        context.Response.Cache.SetETag(eTag);

                        data.Dispose();
                        ms.Dispose();

                        break;
                    }
                }
            }
            catch (Exception x)
            {
                YAF.Classes.Data.DB.eventlog_create(null, this.GetType().ToString(), x, 1);
                context.Response.Write("Error: Resource has been moved or is unavailable. Please contact the forum admin.");
            }
        }

        private void GetAlbumImage(HttpContext context)
        {
            try
            {
                string eTag = String.Format(@"""{0}""", context.Request.QueryString["image"]);

                if (CheckETag(context, eTag))
                {
                    // found eTag... no need to resend/create this image -- just mark another view?
                    //YAF.Classes.Data.DB.album_image_download(context.Request.QueryString["image"]);
                    return;
                }

                // ImageID
                using (DataTable dt = YAF.Classes.Data.DB.album_image_list(null, context.Request.QueryString["image"]))
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        byte[] data = null;

                        string sUpDir = YafBoardFolders.Current.Uploads;

                        string oldFileName = context.Server.MapPath(String.Format("{0}/{1}.{2}.{3}", sUpDir, row["UserID"], row["AlbumID"], row["FileName"]));
                        string newFileName = context.Server.MapPath(String.Format("{0}/{1}.{2}.{3}.yafalbum", sUpDir, row["UserID"], row["AlbumID"], row["FileName"]));

                        string fileName = string.Empty;

                        // use the new fileName (with extension) if it exists...
                        if (File.Exists(newFileName)) fileName = newFileName;
                        else fileName = oldFileName;
                        using (System.IO.FileStream input = new System.IO.FileStream(fileName, System.IO.FileMode.Open))
                        {
                            data = new byte[input.Length];
                            input.Read(data, 0, data.Length);
                            input.Close();
                        }


                        context.Response.ContentType = row["ContentType"].ToString();
                        //context.Response.Cache.SetCacheability(HttpCacheability.Public);
                        //context.Response.Cache.SetETag(eTag);
                        context.Response.OutputStream.Write(data, 0, data.Length);

                        // add a download count...
                        YAF.Classes.Data.DB.album_image_download(context.Request.QueryString["image"]);
                        break;
                    }
                }
            }
            catch (Exception x)
            {
                YAF.Classes.Data.DB.eventlog_create(null, this.GetType().ToString(), x, 1);
                context.Response.Write("Error: Resource has been moved or is unavailable. Please contact the forum admin.");
            }
        }
                
		/// <summary>
		/// Check if the ETag that sent from the client is match to the current ETag.
		/// If so, set the status code to 'Not Modified' and stop the response.
		/// </summary>
		private static bool CheckETag( HttpContext context, string eTagCode )
		{
			string ifNoneMatch = context.Request.Headers["If-None-Match"];
			if ( eTagCode.Equals( ifNoneMatch, StringComparison.Ordinal ) )
			{
				context.Response.AppendHeader( "Content-Length", "0" );
				context.Response.StatusCode = (int)HttpStatusCode.NotModified;
				context.Response.StatusDescription = "Not modified";
				context.Response.SuppressContent = true;
				context.Response.Cache.SetCacheability( HttpCacheability.Public );
				context.Response.Cache.SetETag( eTagCode );
				context.Response.Flush();
				return true;
			}

			return false;
		}		

		private void GetResponseGoogleSpell(HttpContext context)
		{
			string url = string.Format("https://www.google.com/tbproxy/spell?lang={0}", context.Request.QueryString["lang"]);

			System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
			webRequest.KeepAlive = true;
			webRequest.Timeout = 100000;
			webRequest.Method = "POST";
			webRequest.ContentType = "application/x-www-form-urlencoded";
			webRequest.ContentLength = context.Request.InputStream.Length;

			System.IO.Stream requestStream = webRequest.GetRequestStream();

			CopyStream(context.Request.InputStream, requestStream);

			requestStream.Close();

			System.Net.HttpWebResponse httpWebResponse = (System.Net.HttpWebResponse)webRequest.GetResponse();
			System.IO.Stream responseStream = httpWebResponse.GetResponseStream();

			CopyStream(responseStream, context.Response.OutputStream);
		}

		private void CopyStream(System.IO.Stream input, System.IO.Stream output)
		{
			byte[] buffer = new byte[1024];
			int count = buffer.Length;

			while (count > 0)
			{
				count = input.Read(buffer, 0, count);
				if (count > 0) output.Write(buffer, 0, count);
			}
		}

		private void GetResponseCaptcha(HttpContext context)
		{
			try
			{
				YAF.Classes.UI.CaptchaImage captchaImage = new YAF.Classes.UI.CaptchaImage(context.Session["CaptchaImageText"].ToString(), 200, 50, "Century Schoolbook");
				context.Response.Clear();
				context.Response.ContentType = "image/jpeg";
				captchaImage.Image.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
			}
			catch (Exception x)
			{
				YAF.Classes.Data.DB.eventlog_create(null, this.GetType().ToString(), x, 1);
				context.Response.Write("Error: Resource has been moved or is unavailable. Please contact the forum admin.");
			}
		}

		private bool CheckAccessRights(object boardID, object messageID)
		{
			// Find user name
			System.Web.Security.MembershipUser user = System.Web.Security.Membership.GetUser();

			string browser = String.Format("{0} {1}", HttpContext.Current.Request.Browser.Browser, HttpContext.Current.Request.Browser.Version);
			string platform = HttpContext.Current.Request.Browser.Platform;
			bool isSearchEngine = false;

			if (HttpContext.Current.Request.UserAgent != null)
			{
				if (HttpContext.Current.Request.UserAgent.IndexOf("Windows NT 5.2") >= 0)
				{
					platform = "Win2003";
				}
				else if (HttpContext.Current.Request.UserAgent.IndexOf("Windows NT 6.0") >= 0)
				{
					platform = "Vista";
				}
				else
				{
					// check if it's a search engine spider...
					isSearchEngine = UserAgentHelper.IsSearchEngineSpider(HttpContext.Current.Request.UserAgent);
				}
			}
			
			YafServices.InitializeDb.Run();
			
			object userKey = DBNull.Value;

			if (user != null)
			{
				userKey = user.ProviderUserKey;
			}     
             
			DataRow pageRow = YAF.Classes.Data.DB.pageload(
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
                    YafContext.Current.BoardSettings.EnableBuddyList, 
                    YafContext.Current.BoardSettings.AllowPrivateMessages, 
                    YafContext.Current.BoardSettings.UseStyledNicks);

			return General.BinaryAnd(pageRow["DownloadAccess"], YAF.Classes.Data.AccessFlags.Flags.DownloadAccess) ||
                General.BinaryAnd(pageRow["ModeratorAccess"], YAF.Classes.Data.AccessFlags.Flags.ModeratorAccess);
		}

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}
	}
}
