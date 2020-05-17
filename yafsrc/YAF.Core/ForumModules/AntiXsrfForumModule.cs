/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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
namespace YAF.Core.ForumModules
{
    #region Using

    using System;
    using System.Web;

    using YAF.Core.BaseModules;
    using YAF.Types;
    using YAF.Types.Attributes;

    #endregion

    /// <summary>
    /// The Anti XSRF Forum Module.
    /// "https://software-security.sans.org/developer-how-to/developer-guide-csrf"
    /// </summary>
    [Module("Anti CSRF Forum Module", "Tiny Gecko", 1)]
    public class AntiXsrfForumModule : BaseForumModule
    {
        /// <summary>
        /// The anti XSRF token key.
        /// </summary>
        private const string AntiXsrfTokenKey = "__AntiXsrfToken";

        /// <summary>
        /// The anti XSRF user name key.
        /// </summary>
        private const string AntiXsrfUserNameKey = "__AntiXsrfUserName";

        /// <summary>
        /// The anti XSRF token value.
        /// </summary>
        private string antiXsrfTokenValue;

        #region Public Methods

        /// <summary>
        /// The init.
        /// </summary>
        public override void Init()
        {
            // hook the page init for mail sending...
            this.PageContext.Init += this.CurrentPageInit;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Currents the after initialize.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void CurrentPageInit([NotNull] object sender, [NotNull] EventArgs e)
        {
            var requestCookie = HttpContext.Current.Request.Cookies[AntiXsrfTokenKey];

            if (requestCookie != null && Guid.TryParse(requestCookie.Value, out _))
            {
                this.antiXsrfTokenValue = requestCookie.Value;

                this.PageContext.CurrentForumPage.Page.ViewStateUserKey = this.antiXsrfTokenValue;
            }
            else
            {
                // If the CSRF cookie is not found, then this is a new session.
                // Generate a new Anti-XSRF token
                this.antiXsrfTokenValue = Guid.NewGuid().ToString("N");

                this.PageContext.CurrentForumPage.Page.ViewStateUserKey = this.antiXsrfTokenValue;

                // Create the non-persistent CSRF cookie
                var responseCookie = new HttpCookie(AntiXsrfTokenKey)
                {
                    HttpOnly = true,
                    Value = this.antiXsrfTokenValue
                };

                if (HttpContext.Current.Request.IsSecureConnection)
                {
                    responseCookie.Secure = true;
                }

                HttpContext.Current.Response.Cookies.Set(responseCookie);
            }

            this.PageContext.CurrentForumPage.Page.PreLoad += this.Page_OnPreLoad;
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
            if (!this.PageContext.CurrentForumPage.Page.IsPostBack)
            {
                HttpContext.Current.Items[AntiXsrfTokenKey] = this.PageContext.CurrentForumPage.Page.ViewStateUserKey;
                HttpContext.Current.Items[AntiXsrfUserNameKey] = HttpContext.Current.User.Identity.Name ?? string.Empty;
            }
            else
            {
                // Validate the Anti-XSRF token
                if ((string)HttpContext.Current.Items[AntiXsrfTokenKey] != this.antiXsrfTokenValue ||
                    (string)HttpContext.Current.Items[AntiXsrfUserNameKey] !=
                    (HttpContext.Current.User.Identity.Name ?? string.Empty))
                {
                    throw new InvalidOperationException("Validation of Anti -XSRF token failed.");
                }
            }
        }

        #endregion
    }
}