/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Modules;

using System;

using YAF.Core.Context;

/// <summary>
/// The Anti XSRF Forum Module.
/// "https://software-security.sans.org/developer-how-to/developer-guide-csrf"
/// </summary>
[Module("Anti CSRF Forum Module", "Tiny Gecko", 1)]
public class AntiXsrfModule : SimpleBaseForumModule
{
    /// <summary>
    /// The anti XSRF token key.
    /// </summary>
    private const string AntiXsrfTokenKey = "__AntiXsrfToken";

    /// <summary>
    /// The anti XSRF token value.
    /// </summary>
    private string antiXsrfTokenValue;

    /// <summary>
    /// The init.
    /// </summary>
    public override void Init()
    {
        // hook the page init for mail sending...
        this.ForumControl.InitForumPage += this.CurrentPageInit;
    }

    /// <summary>
    /// Currents the after initialize.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private void CurrentPageInit(object sender, EventArgs e)
    {
        var requestCookie = HttpContext.Current.Request.Cookies[AntiXsrfTokenKey];

        if (requestCookie != null && Guid.TryParse(requestCookie.Value, out _))
        {
            this.antiXsrfTokenValue = requestCookie.Value;

            this.ForumControl.Page.ViewStateUserKey = this.antiXsrfTokenValue;
        }
        else
        {
            // If the CSRF cookie is not found, then this is a new session.
            // Generate a new Anti-XSRF token
            this.antiXsrfTokenValue = Guid.NewGuid().ToString("N");

            this.ForumControl.Page.ViewStateUserKey = this.antiXsrfTokenValue;

            // Create the non-persistent CSRF cookie
            var responseCookie = new HttpCookie(AntiXsrfTokenKey) {
                                                                      HttpOnly = true,
                                                                      Value = this.antiXsrfTokenValue,
                                                                      Secure = BoardContext.Current
                                                                          .Get<HttpRequestBase>().IsSecureConnection
                                                                  };

            HttpContext.Current.Response.Cookies.Set(responseCookie);
        }

        this.ForumControl.Page.PreLoad += this.Page_OnPreLoad;
    }

    /// <summary>
    /// The page_ on pre load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// </exception>
    private void Page_OnPreLoad(object sender, EventArgs e)
    {
        var cacheKey = $"{this.Get<HttpSessionStateBase>().SessionID}{AntiXsrfTokenKey}";

        if (!this.ForumControl.Page.IsPostBack)
        {
            // Set Anti-XSRF token
            this.Get<IDataCache>().Set(cacheKey, this.ForumControl.Page.ViewStateUserKey);
        }
        else
        {
            // Validate the Anti-XSRF token
            if ((string)this.Get<IDataCache>().Get(cacheKey) != this.antiXsrfTokenValue)
            {
               throw new InvalidOperationException("Validation of Anti -XSRF token failed.");
            }
        }
    }
}