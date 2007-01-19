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

namespace YAF.Controls
{
	public partial class EditUsersPoints : YAF.Classes.Base.BaseUserControl
	{
		protected void Page_Load( object sender, EventArgs e )
		{
			if ( !IsPostBack )
			{
				BindData();
			}
		}

		private void BindData()
		{
			ltrCurrentPoints.Text = YAF.Classes.Data.DB.user_getpoints( Request.QueryString.Get( "u" ) ).ToString();
		}

		protected void AddPoints_Click( object sender, EventArgs e )
		{
			if ( Page.IsValid )
			{
				YAF.Classes.Data.DB.user_addpoints( Request.QueryString.Get( "u" ), txtAddPoints.Text );
				BindData();
			}
		}

		protected void RemovePoints_Click( object sender, EventArgs e )
		{
			if ( Page.IsValid )
			{
				YAF.Classes.Data.DB.user_removepoints( Request.QueryString.Get( "u" ), txtRemovePoints.Text );
				BindData();
			}
		}

		protected void SetUserPoints_Click( object sender, EventArgs e )
		{
			if ( Page.IsValid )
			{
				YAF.Classes.Data.DB.user_setpoints( Request.QueryString.Get( "u" ), txtUserPoints.Text );
				BindData();
			}
		}
	}
}