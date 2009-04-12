/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2009 Jaben Cargman
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
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace YAF.Pages
{
	public partial class showsmilies : YAF.Classes.Base.ForumPage
	{
		// constructor
		public showsmilies() : base("SHOWSMILIES") { }

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack && !PageContext.IsGuest)
			{
				ShowToolBar = false;

				try
				{
					Parent.Parent.FindControl("imgBanner").Visible = false;
				}
				catch
				{
					// control is nested more than anticipated
				}

				BindData();
			}
		}

		private void BindData()
		{
			List.DataSource = YAF.Classes.Data.DB.smiley_listunique(PageContext.PageBoardID);
			DataBind();
		}

		protected string GetSmileyScript(string code, string icon)
		{
			code = code.ToLower();
			code = code.Replace("&", "&amp;");
			code = code.Replace("\"", "&quot;");
			code = code.Replace("'", "\\'");

			return String.Format( "javascript:{0}('{1} ','{3}images/emoticons/{2}');", "insertsmiley", code, icon, YAF.Classes.Utils.YafForumInfo.ForumRoot );
		}
	}
}