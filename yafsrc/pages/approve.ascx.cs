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

namespace yaf.pages
{
	/// <summary>
	/// Summary description for approve.
	/// </summary>
	public class approve : ForumPage
	{
		protected System.Web.UI.HtmlControls.HtmlTable approved;
		protected System.Web.UI.HtmlControls.HtmlTable error;
		protected System.Web.UI.WebControls.TextBox key;
		protected System.Web.UI.WebControls.Button ValidateKey;
		protected controls.PageLinks PageLinks;
		
		public approve() : base("APPROVE")
		{
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsPostBack) {
				PageLinks.AddLink(Config.BoardSettings.Name,Forum.GetLink(Pages.forum));
				PageLinks.AddLink(GetText("TITLE"),Request.RawUrl);
				ValidateKey.Text = GetText("validate");
				if(Request.QueryString["k"]!=null)
				{
					key.Text = Request.QueryString["k"];
					ValidateKey_Click(sender,e);
				}
				else
				{
					approved.Visible = false;
					error.Visible = !approved.Visible;
				}
			}
		}

		private void ValidateKey_Click(object sender, System.EventArgs e)
		{
			approved.Visible = DataProvider.checkemail_update(key.Text);
			error.Visible = !approved.Visible;
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			ValidateKey.Click += new EventHandler(ValidateKey_Click);
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
