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

namespace YAF.Controls
{
	public partial class ForumWelcome : YAF.Classes.Base.BaseUserControl
	{
		public ForumWelcome()
		{
			this.PreRender += new EventHandler( ForumWelcome_PreRender );
		}

		void ForumWelcome_PreRender( object sender, EventArgs e )
		{
			TimeNow.Text = String.Format( PageContext.Localization.GetText( "Current_Time" ), YafDateTime.FormatTime( DateTime.Now ) );
			TimeLastVisit.Text = String.Format( PageContext.Localization.GetText( "last_visit" ), YafDateTime.FormatDateTime( Mession.LastVisit ) );

			if ( PageContext.UnreadPrivate > 0 )
			{
				UnreadMsgs.Visible = true;
				UnreadMsgs.NavigateUrl = YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.cp_pm );
				if ( PageContext.UnreadPrivate == 1 )
					UnreadMsgs.Text = String.Format( PageContext.Localization.GetText( "unread1" ), PageContext.UnreadPrivate );
				else
					UnreadMsgs.Text = String.Format( PageContext.Localization.GetText( "unread0" ), PageContext.UnreadPrivate );
			}
		}
	}
}