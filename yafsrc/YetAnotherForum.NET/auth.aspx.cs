/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
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

namespace YAF
{
    using System;
    using System.Text;
    using System.Web;
    using System.Web.UI;

    using YAF.Core;
    using YAF.Core.Services.Auth;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    /// <summary>
    /// The Twitter Authentication Page
    /// </summary>
    public partial class Auth : Page
    {
        /// <summary>
        /// The script begin tag
        /// </summary>
        private const string ScriptBeginTag = "<script type='text/javascript'>";

        /// <summary>
        /// The script end tag
        /// </summary>
        private const string ScriptEndTag = "</script>";

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            var closeScript = new StringBuilder();

            closeScript.Append(value: "if (window.opener != null)");
            closeScript.AppendFormat(
                format: "{{window.opener.location.href = '{0}';window.close();}}",
                arg0: YafBuildLink.GetLink(page: ForumPages.login).Replace(oldValue: "auth.aspx", newValue: "default.aspx"));
            closeScript.AppendFormat(
                format: "else {{ window.location.href = '{0}' }}",
                arg0: YafBuildLink.GetLink(page: ForumPages.login).Replace(oldValue: "auth.aspx", newValue: "default.aspx"));

            if (this.Request.QueryString.GetFirstOrDefault(paramName: "denied") != null)
            {
                this.Response.Clear();

                this.Response.Write(s: $"{ScriptBeginTag} {closeScript} {ScriptEndTag}");

                return;
            }

            var loginAuth =
                (AuthService)
                Enum.Parse(
                    enumType: typeof(AuthService),
                    value: YafContext.Current.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAs<string>(paramName: "auth"),
                    ignoreCase: true);

            switch (loginAuth)
            {
                case AuthService.twitter:
                    this.HandleTwitterReturn();
                    break;
                case AuthService.facebook:
                    this.HandleFacebookReturn(); 
                    break;
                case AuthService.google:
                    this.HandleGoogleReturn();
                    break;
                default:
                    this.Response.Write(s: $"{ScriptBeginTag} {closeScript} {ScriptEndTag}");
                    break;
            }
        }

        /// <summary>
        /// Handles the twitter return.
        /// </summary>
        private void HandleTwitterReturn()
        {
            string message;

            var twitterAuth = new Twitter();

            if (YafContext.Current.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAs<bool>(paramName: "connectCurrent"))
            {
                try
                {
                    twitterAuth.ConnectUser(request: this.Request, parameters: null, message: out message);
                }
                catch (Exception ex)
                {
                    YafContext.Current.Get<ILogger>().Error(ex: ex, format: "Error while trying to connect the twitter user");

                    message = ex.Message;
                }

                if (message.IsSet())
                {
                    this.Response.Write(
                        s: string.Format(
                            format: "{2} alert('{0}');window.location.href = '{1}'; {3}",
                            message,
                                YafBuildLink.GetLink(page: ForumPages.login).Replace(oldValue: "auth.aspx", newValue: "default.aspx"),
                                ScriptBeginTag,
                                ScriptEndTag));
                }
                else
                {
                    YafBuildLink.Redirect(page: ForumPages.forum);
                }
            }
            else
            {
                try
                {
                    twitterAuth.LoginOrCreateUser(request: this.Request, parameters: null, message: out message);
                }
                catch (Exception ex)
                {
                    YafContext.Current.Get<ILogger>()
                        .Error(ex: ex, format: "Error while trying to login or register the twitter user");

                    message = ex.Message;
                }

                this.Response.Clear();

                if (message.IsSet())
                {
                    this.Response.Write(
                        s: string.Format(
                            format: "{2} alert('{0}');window.opener.location.href = '{1}';window.close(); {3}>",
                            message,
                                YafBuildLink.GetLink(page: ForumPages.login).Replace(oldValue: "auth.aspx", newValue: "default.aspx"),
                                ScriptBeginTag,
                                ScriptEndTag));
                }
                else
                {
                    this.Response.Write(
                        s: string.Format(
                            format: "{1} window.opener.location.href = '{0}';window.close(); {2}>",
                            arg0: YafBuildLink.GetLink(page: ForumPages.forum).Replace(oldValue: "auth.aspx", newValue: "default.aspx"),
                                arg1: ScriptBeginTag,
                                arg2: ScriptEndTag));
                }
            }
        }

        /// <summary>
        /// Handles the facebook return.
        /// </summary>
        private void HandleFacebookReturn()
        {
            var facebookAuth = new Facebook();

            if (YafContext.Current.Get<HttpRequestBase>().QueryString.GetFirstOrDefault(paramName: "code") != null)
            {
                var authorizationCode = YafContext.Current.Get<HttpRequestBase>().QueryString.GetFirstOrDefault(paramName: "code");
                var accessToken = facebookAuth.GetAccessToken(authorizationCode: authorizationCode, request: this.Request);

                if (accessToken.IsNotSet())
                {
                    this.Response.Write(
                        s: string.Format(
                            format: "{2} alert('{0}');window.location.href = '{1}'; {3}",
                            YafContext.Current.Get<ILocalization>().GetText(text: "AUTH_NO_ACCESS_TOKEN"),
                                YafBuildLink.GetLink(page: ForumPages.login).Replace(oldValue: "auth.aspx", newValue: "default.aspx"),
                                ScriptBeginTag,
                                ScriptEndTag));

                    return;
                }

                if (YafContext.Current.Get<HttpRequestBase>().QueryString.GetFirstOrDefault(paramName: "state") != null
                    && YafContext.Current.Get<HttpRequestBase>().QueryString.GetFirstOrDefault(paramName: "state")
                    == "connectCurrent")
                {
                    string message;

                    try
                    {
                        facebookAuth.ConnectUser(request: this.Request, parameters: accessToken, message: out message);
                    }
                    catch (Exception ex)
                    {
                        YafContext.Current.Get<ILogger>().Error(ex: ex, format: "Error while trying to connect the facebook user");

                        message = ex.Message;
                    }

                    if (message.IsSet())
                    {
                        this.Response.Write(
                            s: string.Format(
                                format: "{2} alert('{0}');window.location.href = '{1}'; {3}",
                                message,
                                    YafBuildLink.GetLink(page: ForumPages.forum).Replace(oldValue: "auth.aspx", newValue: "default.aspx"),
                                    ScriptBeginTag,
                                    ScriptEndTag));
                    }
                    else
                    {
                        YafBuildLink.Redirect(page: ForumPages.forum);
                    }
                }
                else
                {
                    string message;

                    try
                    {
                        facebookAuth.LoginOrCreateUser(request: this.Request, parameters: accessToken, message: out message);
                    }
                    catch (Exception ex)
                    {
                        YafContext.Current.Get<ILogger>()
                            .Error(ex: ex, format: "Error while trying to login or register the facebook user");

                        message = ex.Message;
                    }

                    this.Response.Clear();

                    if (message.IsSet())
                    {
                        this.Response.Write(
                            s: string.Format(
                                format: "{2} alert('{0}');window.location.href = '{1}';window.close(); {3}",
                                message,
                                    YafBuildLink.GetLink(page: ForumPages.login).Replace(oldValue: "auth.aspx", newValue: "default.aspx"),
                                    ScriptBeginTag,
                                    ScriptEndTag));
                    }
                    else
                    {
                        this.Response.Write(
                            s: string.Format(
                                format: "{1} window.location.href = '{0}';window.close(); {2}",
                                arg0: YafBuildLink.GetLink(page: ForumPages.forum).Replace(oldValue: "auth.aspx", newValue: "default.aspx"),
                                    arg1: ScriptBeginTag,
                                    arg2: ScriptEndTag));
                    }
                }
            }
            else if (YafContext.Current.Get<HttpRequestBase>().QueryString.GetFirstOrDefault(paramName: "error") != null)
            {
                // Return to login page if user cancels social login
                this.Response.Redirect(url: YafBuildLink.GetLink(page: ForumPages.login, fullUrl: true));
            }
            else
            {
                // Authorize first
                this.Response.Redirect(url: facebookAuth.GetAuthorizeUrl(request: this.Request), endResponse: true);
            }
        }

        /// <summary>
        /// Handles the Google return.
        /// </summary>
        private void HandleGoogleReturn()
        {
            var googleAuth = new Google();

            if (YafContext.Current.Get<HttpRequestBase>().QueryString.GetFirstOrDefault(paramName: "code") != null)
            {
                var authorizationCode = YafContext.Current.Get<HttpRequestBase>().QueryString.GetFirstOrDefault(paramName: "code");
                var accessTokens = googleAuth.GetAccessToken(authorizationCode: authorizationCode, request: this.Request);

                if (accessTokens.AccessToken == null)
                {
                    this.Response.Write(
                        s: string.Format(
                            format: "{2} alert('{0}');window.location.href = '{1}'; {3}",
                            YafContext.Current.Get<ILocalization>().GetText(text: "AUTH_NO_ACCESS_TOKEN"),
                                YafBuildLink.GetLink(page: ForumPages.login).Replace(oldValue: "auth.aspx", newValue: "default.aspx"),
                                ScriptBeginTag,
                                ScriptEndTag));

                    return;
                }

                if (YafContext.Current.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAs<bool>(paramName: "connectCurrent"))
                {
                    string message;

                    try
                    {
                        googleAuth.ConnectUser(request: this.Request, parameters: accessTokens.AccessToken, message: out message);
                    }
                    catch (Exception ex)
                    {
                        YafContext.Current.Get<ILogger>().Error(ex: ex, format: "Error while trying to connect the google user");

                        message = ex.Message;
                    }

                    if (message.IsSet())
                    {
                        this.Response.Write(
                            s: string.Format(
                                format: "{2} alert('{0}');window.location.href = '{1}'; {3}",
                                message,
                                    YafBuildLink.GetLink(page: ForumPages.forum).Replace(oldValue: "auth.aspx", newValue: "default.aspx"),
                                    ScriptBeginTag,
                                    ScriptEndTag));
                    }
                    else
                    {
                        YafBuildLink.Redirect(page: ForumPages.forum);
                    }
                }
                else
                {
                    string message;

                    try
                    {
                        googleAuth.LoginOrCreateUser(request: this.Request, parameters: accessTokens.AccessToken, message: out message);
                    }
                    catch (Exception ex)
                    {
                        YafContext.Current.Get<ILogger>()
                            .Error(ex: ex, format: "Error while trying to login or register the google user");

                        message = ex.Message;
                    }

                    this.Response.Clear();

                    if (message.IsSet())
                    {
                        this.Response.Write(
                            s: string.Format(
                                format: "{2} alert('{0}');window.location.href = '{1}';window.close(); {3}",
                                message,
                                    YafBuildLink.GetLink(page: ForumPages.login).Replace(oldValue: "auth.aspx", newValue: "default.aspx"),
                                    ScriptBeginTag,
                                    ScriptEndTag));
                    }
                    else
                    {
                        this.Response.Write(
                            s: string.Format(
                                format: "{1} window.location.href = '{0}';window.close(); {2}",
                                arg0: YafBuildLink.GetLink(page: ForumPages.forum).Replace(oldValue: "auth.aspx", newValue: "default.aspx"),
                                    arg1: ScriptBeginTag,
                                    arg2: ScriptEndTag));
                    }
                }
            }
            else if (YafContext.Current.Get<HttpRequestBase>().QueryString.GetFirstOrDefault(paramName: "error") != null)
            {
                // Return to login page if user cancels social login
                this.Response.Redirect(url: YafBuildLink.GetLink(page: ForumPages.login, fullUrl: true));
            }
            else
            {
                // Authorize first
                this.Response.Redirect(url: googleAuth.GetAuthorizeUrl(request: this.Request), endResponse: true);
            }
        }
    }
}