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
	/// Summary description for cp_signature.
	/// </summary>
	public class cp_signature : BasePage
	{
		protected Button save, cancel;
		protected rte.rte sig;
		protected controls.PageLinks PageLinks;

		private void Page_Load(object sender, System.EventArgs e)
		{
			sig.EnableRTE = AllowRichEdit;

			if(!User.Identity.IsAuthenticated)
				Response.Redirect(String.Format("login.aspx?ReturnUrl={0}",Request.RawUrl));

			if(!IsPostBack) {
				string msg = DB.user_getsignature(PageUserID);
				bool isHtml = msg.IndexOf('<')>=0;
				if(sig.IsRTEBrowser && !isHtml)
					msg = FormatMsg.ForumCodeToHtml(this,msg);
				else if(!sig.IsRTEBrowser && isHtml)
					msg = FormatMsg.HtmlToForumCode(msg);
				sig.Text = msg;

				PageLinks.AddLink(ForumName,BaseDir);
				PageLinks.AddLink(PageUserName,"cp_profile.aspx");
				PageLinks.AddLink(GetText("TITLE"),Request.RawUrl);

				save.Text = GetText("Save");
				cancel.Text = GetText("Cancel");
			}
		}

		private void save_Click(object sender,EventArgs e) {
			string body = sig.Text;
			if(!sig.IsRTEBrowser)
				body = FormatMsg.ForumCodeToHtml(this,Server.HtmlEncode(body));
			else
				body = FormatMsg.RepairHtml(this,body);

			DB.user_savesignature(PageUserID,body);
			Response.Redirect("cp_profile.aspx");
		}

		private void cancel_Click(object sender,EventArgs e) {
			Response.Redirect("cp_profile.aspx");
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
