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
using System.Web.Profile;
using System.Collections.Generic;

namespace YAF.Classes.Utils
{
  public class YAF_UserProfile : ProfileBase
  {
    public YAF_UserProfile()
      : base()
    {

    }

    [SettingsAllowAnonymous(false)]
    public string IP
    {
      get { return base["IP"] as string; }
      set { base["IP"] = value; }
    }

    [SettingsAllowAnonymous(false)]
    public string Name
    {
      get { return base["Name"] as string; }
      set { base["Name"] = value; }
    }

    [SettingsAllowAnonymous(false)]
    public DateTime Joined
    {
      get { return (DateTime)base["Joined"]; }
      set { base["Joined"] = value; }
    }

    [SettingsAllowAnonymous(false)]
    public int NumberOfPosts
    {
      get { return (int)base["NumberOfPosts"]; }
      set { base["NumberOfPosts"] = value; }
    }

    [SettingsAllowAnonymous(false)]
    public string Location
    {
      get { return base["Location"] as string; }
      set { base["Location"] = value; }
    }

    [SettingsAllowAnonymous(false)]
    public string Homepage
    {
      get { return base["Homepage"] as string; }
      set { base["Homepage"] = value; }
    }

    [SettingsAllowAnonymous(false)]
    public int TimeZone
    {
      get { return (int)base["TimeZone"]; }
      set { base["TimeZone"] = value; }
    }

    [SettingsAllowAnonymous(false)]
    public string Avatar
    {
      get { return base["Avatar"] as string; }
      set { base["Avatar"] = value; }
    }

    [SettingsAllowAnonymous(false)]
    public string Signature
    {
      get { return base["Signature"] as string; }
      set { base["Signature"] = value; }
    }

    [SettingsAllowAnonymous(false)]
    public DateTime Suspended
    {
      get { return (DateTime)base["Suspended"]; }
      set { base["Suspended"] = value; }
    }

    [SettingsAllowAnonymous(false)]
    public string LanguageFile
    {
      get { return base["LanguageFile"] as string; }
      set { base["LanguageFile"] = value; }
    }

    [SettingsAllowAnonymous(false)]
    public string ThemeFile
    {
      get { return base["ThemeFile"] as string; }
      set { base["ThemeFile"] = value; }
    }

    [SettingsAllowAnonymous(false)]
    public bool OverrideDefaultThemes
    {
      get { return (bool)base["OverrideDefaultThemes"]; }
      set { base["OverrideDefaultThemes"] = value; }
    }

    [SettingsAllowAnonymous(false)]
    public string MSN
    {
      get { return base["MSN"] as string; }
      set { base["MSN"] = value; }
    }

    [SettingsAllowAnonymous(false)]
    public string YIM
    {
      get { return base["YIM"] as string; }
      set { base["YIM"] = value; }
    }

    [SettingsAllowAnonymous(false)]
    public string AIM
    {
      get { return base["AIM"] as string; }
      set { base["AIM"] = value; }
    }

    [SettingsAllowAnonymous(false)]
    public string ICQ
    {
      get { return base["ICQ"] as string; }
      set { base["ICQ"] = value; }
    }

    [SettingsAllowAnonymous(false)]
    public string GoogleTalk
    {
      get { return base["GoogleTalk"] as string; }
      set { base["GoogleTalk"] = value; }
    }

    [SettingsAllowAnonymous(false)]
    public string Blog
    {
      get { return base["Blog"] as string; }
      set { base["Blog"] = value; }
    }

    [SettingsAllowAnonymous(false)]
    public string BlogServiceUrl
    {
      get { return base["BlogServiceUrl"] as string; }
      set { base["BlogServiceUrl"] = value; }
    }

    [SettingsAllowAnonymous(false)]
    public string BlogServiceUsername
    {
      get { return base["BlogServiceUsername"] as string; }
      set { base["BlogServiceUsername"] = value; }
    }

    [SettingsAllowAnonymous(false)]
    public string BlogServicePassword
    {
      get { return base["BlogServicePassword"] as string; }
      set { base["BlogServicePassword"] = value; }
    }

    [SettingsAllowAnonymous(false)]
    public string RealName
    {
      get { return base["RealName"] as string; }
      set { base["RealName"] = value; }
    }

    [SettingsAllowAnonymous(false)]
    public string Occupation
    {
      get { return base["Occupation"] as string; }
      set { base["Occupation"] = value; }
    }

    [SettingsAllowAnonymous(false)]
    public string Interests
    {
      get { return base["Interests"] as string; }
      set { base["Interests"] = value; }
    }

    [SettingsAllowAnonymous(false)]
    public bool PMNotification
    {
      get { return (bool)base["PMNotification"]; }
      set { base["PMNotification"] = value; }
    }

    [SettingsAllowAnonymous(false)]
    public int Points
    {
      get { return (int)base["Points"]; }
      set { base["Points"] = value; }
    }

    [SettingsAllowAnonymous(false)]
    public int Gender
    {
      get { return (int)base["Gender"]; }
      set { base["Gender"] = value; }
    }

    [SettingsAllowAnonymous(false)]
    public DateTime Birthday
    {
      get { return (DateTime)base["Birthday"]; }
      set { base["Birthday"] = value; }
    }
  }
}
