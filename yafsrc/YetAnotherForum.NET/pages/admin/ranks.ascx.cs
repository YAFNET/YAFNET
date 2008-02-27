/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2008 Jaben Cargman
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

namespace YAF.Pages.Admin
{
	/// <summary>
	/// Summary description for ranks.
	/// </summary>
	public partial class ranks : YAF.Classes.Base.AdminPage
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsPostBack) 
			{
				PageLinks.AddLink(PageContext.BoardSettings.Name,YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum));
				PageLinks.AddLink("Administration",YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.admin_admin));
				PageLinks.AddLink("Ranks","");

				BindData();
			}
		}

		protected void Delete_Load(object sender, System.EventArgs e) 
		{
			General.AddOnClickConfirmDialog(sender, "Delete this rank?");
		}

		private void BindData() 
		{
			RankList.DataSource = YAF.Classes.Data.DB.rank_list(PageContext.PageBoardID,null);
			DataBind();
		}

		protected void RankList_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
		{
			switch(e.CommandName) 
			{
				case "edit":
					YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_editrank,"r={0}",e.CommandArgument);
					break;
				case "delete":
					YAF.Classes.Data.DB.rank_delete(e.CommandArgument);
					BindData();
					break;
			}
		}

		protected void NewRank_Click(object sender, System.EventArgs e)
		{
			YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_editrank);
		}

		protected string LadderInfo(object _o) {
			DataRowView dr = (DataRowView)_o;

			///object IsLadder,object MinPosts
			///Eval( "IsLadder"),Eval( "MinPosts")

			bool isLadder = General.BinaryAnd(dr["Flags"], RankFlags.Flags.IsLadder);
			
			string tmp = String.Format("{0}",isLadder);
			if(isLadder) {
				tmp += String.Format(" ({0} posts)",dr["MinPosts"]);
			}
			return tmp;
		}

		protected bool BitSet(object _o,int bitmask) 
		{
			int i = (int)_o;
			return (i & bitmask)!=0;
		}
	}
}
