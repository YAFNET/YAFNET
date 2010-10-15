namespace YAF.Pages // YAF.Pages
{
    using System;
    using System.Web.Security;
    using YAF.Classes;
    using YAF.Classes.Core;
    using YAF.Classes.Utils;

	public partial class im_msn : YAF.Classes.Core.ForumPage
	{
		public int UserID
		{
			get
			{
				return ( int )Security.StringToLongOrRedirect( Request.QueryString ["u"] );
			}
		}

        public im_msn()
			: base( "IM_MSN" )
		{
		}

		protected void Page_Load( object sender, EventArgs e )
		{
			if ( User == null )
			{
				YafBuildLink.AccessDenied();
			}

			if ( !IsPostBack )
			{
				// get user data...
				MembershipUser user = UserMembershipHelper.GetMembershipUserById( UserID );

				if ( user == null )
				{
					YafBuildLink.AccessDenied(/*No such user exists*/);
				}

                string displayName = UserMembershipHelper.GetDisplayNameFromID(UserID);

				PageLinks.AddLink( PageContext.BoardSettings.Name, YafBuildLink.GetLink( ForumPages.forum ) );
                PageLinks.AddLink(!string.IsNullOrEmpty(displayName) ? displayName : user.UserName, YafBuildLink.GetLink(ForumPages.profile, "u={0}", UserID));
				PageLinks.AddLink( GetText( "TITLE" ), "" );

				// get full user data...
				CombinedUserDataHelper userData = new CombinedUserDataHelper( user, UserID );

                Msg.NavigateUrl = "msnim:chat?contact={0}".FormatWith(userData.Profile.MSN);
				//Msg.Attributes.Add( "onclick", "return skypeCheck();" );
                Img.Src = "http://messenger.services.live.com/users/{0}/presenceimage".FormatWith(userData.Profile.MSN);
			}
		}
	}
}
