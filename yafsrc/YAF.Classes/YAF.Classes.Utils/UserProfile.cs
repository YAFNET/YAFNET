/* Yet Another Forum.net
 * Copyright (C) 2006-2009 Jaben Cargman
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
using System.Text;
using System.Data;
using System.Web.Profile;
using System.Web.Security;
using System.Collections.Generic;
using YAF.Classes.Data;

namespace YAF.Classes.Utils
{
  public class YafUserProfile : ProfileBase
  {
    public YafUserProfile()
      : base()
    {

    }

  	[SettingsAllowAnonymous( false )]
		[CustomProviderData( "Location;nvarchar;255" )]
    public string Location
    {
      get { return base["Location"] as string; }
      set { base["Location"] = value; }
    }

		[SettingsAllowAnonymous( false )]
		[CustomProviderData( "Homepage;nvarchar;255" )]
		public string Homepage
		{
			get { return base ["Homepage"] as string; }
			set { base ["Homepage"] = value; }
		}

    [SettingsAllowAnonymous( false )]
		[CustomProviderData( "MSN;nvarchar;255" )]
    public string MSN
    {
      get { return base["MSN"] as string; }
      set { base["MSN"] = value; }
    }

    [SettingsAllowAnonymous( false )]
		[CustomProviderData( "YIM;nvarchar;255" )]
    public string YIM
    {
      get { return base["YIM"] as string; }
      set { base["YIM"] = value; }
    }

    [SettingsAllowAnonymous( false )]
		[CustomProviderData( "AIM;nvarchar;255" )]
    public string AIM
    {
      get { return base["AIM"] as string; }
      set { base["AIM"] = value; }
    }

    [SettingsAllowAnonymous( false )]
		[CustomProviderData( "ICQ;nvarchar;255" )]
    public string ICQ
    {
      get { return base["ICQ"] as string; }
      set { base["ICQ"] = value; }
    }

    [SettingsAllowAnonymous( false )]
		[CustomProviderData( "GoogleTalk;nvarchar;255" )]
    public string GoogleTalk
    {
      get { return base["GoogleTalk"] as string; }
      set { base["GoogleTalk"] = value; }
    }

		[SettingsAllowAnonymous( false )]
		[CustomProviderData( "Skype;nvarchar;255" )]
		public string Skype
		{
			get { return base ["Skype"] as string; }
			set { base ["Skype"] = value; }
		}

    [SettingsAllowAnonymous( false )]
		[CustomProviderData( "Blog;nvarchar;255" )]
    public string Blog
    {
      get { return base["Blog"] as string; }
      set { base["Blog"] = value; }
    }

    [SettingsAllowAnonymous( false )]
		[CustomProviderData( "BlogServiceUrl;nvarchar;255" )]
    public string BlogServiceUrl
    {
      get { return base["BlogServiceUrl"] as string; }
      set { base["BlogServiceUrl"] = value; }
    }

    [SettingsAllowAnonymous( false )]
		[CustomProviderData( "BlogServiceUsername;nvarchar;255" )]
    public string BlogServiceUsername
    {
      get { return base["BlogServiceUsername"] as string; }
      set { base["BlogServiceUsername"] = value; }
    }

    [SettingsAllowAnonymous( false )]
		[CustomProviderData( "BlogServicePassword;nvarchar;255" )]
    public string BlogServicePassword
    {
      get { return base["BlogServicePassword"] as string; }
      set { base["BlogServicePassword"] = value; }
    }

    [SettingsAllowAnonymous( false )]
		[CustomProviderData( "RealName;nvarchar;255" )]
    public string RealName
    {
      get { return base["RealName"] as string; }
      set { base["RealName"] = value; }
    }

    [SettingsAllowAnonymous( false )]
		[CustomProviderData( "Occupation;nvarchar;400" )]
    public string Occupation
    {
      get { return base["Occupation"] as string; }
      set { base["Occupation"] = value; }
    }

    [SettingsAllowAnonymous( false )]
		[CustomProviderData( "Interests;nvarchar;400" )]
    public string Interests
    {
      get { return base["Interests"] as string; }
      set { base["Interests"] = value; }
    }

    [SettingsAllowAnonymous( false )]
		[CustomProviderData( "Gender;int" )]
    public int Gender
    {
      get { return (int)base["Gender"]; }
      set { base["Gender"] = value; }
    }

    [SettingsAllowAnonymous( false )]
		[CustomProviderData( "Birthday;DateTime" )]
    public DateTime Birthday
    {
      get { return (DateTime)base["Birthday"]; }
      set { base["Birthday"] = value; }
    }
  }

	/// <summary>
	/// Helps get a complete user profile from various locations
	/// </summary>
	public class YafCombinedUserData
	{
		private MembershipUser _membershipUser = null;
		private YafUserProfile _userProfile = null;
		private DataRow _userDBRow = null;
		private DataRowConvert _rowConvert = null;
		private int? _userID = null;

		public YafCombinedUserData( MembershipUser membershipUser, int userID )
		{
			_userID = userID;
			_membershipUser = membershipUser;
			InitUserData();
		}

		public YafCombinedUserData( MembershipUser membershipUser )
			: this( membershipUser, UserMembershipHelper.GetUserIDFromProviderUserKey( membershipUser.ProviderUserKey ) )
		{
		}

		public YafCombinedUserData( int userID )
			: this( UserMembershipHelper.GetMembershipUser( userID ), userID )
		{
		}

		private void InitUserData()
		{
			if ( _membershipUser != null )
			{
				if ( _userID == null )
				{
					// get the user id
					_userID = UserMembershipHelper.GetUserIDFromProviderUserKey( _membershipUser.ProviderUserKey );
				}
				_userProfile = YafContext.Current.GetProfile( _membershipUser.UserName );
				// get the data for this user from the DB...
				DataRow userRow = UserMembershipHelper.GetUserRowForID( (int)_userID, false );
				if ( userRow != null )
				{
					_userDBRow = userRow;
					_rowConvert = new DataRowConvert( _userDBRow );
				}				
			}
			else if ( _userID != null )
			{
				// see if this is the guest user
				DataRow userRow = UserMembershipHelper.GetUserRowForID( (int)_userID, false );
				if ( userRow != null )
				{
					_userDBRow = userRow;
					_userProfile = YafContext.Current.GetProfile( _userDBRow ["Name"].ToString() );
					_rowConvert = new DataRowConvert( _userDBRow );
				}
			}
			else
			{
				throw new Exception( "Cannot locate user information." );
			}
		}

		public int UserID
		{
			get
			{
				if ( _userID != null ) return (int)_userID;
				
				return 0;
			}
		}

		public bool IsGuest
		{
			get
			{
				if ( _userDBRow != null )
				{
					if ( Convert.ToInt32( _userDBRow ["IsGuest"] ) > 0 ) return true;
				}

				return false;
			}
		}

		public MembershipUser Membership
		{
			get
			{
				return _membershipUser;
			}
		}

		public YafUserProfile Profile
		{
			get
			{
				return _userProfile;
			}
		}

		public DataRow DBRow
		{
			get
			{
				return _userDBRow;
			}
		}

		public string Email
		{
			get
			{
				if ( IsGuest )
				{
					return _rowConvert.AsString( "Email" );
				}

				return Membership.Email;
			}
		}

		public string ThemeFile
		{
			get
			{
				return _rowConvert.AsString( "ThemeFile" );
			}
		}

		public string LanguageFile
		{
			get
			{
				return _rowConvert.AsString( "LanguageFile" );
			}
		}

		public string Signature
		{
			get
			{
				return _rowConvert.AsString( "Signature" );
			}
		}

		public string Avatar
		{
			get
			{
				return _rowConvert.AsString( "Avatar" );
			}
		}

		public string RankName
		{
			get
			{
				return _rowConvert.AsString( "RankName" );
			}
		}

		public int? NumPosts
		{
			get
			{
				return _rowConvert.AsInt32( "NumPosts" );
			}
		}

		public int? TimeZone
		{
			get
			{
				return _rowConvert.AsInt32( "TimeZone" );
			}
		}

		public int? Points
		{
			get
			{
				return _rowConvert.AsInt32( "Points" );
			}
		}

		public bool OverrideDefaultThemes
		{
			get
			{
				return _rowConvert.AsBool( "OverrideDefaultThemes" );
			}
		}

		public bool PMNotification
		{
			get
			{
				return _rowConvert.AsBool( "PMNotification" );
			}
		}

		public DateTime? Joined
		{
			get
			{
				return _rowConvert.AsDateTime( "Joined" );
			}
		}

		public DateTime? LastVisit
		{
			get
			{
				return _rowConvert.AsDateTime( "LastVisit" );
			}
		}

		public bool HasAvatarImage
		{
			get
			{
				bool hasImage = false;

				if (DBRow ["HasAvatarImage"] != null && DBRow["HasAvatarImage"] != DBNull.Value)
				{
					hasImage = _rowConvert.AsInt64( "HasAvatarImage" ) > 0;
				}

				return hasImage;
			}
		}
	}
}
