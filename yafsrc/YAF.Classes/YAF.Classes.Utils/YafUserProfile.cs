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

		/// <summary>
		/// Helper function to get a profile from the system
		/// </summary>
		/// <param name="userName"></param>
		/// <returns></returns>
		static public YafUserProfile GetProfile(string userName)
		{
			return Create(userName) as YafUserProfile;
		}		
  }
}
