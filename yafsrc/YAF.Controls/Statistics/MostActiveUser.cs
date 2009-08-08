/* Yet Another Forum.NET
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
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Data;
using YAF.Classes.Utils;

namespace YAF.Controls.Statistics
{
	[ToolboxData( "<{0}:MostActiveUsers runat=\"server\"></{0}:MostActiveUsers>" )]
	public class MostActiveUsers : BaseControl
	{
		private int _displayNumber = 10;
		private int _lastNumOfDays = 7;

		/// <summary>
		/// The default constructor for MostActiveUsers.
		/// </summary>
		public MostActiveUsers()
		{
		}

		/// <summary>
		/// Renders the MostActiveUsers class.
		/// </summary>
		/// <param name="writer"></param>
		protected override void Render( System.Web.UI.HtmlTextWriter writer )
		{
			int currentRank = 1;
			string actRank = "";
			string cacheKey = YafCache.GetBoardCacheKey( Constants.Cache.ActiveUsers );

			DataTable rankDT = YafCache.Current [cacheKey] as DataTable;

			if ( rankDT == null )
			{
				rankDT = YAF.Classes.Data.DB.user_activity_rank( PageContext.PageBoardID, DateTime.Now.AddDays( -LastNumOfDays ), DisplayNumber );
				YafCache.Current.Insert( cacheKey, rankDT, null, DateTime.Now.AddMinutes( 10 ), TimeSpan.Zero );
			}

			actRank += "<table width=\"100%\" class=\"content\" cellspacing=\"1\" border=\"0\" cellpadding=\"0\">";
			actRank += "<tr><td class=\"header1\">Most Active Users</td></tr>";
			actRank += String.Format("<tr><td class=\"header2\">Last {0} Days</td></tr>", LastNumOfDays);
			actRank += @"<tr class=""post""><td>";

			foreach ( System.Data.DataRow r in rankDT.Rows )
			{			
				int userID = Convert.ToInt32(r["ID"]);
				actRank += string.Format( @"{3}.&nbsp;<a href=""{1}"">{0}</a> ({2})<br/>", r ["Name"], YafBuildLink.GetLink( ForumPages.profile, "u={0}", userID ), r ["NumOfPosts"], currentRank );
				currentRank++;		
			}

			actRank += "</td></tr></table>";

			writer.Write( actRank );
		}

		public int DisplayNumber
		{
			get { return _displayNumber; }
			set { _displayNumber = value; }
		}

		public int LastNumOfDays
		{
			get { return _lastNumOfDays; }
			set { _lastNumOfDays = value; }
		}
	}
}
