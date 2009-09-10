using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using YAF.Classes.Core;

namespace YAF.Controls
{
	public partial class LastPosts : BaseUserControl
	{
		public object DataSource
		{
			get
			{
				return repLastPosts.DataSource;
			}
			set
			{
				repLastPosts.DataSource = value;
			}
		}

		protected void Page_Load( object sender, EventArgs e )
		{

		}
	}
}
