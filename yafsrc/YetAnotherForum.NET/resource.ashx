<%@ WebHandler Language="C#" Class="YAF.resource" %>
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

using YAF.Classes.Utils;

namespace YAF
{
    /// <summary>
    /// Summary description for $Codefileclassname$
    /// </summary>
    [WebService( Namespace = "http://www.yetanotherforum.net/" )]
    [WebServiceBinding( ConformsTo = WsiProfiles.BasicProfile1_1 )]
    public class resource : IHttpHandler, IReadOnlySessionState
    {
        public void ProcessRequest( HttpContext context )
        {
            // resource no longer works with dynamic compile...
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
                else if ( context.Request.QueryString ["c"] != null && context.Session ["CaptchaImageText"] != null )
                {
                    // captcha					
                    GetResponseCaptcha( context );
                }
                else if ( context.Request.QueryString ["s"] != null && context.Request.QueryString ["lang"] != null )
                {
                    GetResponseGoogleSpell( context );
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
            // redirect to the resource?
            context.Response.Redirect( "resources/" + context.Request.QueryString ["r"] );

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

        private void GetResponseLocalAvatar( HttpContext context )
        {
            using ( DataTable dt = YAF.Classes.Data.DB.user_avatarimage( context.Request.QueryString ["u"] ) )
            {
                foreach ( DataRow row in dt.Rows )
                {
                    byte [] data = ( byte [] )row ["AvatarImage"];
                    string contentType = row ["AvatarImageType"].ToString();

                    context.Response.Clear();
                    if ( !String.IsNullOrEmpty( contentType ) )
                    {
                        context.Response.ContentType = contentType;
                    }
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
                    height = Convert.ToInt32( ( double )height / ( double )width * ( double )maxwidth );
                    width = maxwidth;
                }
                if ( height > maxheight )
                {
                    width = Convert.ToInt32( ( double )width / ( double )height * ( double )maxheight );
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
                        /// TODO : check download permissions here					
                        if ( !CheckAccessRights( row ["BoardID"], row ["MessageID"] ) )
                        {
                            // tear it down
                            // no permission to download
                            context.Response.Write( "You have insufficient rights to download this resource. Contact forum administrator for further details." );
                            return;
                        }

                        byte [] data = null;

                        if ( row.IsNull( "FileData" ) )
                        {
                            string sUpDir = YAF.Classes.Config.UploadDir;
                            
                            string oldFileName = context.Server.MapPath( String.Format( "{0}{1}.{2}", sUpDir, row ["MessageID"], row ["FileName"] ) );
                            string newFileName = context.Server.MapPath(String.Format("{0}{1}.{2}.yafupload", sUpDir, row["MessageID"], row["FileName"]));
                            
                            string fileName = string.Empty;

                            // use the new fileName (with extension) if it exists...
                            if (File.Exists(newFileName)) fileName = newFileName;
                            else fileName = oldFileName;
                                                        
                            using ( System.IO.FileStream input = new System.IO.FileStream( fileName, System.IO.FileMode.Open ) )
                            {
                                data = new byte [input.Length];
                                input.Read( data, 0, data.Length );
                                input.Close();
                            }
                        }
                        else
                        {
                            data = ( byte [] )row ["FileData"];
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

        private void GetResponseGoogleSpell( HttpContext context )
        {
            string url = string.Format( "https://www.google.com/tbproxy/spell?lang={0}", context.Request.QueryString ["lang"] );

            System.Net.HttpWebRequest webRequest = ( System.Net.HttpWebRequest )System.Net.WebRequest.Create( url );
            webRequest.KeepAlive = true;
            webRequest.Timeout = 100000;
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ContentLength = context.Request.InputStream.Length;

            System.IO.Stream requestStream = webRequest.GetRequestStream();

            CopyStream( context.Request.InputStream, requestStream );

            requestStream.Close();

            System.Net.HttpWebResponse httpWebResponse = ( System.Net.HttpWebResponse )webRequest.GetResponse();
            System.IO.Stream responseStream = httpWebResponse.GetResponseStream();

            CopyStream( responseStream, context.Response.OutputStream );
        }

        private void CopyStream( System.IO.Stream input, System.IO.Stream output )
        {
            byte [] buffer = new byte [1024];
            int count = buffer.Length;

            while ( count > 0 )
            {
                count = input.Read( buffer, 0, count );
                if ( count > 0 ) output.Write( buffer, 0, count );
            }
        }

        private void GetResponseCaptcha( HttpContext context )
        {
            try
            {
                YAF.Classes.UI.CaptchaImage captchaImage = new YAF.Classes.UI.CaptchaImage( context.Session ["CaptchaImageText"].ToString(), 200, 50, "Century Schoolbook" );
                context.Response.Clear();
                context.Response.ContentType = "image/jpeg";
                captchaImage.Image.Save( context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg );
            }
            catch ( Exception x )
            {
                YAF.Classes.Data.DB.eventlog_create( null, this.GetType().ToString(), x, 1 );
                context.Response.Write( "Error: Resource has been moved or is unavailable. Please contact the forum admin." );
            }
        }

        private bool CheckAccessRights( object boardID, object messageID )
        {
            YAF.Classes.Utils.YafContext context = new YAF.Classes.Utils.YafContext();
            // Find user name
            System.Web.Security.MembershipUser user = System.Web.Security.Membership.GetUser();

            string browser = String.Format( "{0} {1}", HttpContext.Current.Request.Browser.Browser, HttpContext.Current.Request.Browser.Version );
            string platform = HttpContext.Current.Request.Browser.Platform;
            bool isSearchEngine = false;

            if ( HttpContext.Current.Request.UserAgent != null )
            {
                if ( HttpContext.Current.Request.UserAgent.IndexOf( "Windows NT 5.2" ) >= 0 )
                {
                    platform = "Win2003";
                }
                else if ( HttpContext.Current.Request.UserAgent.IndexOf( "Windows NT 6.0" ) >= 0 )
                {
                    platform = "Vista";
                }
                else
                {
                    // check if it's a search engine spider...
                    isSearchEngine = General.IsSearchEngineSpider( HttpContext.Current.Request.UserAgent );
                }
            }

            object userKey = DBNull.Value;

            if ( user != null )
            {
                userKey = user.ProviderUserKey;
            }

            DataRow pageRow = YAF.Classes.Data.DB.pageload(
                HttpContext.Current.Session.SessionID,
                boardID,
                userKey,
                HttpContext.Current.Request.UserHostAddress,
                HttpContext.Current.Request.FilePath,
                browser,
                platform,
                null,
                null,
                null,
                messageID,
                // don't track if this is a search engine
                isSearchEngine );

            return General.BinaryAnd( pageRow ["DownloadAccess"], YAF.Classes.Data.AccessFlags.Flags.DownloadAccess ) ||
                General.BinaryAnd( pageRow ["ModeratorAccess"], YAF.Classes.Data.AccessFlags.Flags.ModeratorAccess );
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
