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

namespace yaf
{
	/// <summary>
	/// Summary description for approve.
	/// </summary>
	public class approve : BasePage
	{
		protected System.Web.UI.HtmlControls.HtmlTable approved;
		protected System.Web.UI.HtmlControls.HtmlTable error;
		protected System.Web.UI.WebControls.TextBox k;
		protected System.Web.UI.WebControls.Button ValidateKey;
		protected System.Web.UI.WebControls.HyperLink HomeLink;
		
		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsPostBack) {
				HomeLink.Text = ForumName;
				HomeLink.NavigateUrl = BaseDir;
			}

			if(Request.QueryString["k"] != null)
				approved.Visible = DB.checkemail_update(Request.QueryString["k"]);

			error.Visible = !approved.Visible;
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
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
	}
}
