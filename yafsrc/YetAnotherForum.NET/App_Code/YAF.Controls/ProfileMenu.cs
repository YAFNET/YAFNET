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
using YAF.Classes.Utils;

namespace YAF.Controls
{
  /// <summary>
  /// Summary description for ForumUsers.
  /// </summary>
  public class ProfileMenu : BaseControl
  {
    protected override void Render( System.Web.UI.HtmlTextWriter writer )
    {
      System.Text.StringBuilder html = new System.Text.StringBuilder( 2000 );

      html.Append( @"<table width=""100%"" cellspacing=""0"" cellpadding=""0"">" );

      if ( PageContext.BoardSettings.AllowPrivateMessages )
      {
        html.AppendFormat( @"<tr class=""header2""><td>{0}</td></tr>", PageContext.Localization.GetText( "MESSENGER" ) );
        html.AppendFormat( @"<tr><td>" );
        html.AppendFormat( @"<li><a href=""{0}"">{1}</a></li>", YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.cp_pm, "v=in" ), PageContext.Localization.GetText( "INBOX" ) );
        html.AppendFormat( @"<li><a href=""{0}"">{1}</a></li>", YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.cp_pm, "v=out" ), PageContext.Localization.GetText( "SENTITEMS" ) );
        html.AppendFormat( @"<li><a href=""{0}"">{1}</a></li>", YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.cp_pm, "v=arch" ), PageContext.Localization.GetText( "ARCHIVE" ) );
        html.AppendFormat( @"<li><a href=""{0}"">{1}</a></li>", YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.pmessage ), PageContext.Localization.GetText( "NEW_MESSAGE" ) );
        html.AppendFormat( @"</td></tr>" );
        html.AppendFormat( @"<tr><td>&nbsp;</td></tr>" );
      }

      html.AppendFormat( @"<tr class=""header2""><td>{0}</td></tr>", PageContext.Localization.GetText( "PERSONAL_PROFILE" ) );
      html.AppendFormat( @"<tr><td>" );
      html.AppendFormat( @"<li><a href=""{0}"">{1}</a></li>", YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.cp_editprofile ), PageContext.Localization.GetText( "EDIT_PROFILE" ) );
      html.AppendFormat( @"<li><a href=""{0}"">{1}</a></li>", YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.cp_editavatar ), PageContext.Localization.GetText( "EDIT_AVATAR" ) );
      if ( PageContext.BoardSettings.AllowSignatures ) html.AppendFormat( @"<li><a href=""{0}"">{1}</a></li>", YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.cp_signature ), PageContext.Localization.GetText( "SIGNATURE" ) );
      html.AppendFormat( @"<li><a href=""{0}"">{1}</a></li>", YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.cp_subscriptions ), PageContext.Localization.GetText( "SUBSCRIPTIONS" ) );
      html.AppendFormat( @"<li><a href=""{0}"">{1}</a></li>", YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.cp_changepassword ), PageContext.Localization.GetText( "CHANGE_PASSWORD" ) );
      html.AppendFormat( @"</td></tr>" );
      html.Append( @"</table>" );

      writer.Write( html.ToString() );
    }
  }
}
