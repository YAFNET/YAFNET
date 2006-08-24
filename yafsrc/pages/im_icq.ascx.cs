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
	/// Summary description for active.
	/// </summary>
	public partial class im_icq : ForumPage
	{

		public im_icq() : base("IM_ICQ")
		{
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(User==null)
				Data.AccessDenied();

			if(!IsPostBack) 
			{
				Send.Text = GetText("SEND");
				From.Text = PageUserName;
				using(DataTable dt=DB.user_list(PageBoardID,Request.QueryString["u"],null)) 
				{
					foreach(DataRow row in dt.Rows) 
					{
						PageLinks.AddLink(BoardSettings.Name,Forum.GetLink(Pages.forum));
						PageLinks.AddLink(row["Name"].ToString(),Forum.GetLink(Pages.profile,"u={0}",row["UserID"]));
						PageLinks.AddLink(GetText("TITLE"),"");
						ViewState["to"] = (int)row["ICQ"];
						Status.Src = string.Format("http://web.icq.com/whitepages/online?icq={0}&img=5",row["ICQ"]);
						break;
					}
				}
				using(DataTable dt=DB.user_list(PageBoardID,PageUserID,null))
				{
					foreach(DataRow row in dt.Rows)
					{
						Email.Text = row["Email"].ToString();
						break;
					}
				}
			}
		}

		private void Send_Click(object sender,EventArgs e)
		{
			string html = string.Format("http://wwp.icq.com/scripts/WWPMsg.dll?from={0}&fromemail={1}&subject={2}&to={3}&body={4}",
				Server.UrlEncode(From.Text),
				Server.UrlEncode(Email.Text),
				Server.UrlEncode("From WebPager Panel"),
				ViewState["to"],
				Server.UrlEncode(Body.Text)
				);
			Response.Redirect(html);
		}

		override protected void OnInit(EventArgs e)
		{
			this.Load += new System.EventHandler(this.Page_Load);
			this.Send.Click += new EventHandler(Send_Click);
			base.OnInit(e);
		}
	}
}
