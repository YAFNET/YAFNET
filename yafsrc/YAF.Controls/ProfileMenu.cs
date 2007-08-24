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

      html.Append( "<table width='100%' cellspacing=0 cellpadding=0>" );

      if ( PageContext.BoardSettings.AllowPrivateMessages )
      {
        html.AppendFormat( "<tr class='header2'><td>{0}</td></tr>", PageContext.Localization.GetText( "MESSENGER" ) );
        html.AppendFormat( "<tr><td>" );
        html.AppendFormat( "<li><a href='{0}'>{1}</a></li>", YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.pm, "v=in" ), PageContext.Localization.GetText( "INBOX" ) );
        html.AppendFormat( "<li><a href='{0}'>{1}</a></li>", YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.pm, "v=out" ), PageContext.Localization.GetText( "SENTITEMS" ) );
        html.AppendFormat( "<li><a href='{0}'>{1}</a></li>", YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.pmessage ), PageContext.Localization.GetText( "NEW_MESSAGE" ) );
        html.AppendFormat( "</td></tr>" );
        html.AppendFormat( "<tr><td>&nbsp;</td></tr>" );
      }

      html.AppendFormat( "<tr class='header2'><td>{0}</td></tr>", PageContext.Localization.GetText( "PERSONAL_PROFILE" ) );
      html.AppendFormat( "<tr><td>" );
      html.AppendFormat( "<li><a href='{0}'>{1}</a></li>", YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.cp_editprofile ), PageContext.Localization.GetText( "EDIT_PROFILE" ) );
      html.AppendFormat( "<li><a href='{0}'>{1}</a></li>", YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.cp_editavatar ), PageContext.Localization.GetText( "EDIT_AVATAR" ) );
      if ( PageContext.BoardSettings.AllowSignatures ) html.AppendFormat( "<li><a href='{0}'>{1}</a></li>", YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.cp_signature ), PageContext.Localization.GetText( "SIGNATURE" ) );
      html.AppendFormat( "<li><a href='{0}'>{1}</a></li>", YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.cp_subscriptions ), PageContext.Localization.GetText( "SUBSCRIPTIONS" ) );
      html.AppendFormat( "<li><a href='{0}'>{1}</a></li>", YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.cp_changepassword ), "Change Password"/*PageContext.Localization.GetText("CHANGE_PASSWORD")*/);
      html.AppendFormat( "</td></tr>" );
      html.Append( "</table>" );

      writer.Write( html.ToString() );
    }
  }
}
