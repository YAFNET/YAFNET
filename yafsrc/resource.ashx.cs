using System;
using System.Collections;
using System.Data;
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

namespace yaf
{
	/// <summary>
	/// Summary description for $codebehindclassname$
	/// </summary>
	[WebService( Namespace = "http://www.yetanotherforum.net/" )]
	[WebServiceBinding( ConformsTo = WsiProfiles.BasicProfile1_1 )]
	public class resource : IHttpHandler, IReadOnlySessionState
	{
		public void ProcessRequest( HttpContext context )
		{
			if ( context.Request.QueryString ["r"] != null )
			{
				// resource request
				GetResource( context );

			}
			else if ( context.Session ["lastvisit"] != null )
			{
				if ( context.Request.QueryString ["u"] != null )
				{
					GetResponseLocalAvatar( context );
				}
				else if ( context.Request.QueryString ["url"] != null && context.Request.QueryString ["width"] != null && context.Request.QueryString ["height"] != null )
				{
					GetResponseRemoteAvatar( context );
				}
				else if ( context.Request.QueryString ["a"] != null )
				{
					GetResponseAttachment( context );
				}
			}
			else
			{
				// they don't have a session...
				context.Response.Write( "Please do not link directly to this resource. You must have a session in the forum." );
			}
		}

		private void GetResource( HttpContext context )
		{
			string resourceName = "YAF.App_GlobalResources." + context.Request.QueryString ["r"];
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
		}

		private void GetResponseLocalAvatar( HttpContext context )
		{
			using ( DataTable dt = YAF.Classes.Data.DB.user_avatarimage( context.Request.QueryString ["u"] ) )
			{
				foreach ( DataRow row in dt.Rows )
				{
					byte [] data = ( byte [] ) row ["AvatarImage"];

					context.Response.Clear();
					//Response.ContentType = "image/jpg";
					context.Response.OutputStream.Write( data, 0, data.Length );
					break;
				}
			}
		}

		private void GetResponseRemoteAvatar( HttpContext context )
		{
			System.Net.WebClient wc = new System.Net.WebClient();
			Image img = null;
			Bitmap bmp = null;
			Graphics gfx = null;

			string wb = context.Request.QueryString ["url"];
			wb.Substring( 0, wb.LastIndexOf( "/" ) );

			try
			{
				Stream input = wc.OpenRead( wb );
				img = new Bitmap( input );
				input.Close();

				int maxwidth = int.Parse( context.Request.QueryString ["width"] );
				int maxheight = int.Parse( context.Request.QueryString ["height"] );
				int width = img.Width;
				int height = img.Height;

				if ( width <= maxwidth && height <= maxheight )
					context.Response.Redirect( wb );

				if ( width > maxwidth )
				{
					height = Convert.ToInt32( ( double ) height / ( double ) width * ( double ) maxwidth );
					width = maxwidth;
				}
				if ( height > maxheight )
				{
					width = Convert.ToInt32( ( double ) width / ( double ) height * ( double ) maxheight );
					height = maxheight;
				}

				// Create the target bitmap
				bmp = new Bitmap( width, height );

				// Create the graphics object to do the high quality resizing
				gfx = Graphics.FromImage( bmp );
				gfx.CompositingQuality = CompositingQuality.HighQuality;
				gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
				gfx.SmoothingMode = SmoothingMode.HighQuality;

				// Draw the source image
				gfx.DrawImage( img, new Rectangle( new Point( 0, 0 ), new Size( width, height ) ) );

				// Output the data
				context.Response.ContentType = "image/jpeg";
				bmp.Save( context.Response.OutputStream, ImageFormat.Jpeg );

			}
			finally
			{
				if ( gfx != null ) gfx.Dispose();
				if ( img != null ) img.Dispose();
				if ( bmp != null ) bmp.Dispose();
			}
		}

		private void GetResponseAttachment( HttpContext context )
		{
			try
			{
				// AttachmentID
				using ( DataTable dt = YAF.Classes.Data.DB.attachment_list( null, context.Request.QueryString ["a"], null ) )
				{
					foreach ( DataRow row in dt.Rows )
					{
						byte [] data = null;

						if ( row.IsNull( "FileData" ) )
						{
							string sUpDir = YAF.Classes.Config.UploadDir;
							string fileName = context.Server.MapPath( String.Format( "{0}{1}.{2}", sUpDir, row ["MessageID"], row ["FileName"] ) );
							using ( System.IO.FileStream input = new System.IO.FileStream( fileName, System.IO.FileMode.Open ) )
							{
								data = new byte [input.Length];
								input.Read( data, 0, data.Length );
								input.Close();
							}
						}
						else
						{
							data = ( byte [] ) row ["FileData"];
						}

						context.Response.ContentType = row ["ContentType"].ToString();
						context.Response.AppendHeader( "Content-Disposition", String.Format( "attachment; filename={0}", HttpUtility.UrlEncode( row ["FileName"].ToString() ).Replace( "+", "%20" ) ) );
						context.Response.OutputStream.Write( data, 0, data.Length );
						YAF.Classes.Data.DB.attachment_download( context.Request.QueryString ["a"] );
						break;
					}
				}
			}
			catch ( Exception x )
			{
				YAF.Classes.Data.DB.eventlog_create( null, this.GetType().ToString(), x, 1 );
				context.Response.Write( "Error: Resource has been moved or is unavailable. Please contact the forum admin." );
			}
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
