using System;
using System.Data;

namespace YAF.Controls
{
	/// <summary>
	/// Summary description for ForumUsers.
	/// </summary>
	public class IconLegend : YAF.Classes.Base.BaseControl
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
