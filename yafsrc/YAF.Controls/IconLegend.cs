/* Yet Another Forum.NET
 * Copyright (C) 2006-2007 Jaben Cargman
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

namespace YAF.Controls
{
	/// <summary>
	/// Summary description for ForumUsers.
	/// </summary>
	public class IconLegend : BaseControl
	{
		protected override void Render( System.Web.UI.HtmlTextWriter writer )
		{
			System.Text.StringBuilder html = new System.Text.StringBuilder( 2000 );
			html.Append( @"<table cellspacing=""1"" cellpadding=""1"" class=""iconlegend""><tr>" );
			html.AppendFormat( @"<td><img src=""{0}""/> {1}</td>", PageContext.Theme.GetItem( "ICONS", "TOPIC_NEW" ), PageContext.Localization.GetText( "NEW_POSTS" ) );
			html.AppendFormat( @"<td><img src=""{0}""/> {1}</td>", PageContext.Theme.GetItem( "ICONS", "TOPIC" ), PageContext.Localization.GetText( "NO_NEW_POSTS" ) );
			html.Append( @"</tr><tr>" );
			html.AppendFormat( @"<td><img src=""{0}""/> {1}</td>", PageContext.Theme.GetItem( "ICONS", "TOPIC_NEW_LOCKED" ), PageContext.Localization.GetText( "NEW_POSTS_LOCKED" ) );
			html.AppendFormat( @"<td><img src=""{0}""/> {1}</td>", PageContext.Theme.GetItem( "ICONS", "TOPIC_LOCKED" ), PageContext.Localization.GetText( "NO_NEW_POSTS_LOCKED" ) );
			html.Append( @"</tr><tr>" );
			html.AppendFormat( @"<td><img src=""{0}""/> {1}</td>", PageContext.Theme.GetItem( "ICONS", "TOPIC_ANNOUNCEMENT" ), PageContext.Localization.GetText( "ANNOUNCEMENT" ) );
			html.AppendFormat( @"<td><img src=""{0}""/> {1}</td>", PageContext.Theme.GetItem( "ICONS", "TOPIC_STICKY" ), PageContext.Localization.GetText( "STICKY" ) );
			html.Append( @"</tr><tr>" );
			html.AppendFormat( @"<td><img src=""{0}""/> {1}</td>", PageContext.Theme.GetItem( "ICONS", "TOPIC_MOVED" ), PageContext.Localization.GetText( "MOVED" ) );
			html.AppendFormat( @"<td><img src=""{0}""/> {1}</td>", PageContext.Theme.GetItem( "ICONS", "TOPIC_POLL" ), PageContext.Localization.GetText( "POLL" ) );
			html.Append( @"</tr></table>" );

			writer.Write( html.ToString() );
		}
	}
}
