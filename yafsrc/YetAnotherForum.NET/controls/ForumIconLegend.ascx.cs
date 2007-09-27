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
	public partial class ForumIconLegend : YAF.Classes.Base.BaseUserControl
	{
		public ForumIconLegend()
		{
			this.PreRender += new EventHandler( ForumIconLegend_PreRender );
		}

		void ForumIconLegend_PreRender( object sender, EventArgs e )
		{
			Forum_New.ImageUrl = PageContext.Theme.GetItem( "ICONS", "FORUM_NEW" );
			Forum.ImageUrl = PageContext.Theme.GetItem( "ICONS", "FORUM" );
			Forum_Locked.ImageUrl = PageContext.Theme.GetItem( "ICONS", "FORUM_LOCKED" );
		}
	}
}