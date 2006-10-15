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

namespace yaf.controls
{
	public partial class EditUsersPoints : BaseUserControl
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
			ltrCurrentPoints.Text = yaf.DB.user_getpoints( Request.QueryString.Get( "u" ) ).ToString();
		}

		protected void AddPoints_Click( object sender, EventArgs e )
		{
			if ( Page.IsValid )
			{
				DB.user_addpoints( Request.QueryString.Get( "u" ), txtAddPoints.Text );
				BindData();
			}
		}

		protected void RemovePoints_Click( object sender, EventArgs e )
		{
			if ( Page.IsValid )
			{
				DB.user_removepoints( Request.QueryString.Get( "u" ), txtRemovePoints.Text );
				BindData();
			}
		}

		protected void SetUserPoints_Click( object sender, EventArgs e )
		{
			if ( Page.IsValid )
			{
				DB.user_setpoints( Request.QueryString.Get( "u" ), txtUserPoints.Text );
				BindData();
			}
		}
	}
}