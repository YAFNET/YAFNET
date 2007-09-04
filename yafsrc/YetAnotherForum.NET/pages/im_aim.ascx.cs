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
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages // YAF.Pages
{
	/// <summary>
	/// Summary description for active.
	/// </summary>
	public partial class im_aim : YAF.Classes.Base.ForumPage
	{

		public im_aim() : base("IM_AIM")
		{
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(User==null)
				YafBuildLink.AccessDenied();

			if(!IsPostBack) {
				using(DataTable dt=YAF.Classes.Data.DB.user_list(PageContext.PageBoardID,Request.QueryString["u"],null)) 
				{
					foreach(DataRow row in dt.Rows) 
					{
						PageLinks.AddLink(PageContext.BoardSettings.Name,YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum));
						PageLinks.AddLink(row["Name"].ToString(),YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.profile,"u={0}",row["UserID"]));
						PageLinks.AddLink(GetText("TITLE"),"");

						Msg.NavigateUrl = string.Format("aim:goim?screenname={0}&message=Hi.+Are+you+there?",row["AIM"]);
						Buddy.NavigateUrl = string.Format("aim:addbuddy?screenname={0}",row["AIM"]);
						break;
					}
				}
			}
		}

		override protected void OnInit(EventArgs e)
		{
			this.Load += new System.EventHandler(this.Page_Load);
			base.OnInit(e);
		}
	}
}
