/* Yet Another Forum.net
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2007 Jaben Cargman
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
    public string Location
    {
      get { return base["Location"] as string; }
      set { base["Location"] = value; }
    }

		[SettingsAllowAnonymous( false )]
		public string Homepage
		{
			get { return base ["Homepage"] as string; }
			set { base ["Homepage"] = value; }
		}

    [SettingsAllowAnonymous( false )]
    public string MSN
    {
      get { return base["MSN"] as string; }
      set { base["MSN"] = value; }
    }

    [SettingsAllowAnonymous( false )]
    public string YIM
    {
      get { return base["YIM"] as string; }
      set { base["YIM"] = value; }
    }

    [SettingsAllowAnonymous( false )]
    public string AIM
    {
      get { return base["AIM"] as string; }
      set { base["AIM"] = value; }
    }

    [SettingsAllowAnonymous( false )]
    public string ICQ
    {
      get { return base["ICQ"] as string; }
      set { base["ICQ"] = value; }
    }

    [SettingsAllowAnonymous( false )]
    public string GoogleTalk
    {
      get { return base["GoogleTalk"] as string; }
      set { base["GoogleTalk"] = value; }
    }

		[SettingsAllowAnonymous( false )]
		public string Skype
		{
			get { return base ["Skype"] as string; }
			set { base ["Skype"] = value; }
		}

    [SettingsAllowAnonymous( false )]
    public string Blog
    {
      get { return base["Blog"] as string; }
      set { base["Blog"] = value; }
    }

    [SettingsAllowAnonymous( false )]
    public string BlogServiceUrl
    {
      get { return base["BlogServiceUrl"] as string; }
      set { base["BlogServiceUrl"] = value; }
    }

    [SettingsAllowAnonymous( false )]
    public string BlogServiceUsername
    {
      get { return base["BlogServiceUsername"] as string; }
      set { base["BlogServiceUsername"] = value; }
    }

    [SettingsAllowAnonymous( false )]
    public string BlogServicePassword
    {
      get { return base["BlogServicePassword"] as string; }
      set { base["BlogServicePassword"] = value; }
    }

    [SettingsAllowAnonymous( false )]
    public string RealName
    {
      get { return base["RealName"] as string; }
      set { base["RealName"] = value; }
    }

    [SettingsAllowAnonymous( false )]
    public string Occupation
    {
      get { return base["Occupation"] as string; }
      set { base["Occupation"] = value; }
    }

    [SettingsAllowAnonymous( false )]
    public string Interests
    {
      get { return base["Interests"] as string; }
      set { base["Interests"] = value; }
    }

    [SettingsAllowAnonymous( false )]
    public int Gender
    {
      get { return (int)base["Gender"]; }
      set { base["Gender"] = value; }
    }

    [SettingsAllowAnonymous( false )]
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

		public YafCombinedUserData( MembershipUser membershipUser )
		{
			_membershipUser = membershipUser;
			InitUserData();
		}

		public YafCombinedUserData( MembershipUser membershipUser, int userID )
			: this( membershipUser )
		{
			_userID = userID;
		}

		private void InitUserData()
		{
			_userProfile = YafContext.Current.GetProfile( _membershipUser.UserName );
			if ( _userID == null )
			{
				// get the user id
				_userID = UserMembershipHelper.GetUserIDFromProviderUserKey( _membershipUser.ProviderUserKey );
			}
			// get the data for this user from the DB...
			DataTable dt = YAF.Classes.Data.DB.user_list( YafContext.Current.PageBoardID, _userID, true );
			if ( dt.Rows.Count > 0 )
				_userDBRow = dt.Rows [0];

			_rowConvert = new DataRowConvert( _userDBRow );
		}

		public int UserID
		{
			get
			{
				if ( _userID != null ) return (int)_userID;
				
				return 0;
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
	}
}
