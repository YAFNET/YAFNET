/* Copyright (C) 2003 Bjørnar Henden
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
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace yaf
{
	/// <summary>
	/// Summary description for members.
	/// </summary>
	public class members : BasePage
	{
		protected System.Web.UI.WebControls.Repeater MemberList;
		protected System.Web.UI.WebControls.HyperLink HomeLink;
		protected System.Web.UI.HtmlControls.HtmlTableCell PageLinks1;
		protected System.Web.UI.HtmlControls.HtmlTableCell PageLinks2;

		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!User.Identity.IsAuthenticated)
				Response.Redirect(String.Format("login.aspx?ReturnUrl={0}",Request.RawUrl));

			int CurrentPage = 0;
			if(Request.QueryString["p"] != null)
				CurrentPage = int.Parse(Request.QueryString["p"]);


			HomeLink.Text = ForumName;
			HomeLink.NavigateUrl = BaseDir;

			PagedDataSource pds = new PagedDataSource();
			pds.DataSource = DataManager.GetData("yaf_user_list",CommandType.Text).DefaultView;
			pds.AllowPaging = true;
			pds.CurrentPageIndex = CurrentPage;
			pds.PageSize = 10;
			
			MemberList.DataSource = pds;
			DataBind();

			if(pds.PageCount>1) {
				PageLinks1.InnerHtml = String.Format("{0} Pages:",pds.PageCount);
				for(int i=0;i<pds.PageCount;i++) {
					if(i==pds.CurrentPageIndex) {
						PageLinks1.InnerHtml += String.Format(" [{0}]",i+1);
					} else {
						PageLinks1.InnerHtml += String.Format(" <a href=\"members.aspx?p={1}\">{0}</a>",i+1,i);
					}
				}
				PageLinks2.InnerHtml = PageLinks1.InnerHtml;
			} else {
				PageLinks1.Visible = false;
				PageLinks2.Visible = false;
			}
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
