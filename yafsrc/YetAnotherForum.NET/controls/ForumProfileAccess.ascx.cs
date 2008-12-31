/* Yet Another Forum.NET
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
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Controls
{
	public partial class ForumProfileAccess : YAF.Classes.Base.BaseUserControl
	{
		protected void Page_Load( object sender, EventArgs e )
		{
			if ( PageContext.IsAdmin || PageContext.IsForumModerator )
			{
				int userID = ( int ) Security.StringToLongOrRedirect( Request.QueryString ["u"] );

				using ( DataTable dt2 = YAF.Classes.Data.DB.user_accessmasks( PageContext.PageBoardID, userID ) )
				{
					System.Text.StringBuilder html = new System.Text.StringBuilder();
					int nLastForumID = 0;
					foreach ( DataRow row in dt2.Rows )
					{
						if ( nLastForumID != Convert.ToInt32( row ["ForumID"] ) )
						{
							if ( nLastForumID != 0 )
								html.AppendFormat( "</td></tr>" );
							html.AppendFormat( "<tr><td width='50%' class='postheader'>{0}</td><td width='50%' class='post'>", row ["ForumName"] );
							nLastForumID = Convert.ToInt32( row ["ForumID"] );
						}
						else
						{
							html.AppendFormat( ", " );
						}
						html.AppendFormat( "{0}", row ["AccessMaskName"] );
					}
					if ( nLastForumID != 0 )
						html.AppendFormat( "</td></tr>" );
					AccessMaskRow.Text = html.ToString();
				}
			}
		}
	}
}