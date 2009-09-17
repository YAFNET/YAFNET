using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using YAF.Classes.Core;
using YAF.Classes.Data;

namespace YAF.Controls
{
	public partial class LastPosts : BaseUserControl
	{
		public long? TopicID
		{
			get
			{
				if ( ViewState["TopicID"] != null )
					return Convert.ToInt32( ViewState["TopicID"] );

				return null;
			}
			set
			{
				ViewState["TopicID"] = value;
			}
		}

		protected void Page_Load( object sender, EventArgs e )
		{
			YafContext.Current.PageElements.RegisterJsBlockStartup( LastPostUpdatePanel, "DisablePageManagerScrollJs", YAF.Utilities.JavaScriptBlocks.DisablePageManagerScrollJs );

			BindData();
		}

		private void BindData()
		{
			if ( TopicID.HasValue )
				repLastPosts.DataSource = DB.post_list_reverse10( TopicID );
			else
				repLastPosts.DataSource = null;

			DataBind();
		}

		protected void LastPostUpdateTimer_Tick( object sender, EventArgs e )
		{
			BindData();
		}
	}
}
