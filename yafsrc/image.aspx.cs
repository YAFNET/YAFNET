/* Yet Another Forum.net
 * Copyright (C) 2003 Bjørnar Henden
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

using System;
using System.Configuration;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace yaf
{
	/// <summary>
	/// Summary description for image.
	/// </summary>
    public class image : System.Web.UI.Page
	{
		private void Page_Load(object sender, System.EventArgs e)
		{
			if(Request.QueryString["u"]!=null) 
			{
				using(DataTable dt = DB.user_avatarimage(Request.QueryString["u"])) 
				{
					foreach(DataRow row in dt.Rows) 
					{
						byte[] data = (byte[])row["AvatarImage"];

						Response.Clear();
						//Response.ContentType = "image/jpg";
						Response.OutputStream.Write(data,0,data.Length);
						Response.End();
						break;
					}
				}
			}
			else if(Request.QueryString["url"] != null && Request.QueryString["width"] != null && Request.QueryString["height"]!=null) 
			{
				System.Net.WebClient wc = new System.Net.WebClient();
				Image img = null;
				Bitmap bmp = null;
				Graphics gfx = null;
				
				string wb = Request.QueryString["url"];
				wb.Substring(0, wb.LastIndexOf("/"));

				try
				{
					Stream input = wc.OpenRead(wb);
					img = new Bitmap(input);
					input.Close();
					
					int	maxwidth	= int.Parse(Request.QueryString["width"]);
					int	maxheight	= int.Parse(Request.QueryString["height"]);
					int	width		= img.Width;
					int	height		= img.Height;

					if(width <= maxwidth && height <= maxheight) 
						Response.Redirect(wb);

					if(width > maxwidth) 
					{
						height = Convert.ToInt32((double)height / (double)width * (double)maxwidth);
						width = maxwidth;
					}
					if(height > maxheight) 
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
					Response.ContentType = "image/jpeg";                            
					bmp.Save(Response.OutputStream, ImageFormat.Jpeg);
            
				}
				finally
				{
					if(gfx!=null) gfx.Dispose();
					if(img!=null) img.Dispose();
					if(bmp!=null) bmp.Dispose();
				}
			} 
			else if(Request.QueryString["a"]!=null) 
			{
				// AttachmentID
				using(DataTable dt = DB.attachment_list(null,Request.QueryString["a"],null)) 
				{
					foreach(DataRow row in dt.Rows) 
					{
						byte[] data = null;
						Response.ContentType = HttpUtility.UrlEncode(row["ContentType"].ToString());
						Response.AppendHeader("Content-Disposition",String.Format("attachment; filename={0}",HttpUtility.UrlEncode(row["FileName"].ToString()).Replace("+","%20")));

						if(row.IsNull("FileData")) 
						{
							string sUpDir = Config.ConfigSection["uploaddir"];
							string fileName = Server.MapPath(String.Format("{0}{1}.{2}",sUpDir,row["MessageID"],row["FileName"]));
							using(System.IO.FileStream input = new System.IO.FileStream(fileName,System.IO.FileMode.Open)) 
							{
								data = new byte[input.Length];
								input.Read(data,0,data.Length);
								input.Close();
							}
						} 
						else 
						{
							data = (byte[])row["FileData"];
						}
						Response.OutputStream.Write(data,0,data.Length);
						DB.attachment_download(Request.QueryString["a"]);
						Response.End();
						break;
					}
				}
			}
		}

		#region Web Form Designer generated code
		/// <summary>
		/// The main initialization of the page.
		/// </summary>
		/// <param name="e">The EventArgs object inherit from Page.</param>
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
}
