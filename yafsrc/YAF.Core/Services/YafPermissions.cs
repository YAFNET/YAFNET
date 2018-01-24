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
namespace YAF.Core.Services
{
  #region Using

  using System.Web;
  using System.Web.Hosting;

  using YAF.Classes;
  using YAF.Types.Constants;
  using YAF.Types.Extensions;
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
      var noAccess = true;

      if (!this.Check(permission))
      {
        if (permission == ViewPermissions.RegisteredUsers)
        {
          if (!Config.AllowLoginAndLogoff && YafContext.Current.BoardSettings.CustomLoginRedirectUrl.IsSet())
          {
            var loginRedirectUrl = YafContext.Current.BoardSettings.CustomLoginRedirectUrl;

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
            var appPath = HostingEnvironment.ApplicationVirtualPath;
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