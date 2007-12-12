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
using YAF.Classes.Data;

namespace YAF.Controls
{
	public partial class ProfileYourAccount : YAF.Classes.Base.BaseUserControl
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
			Groups.DataSource = YAF.Classes.Data.DB.usergroup_list( PageContext.PageUserID );

			// Bind			
			DataBind();

			YafCombinedUserData userData = new YafCombinedUserData( PageContext.PageUserID );

			//TitleUserName.Text = HtmlEncode( userData.Membership.UserName );
			AccountEmail.Text = userData.Membership.Email;
			Name.Text = HtmlEncode( userData.Membership.UserName );
			Joined.Text = YafDateTime.FormatDateTime( userData.Joined );
			NumPosts.Text = String.Format( "{0:N0}", userData.NumPosts );

			if ( PageContext.BoardSettings.AvatarUpload && userData.HasAvatarImage )
			{
				AvatarImage.ImageUrl = String.Format( "{0}resource.ashx?u={1}", YafForumInfo.ForumRoot, PageContext.PageUserID );
			}
			else if ( !String.IsNullOrEmpty( userData.Avatar ) ) // Took out PageContext.BoardSettings.AvatarRemote
			{
				AvatarImage.ImageUrl = String.Format( "{3}resource.ashx?url={0}&width={1}&height={2}",
					Server.UrlEncode( userData.Avatar ),
					PageContext.BoardSettings.AvatarWidth,
					PageContext.BoardSettings.AvatarHeight,
					YafForumInfo.ForumRoot );
			}
			else
			{
				AvatarImage.Visible = false;
			}
		}
	}
}
