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
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Security;
using System.Globalization;

namespace yaf
{
	/// <summary>
	/// Summary description for cp_editprofile.
	/// </summary>
	public class cp_avatar : BasePage
	{
		protected System.Web.UI.WebControls.Button UpdateProfile;
		protected System.Web.UI.HtmlControls.HtmlInputFile File;

		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!User.Identity.IsAuthenticated)
				Response.Redirect(String.Format("login.aspx?ReturnUrl={0}",Request.RawUrl));
		}

		private void UpdateProfile_Click(object sender, System.EventArgs e) {
			if(File.PostedFile==null || File.PostedFile.FileName.Trim().Length==0 || File.PostedFile.ContentLength==0)
				return;

			long x,y;
			using(DataTable dt = DataManager.GetData("yaf_system_list",CommandType.StoredProcedure)) 
			{
				x = long.Parse(dt.Rows[0]["AvatarWidth"].ToString());
				y = long.Parse(dt.Rows[0]["AvatarHeight"].ToString());
			}


			System.Drawing.Image img = System.Drawing.Image.FromStream(File.PostedFile.InputStream);
			if(img.Width>x || img.Height>y) 
			{
				AddLoadMessage(String.Format("Image size can't be larger than {0}x{1} pixels.",x,y));
				AddLoadMessage(String.Format("The size of your image was {0}x{1} pixels.",img.Width,img.Height));
				return;
			}

			using(SqlCommand cmd = new SqlCommand("yaf_user_avatarimage",DataManager.GetConnection())) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",PageUserID);

				using(SqlDataAdapter da = new SqlDataAdapter(cmd)) 
				{
					using(SqlCommandBuilder cb = new SqlCommandBuilder(da)) 
					{
						using(DataSet ds = new DataSet()) 
						{
							byte[] data = new byte[File.PostedFile.InputStream.Length];
							File.PostedFile.InputStream.Seek(0,System.IO.SeekOrigin.Begin);
							File.PostedFile.InputStream.Read(data,0,(int)File.PostedFile.InputStream.Length);

							da.Fill(ds);
							ds.Tables[0].Rows[0]["AvatarImage"] = data;
							da.Update(ds);
						}
					}
				}
			}
			Response.Redirect("cp_profile.aspx");
		}

		#region Web Form Designer generated code
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
			this.UpdateProfile.Click += new System.EventHandler(this.UpdateProfile_Click);
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
}
