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
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace yaf.admin
{
	/// <summary>
	/// Summary description for bannedip_edit.
	/// </summary>
	public class bannedip_edit : BaseAdminPage
	{
		protected TextBox mask;
		protected Button save, cancel;

		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsPostBack) {
				BindData();
			}
		}

		private void BindData() {
			if(Request.QueryString["i"] != null) {
				DataRow row = DB.bannedip_list(PageBoardID,Request.QueryString["i"]).Rows[0];
				mask.Text = (string)row["Mask"];
			}
		}

		private void save_Click(object sender,EventArgs e) {
			String[] ip = mask.Text.Split('.');
			if(ip.Length!=4) {
				AddLoadMessage("Invalid ip address.");
				return;
			}
			DB.bannedip_save(Request.QueryString["i"],PageBoardID,mask.Text);
			Cache.Remove("bannedip");
			Response.Redirect("bannedip.aspx");
		}

		private void cancel_Click(object sender,EventArgs e) {
			Response.Redirect("bannedip.aspx");
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			save.Click += new EventHandler(save_Click);
			cancel.Click += new EventHandler(cancel_Click);
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
