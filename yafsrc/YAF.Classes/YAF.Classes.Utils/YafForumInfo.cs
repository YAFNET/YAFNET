/* Yet Another Forum.NET
 * Copyright (C) 2006-2010 Jaben Cargman
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
using System.Web;

namespace YAF.Classes.Utils
{
  /// <summary>
  /// Class provides helper functions related to the forum path and urls as well as forum version information.
  /// </summary>
  public static class YafForumInfo
  {
    /// <summary>
    /// Gets the forum path (client-side).
    /// May not be the actual URL of the forum.
    /// </summary>
    public static string ForumClientFileRoot
    {
      get
      {
        return BaseUrlBuilder.ClientFileRoot;
      }
    }

    /// <summary>
    /// Gets the forum path (server-side).
    /// May not be the actual URL of the forum.
    /// </summary>
    public static string ForumServerFileRoot
    {
      get
      {
        return BaseUrlBuilder.ServerFileRoot;
      }
    }

    /// <summary>
    /// Gets complete application external (client-side) URL of the forum. (e.g. http://myforum.com/forum
    /// </summary>
    public static string ForumBaseUrl
    {
      get
      {
        return BaseUrlBuilder.BaseUrl + BaseUrlBuilder.AppPath;
      }
    }

    /// <summary>
    /// Gets full URL to the Root of the Forum
    /// </summary>
    public static string ForumURL
    {
      get
      {
        return YafBuildLink.GetLink(ForumPages.forum, true);
      }
    }

    /// <summary>
    /// Gets <see langword="true"/> if the current site is localhost.
    /// </summary>
    public static bool IsLocal
    {
      get
      {
        string s = HttpContext.Current.Request.ServerVariables["SERVER_NAME"];
        return s != null && s.ToLower() == "localhost";
      }
    }

    /// <summary>
    /// Helper function that creates the the url of a resource.
    /// </summary>
    /// <param name="resourceName">
    /// </param>
    /// <returns>
    /// The get url to resource.
    /// </returns>
    public static string GetURLToResource(string resourceName)
    {
      return "{1}resources/{0}".FormatWith(resourceName, ForumClientFileRoot);
    }

    #region Version Information

    /// <summary>
    /// Get the Current YAF Application Version string
    /// </summary>
    public static string AppVersionName
    {
      get
      {
        return AppVersionNameFromCode(AppVersionCode);
      }
    }

    /// <summary>
    /// Get the Current YAF Database Version
    /// </summary>
    public static int AppVersion
    {
      get
      {
        return 41;
      }
    }

    /// <summary>
    /// Gets the Current YAF Application Version
    /// </summary>
    public static long AppVersionCode
    {
      get
      {
        return 0x01090412;
      }
    }

    /// <summary>
    /// Gets the Current YAF Build Date
    /// </summary>
    public static DateTime AppVersionDate
    {
      get
      {
        return new DateTime(2010, 7, 19);
      }
    }

    /// <summary>
    /// Creates a string that is the YAF Application Version from a long value
    /// </summary>
    /// <param name="code">
    /// Value of Current Version
    /// </param>
    /// <returns>
    /// Application Version String
    /// </returns>
    public static string AppVersionNameFromCode(long code)
    {
      string version;

      if ((code & 0xF0) > 0 || (code & 0x0F) == 1)
      {
        version = "{0}.{1}.{2}{3}".FormatWith((code >> 24) & 0xFF, (code >> 16) & 0xFF, (code >> 8) & 0xFF, Convert.ToInt32((code >> 4) & 0x0F).ToString("00"));
      }
      else
      {
        version = "{0}.{1}.{2}".FormatWith((code >> 24) & 0xFF, (code >> 16) & 0xFF, (code >> 8) & 0xFF);
      }

      if ((code & 0x0F) > 0)
      {
        if ((code & 0x0F) == 1)
        {
          // alpha release...
          version += " alpha";
        }
        else if ((code & 0x0F) == 2)
        {
          version += " beta";
        }
        else
        {
          // Add Release Candidate
          version += " RC{0}".FormatWith((code & 0x0F) - 2);
        }
      }

      return version;
    }

    #endregion
  }
}