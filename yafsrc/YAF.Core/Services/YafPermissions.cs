/* Yet Another Forum.net
 * Copyright (C) 2006-2012 Jaben Cargman
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
namespace YAF.Core.Services
{
  #region Using

  using System.Web;
  using System.Web.Hosting;

  using YAF.Classes;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// The yaf permissions.
  /// </summary>
  public class YafPermissions : IPermissions
  {
    #region Implemented Interfaces

    #region IPermissions

    /// <summary>
    /// The check.
    /// </summary>
    /// <param name="permission">
    /// The permission.
    /// </param>
    /// <returns>
    /// The check.
    /// </returns>
    public bool Check(ViewPermissions permission)
    {
      if (permission == ViewPermissions.Everyone)
      {
        return true;
      }

      if (permission == ViewPermissions.RegisteredUsers)
      {
        return !YafContext.Current.IsGuest;
      }

      return YafContext.Current.IsAdmin;
    }

    /// <summary>
    /// The handle request.
    /// </summary>
    /// <param name="permission">
    /// The permission.
    /// </param>
    public void HandleRequest(ViewPermissions permission)
    {
      bool noAccess = true;

      if (!this.Check(permission))
      {
        if (permission == ViewPermissions.RegisteredUsers)
        {
          if (!Config.AllowLoginAndLogoff && YafContext.Current.BoardSettings.CustomLoginRedirectUrl.IsSet())
          {
            string loginRedirectUrl = YafContext.Current.BoardSettings.CustomLoginRedirectUrl;

            if (loginRedirectUrl.Contains("{0}"))
            {
              // process for return url..
              loginRedirectUrl =
                loginRedirectUrl.FormatWith(
                  HttpUtility.UrlEncode(General.GetSafeRawUrl(YafContext.Current.Get<HttpRequestBase>().Url.ToString())));
            }

            // allow custom redirect...
            YafContext.Current.Get<HttpResponseBase>().Redirect(loginRedirectUrl);
            noAccess = false;
          }
          else if (!Config.AllowLoginAndLogoff && Config.IsDotNetNuke)
          {
            // automatic DNN redirect...
            string appPath = HostingEnvironment.ApplicationVirtualPath;
            if (!appPath.EndsWith("/"))
            {
              appPath += "/";
            }

            // redirect to DNN login...
            YafContext.Current.Get<HttpResponseBase>().Redirect(
              appPath + "Login.aspx?ReturnUrl=" + HttpUtility.UrlEncode(General.GetSafeRawUrl()));
            noAccess = false;
          }
          else if (Config.AllowLoginAndLogoff)
          {
            YafBuildLink.Redirect(ForumPages.login, "ReturnUrl={0}", HttpUtility.UrlEncode(General.GetSafeRawUrl()));
            noAccess = false;
          }
        }

        // fall-through with no access...
        if (noAccess)
        {
          YafBuildLink.AccessDenied();
        }
      }
    }

    #endregion

    #endregion
  }
}