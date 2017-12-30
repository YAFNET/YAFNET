/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Utils
{
  #region Using

  using System;
  using System.Web.Profile;

  using YAF.Types.Extensions;
  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// The yaf user profile.
  /// </summary>
  public class YafUserProfile : ProfileBase, IYafUserProfile
  {
    #region Properties

    /// <summary>
    /// Gets or sets AIM.
    /// </summary>
    [SettingsAllowAnonymous(false)]
    [CustomProviderData("AIM;nvarchar;255")]
    public string AIM
    {
      get
      {
        return base["AIM"] as string;
      }

      set
      {
        base["AIM"] = value;
      }
    }

    /// <summary>
    /// Gets or sets Birthday.
    /// </summary>
    [SettingsAllowAnonymous(false)]
    [CustomProviderData("Birthday;DateTime")]
    public DateTime Birthday
    {
      get
      {
        return base["Birthday"].ToType<DateTime>();
      }

      set
      {
        base["Birthday"] = value;
      }
    }

    /// <summary>
    /// Gets or sets Blog.
    /// </summary>
    [SettingsAllowAnonymous(false)]
    [CustomProviderData("Blog;nvarchar;255")]
    public string Blog
    {
      get
      {
        return base["Blog"] as string;
      }

      set
      {
        base["Blog"] = value;
      }
    }

    /// <summary>
    /// Gets or sets BlogServicePassword.
    /// </summary>
    [SettingsAllowAnonymous(false)]
    [CustomProviderData("BlogServicePassword;nvarchar;255")]
    public string BlogServicePassword
    {
      get
      {
        return base["BlogServicePassword"] as string;
      }

      set
      {
        base["BlogServicePassword"] = value;
      }
    }

    /// <summary>
    /// Gets or sets BlogServiceUrl.
    /// </summary>
    [SettingsAllowAnonymous(false)]
    [CustomProviderData("BlogServiceUrl;nvarchar;255")]
    public string BlogServiceUrl
    {
      get
      {
        return base["BlogServiceUrl"] as string;
      }

      set
      {
        base["BlogServiceUrl"] = value;
      }
    }

    /// <summary>
    /// Gets or sets BlogServiceUsername.
    /// </summary>
    [SettingsAllowAnonymous(false)]
    [CustomProviderData("BlogServiceUsername;nvarchar;255")]
    public string BlogServiceUsername
    {
      get
      {
        return base["BlogServiceUsername"] as string;
      }

      set
      {
        base["BlogServiceUsername"] = value;
      }
    }

    /// <summary>
    /// Gets or sets Gender.
    /// </summary>
    [SettingsAllowAnonymous(false)]
    [CustomProviderData("Gender;int")]
    public int Gender
    {
      get
      {
        return (int)base["Gender"];
      }

      set
      {
        base["Gender"] = value;
      }
    }

    /// <summary>
    /// Gets or sets Google+ URL
    /// </summary>
    [SettingsAllowAnonymous(false)]
    [CustomProviderData("Google;nvarchar;400")]
    public string Google
    {
      get
      {
        return base["Google"] as string;
      }

      set
      {
        base["Google"] = value;
      }
    }

    /// <summary>
    /// Gets or sets Google Id
    /// </summary>
    [SettingsAllowAnonymous(false)]
    [CustomProviderData("GoogleId;nvarchar;255")]
    public string GoogleId
    {
        get
        {
            return base["GoogleId"] as string;
        }

        set
        {
            base["GoogleId"] = value;
        }
    }

    /// <summary>
    /// Gets or sets Homepage.
    /// </summary>
    [SettingsAllowAnonymous(false)]
    [CustomProviderData("Homepage;nvarchar;255")]
    public string Homepage
    {
      get
      {
        return base["Homepage"] as string;
      }

      set
      {
        base["Homepage"] = value;
      }
    }

    /// <summary>
    /// Gets or sets ICQ.
    /// </summary>
    [SettingsAllowAnonymous(false)]
    [CustomProviderData("ICQ;nvarchar;255")]
    public string ICQ
    {
      get
      {
        return base["ICQ"] as string;
      }

      set
      {
        base["ICQ"] = value;
      }
    }

    /// <summary>
    /// Gets or sets Facebook.
    /// </summary>
    [SettingsAllowAnonymous(false)]
    [CustomProviderData("Facebook;nvarchar;400")]
    public string Facebook
    {
        get
        {
            return base["Facebook"] as string;
        }

        set
        {
            base["Facebook"] = value;
        }
    }

    /// <summary>
    /// Gets or sets Facebook.
    /// </summary>
    [SettingsAllowAnonymous(false)]
    [CustomProviderData("FacebookId;nvarchar;400")]
    public string FacebookId
    {
        get
        {
            return base["FacebookId"] as string;
        }

        set
        {
            base["FacebookId"] = value;
        }
    }

    /// <summary>
    /// Gets or sets Twitter.
    /// </summary>
    [SettingsAllowAnonymous(false)]
    [CustomProviderData("Twitter;nvarchar;400")]
    public string Twitter
    {
        get
        {
            return base["Twitter"] as string;
        }

        set
        {
            base["Twitter"] = value;
        }
    }

    /// <summary>
    /// Gets or sets Twitter.
    /// </summary>
    [SettingsAllowAnonymous(false)]
    [CustomProviderData("TwitterId;nvarchar;400")]
    public string TwitterId
    {
        get
        {
            return base["TwitterId"] as string;
        }

        set
        {
            base["TwitterId"] = value;
        }
    }

    /// <summary>
    /// Gets or sets Interests.
    /// </summary>
    [SettingsAllowAnonymous(false)]
    [CustomProviderData("Interests;nvarchar;400")]
    public string Interests
    {
      get
      {
        return base["Interests"] as string;
      }

      set
      {
        base["Interests"] = value;
      }
    }

    /// <summary>
    /// Gets or sets Location.
    /// </summary>
    [SettingsAllowAnonymous(false)]
    [CustomProviderData("Location;nvarchar;255")]
    public string Location
    {
      get
      {
        return base["Location"] as string;
      }

      set
      {
        base["Location"] = value;
      }
    }

    /// <summary>
    /// Gets or sets Country.
    /// </summary>
    [SettingsAllowAnonymous(false)]
    [CustomProviderData("Country;nvarchar;2")]
    public string Country
    {
        get
        {
            return base["Country"] as string;
        }

        set
        {
            base["Country"] = value;
        }
    }

    /// <summary>
    /// Gets or sets Region or State(US).
    /// </summary>
    [SettingsAllowAnonymous(false)]
    [CustomProviderData("Region;nvarchar;255")]
    public string Region
    {
        get
        {
            return base["Region"] as string;
        }

        set
        {
            base["Region"] = value;
        }
    }

    /// <summary>
    /// Gets or sets a City.
    /// </summary>
    [SettingsAllowAnonymous(false)]
    [CustomProviderData("City;nvarchar;255")]
    public string City
    {
        get
        {
            return base["City"] as string;
        }

        set
        {
            base["City"] = value;
        }
    }

    /// <summary>
    /// Gets or sets MSN.
    /// </summary>
    [SettingsAllowAnonymous(false)]
    [CustomProviderData("MSN;nvarchar;255")]
    public string MSN
    {
      get
      {
        return base["MSN"] as string;
      }

      set
      {
        base["MSN"] = value;
      }
    }

    /// <summary>
    /// Gets or sets Occupation.
    /// </summary>
    [SettingsAllowAnonymous(false)]
    [CustomProviderData("Occupation;nvarchar;400")]
    public string Occupation
    {
      get
      {
        return base["Occupation"] as string;
      }

      set
      {
        base["Occupation"] = value;
      }
    }

    /// <summary>
    /// Gets or sets RealName.
    /// </summary>
    [SettingsAllowAnonymous(false)]
    [CustomProviderData("RealName;nvarchar;255")]
    public string RealName
    {
      get
      {
        return base["RealName"] as string;
      }

      set
      {
        base["RealName"] = value;
      }
    }

    /// <summary>
    /// Gets or sets Skype.
    /// </summary>
    [SettingsAllowAnonymous(false)]
    [CustomProviderData("Skype;nvarchar;255")]
    public string Skype
    {
      get
      {
        return base["Skype"] as string;
      }

      set
      {
        base["Skype"] = value;
      }
    }

    /// <summary>
    /// Gets or sets XMPP.
    /// </summary>
    [SettingsAllowAnonymous(false)]
    [CustomProviderData("XMPP;nvarchar;255")]
    public string XMPP
    {
      get
      {
        return base["XMPP"] as string;
      }

      set
      {
        base["XMPP"] = value;
      }
    }

    /// <summary>
    /// Gets or sets YIM.
    /// </summary>
    [SettingsAllowAnonymous(false)]
    [CustomProviderData("YIM;nvarchar;255")]
    public string YIM
    {
      get
      {
        return base["YIM"] as string;
      }

      set
      {
        base["YIM"] = value;
      }
    }

    /// <summary>
    /// Gets or sets Last Synced With DNN.
    /// </summary>
    [SettingsAllowAnonymous(false)]
    [CustomProviderData("LastSyncedWithDNN;DateTime")]
    public DateTime LastSyncedWithDNN
    {
        get
        {
            return base["LastSyncedWithDNN"].ToType<DateTime>();
        }

        set
        {
            base["LastSyncedWithDNN"] = value;
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Helper function to get a profile from the system
    /// </summary>
    /// <param name="userName">
    /// Name of the user.
    /// </param>
    /// <returns>
    /// The get profile.
    /// </returns>
    public static YafUserProfile GetProfile(string userName)
    {
      return Create(userName) as YafUserProfile;
    }

    #endregion
  }
}