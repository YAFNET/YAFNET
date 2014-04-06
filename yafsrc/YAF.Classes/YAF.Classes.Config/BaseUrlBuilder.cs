/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014 Ingo Herbote
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
namespace YAF.Classes
{
    #region Using

    using System;
    using System.Collections.Specialized;
    using System.Text;
    using System.Web;
    using System.Web.Hosting;

    using YAF.Types;
    using YAF.Types.Exceptions;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// The base url builder.
    /// </summary>
    public abstract class BaseUrlBuilder : IUrlBuilder
    {
        #region Constants and Fields

        /// <summary>
        /// The base URLs.
        /// </summary>
        private static readonly StringDictionary BaseUrls = new StringDictionary();

        #endregion

        #region Properties

        /// <summary>
        /// Gets BaseUrl.
        /// </summary>
        /// <exception cref="BaseUrlMaskRequiredException">Since there is no active context, a base url mask is required. Please specify in the AppSettings in your web.config.</exception>
        public static string BaseUrl
        {
            get
            {
                string baseUrl;

                try
                {
                    if (HttpContext.Current != null)
                    {
                        // urlKey requires SERVER_NAME in case of systems that use HostNames for seperate sites or in our cases Boards as well as FilePath for multiboards in seperate folders.
                        var urlKey = "{0}{1}".FormatWith(
                            HttpContext.Current.Request.ServerVariables["SERVER_NAME"],
                            HttpContext.Current.Request.FilePath);

                        // Lookup the AppRoot based on the current host + path. 
                        baseUrl = BaseUrls[urlKey];

                        if (baseUrl.IsNotSet())
                        {
                            // Each different filepath (multiboard) will specify a AppRoot key in their own web.config in their directory.
                            baseUrl = Config.BaseUrlMask.IsSet()
                                          ? TreatBaseUrl(Config.BaseUrlMask)
                                          : GetBaseUrlFromVariables();

                            // save to cache
                            BaseUrls[urlKey] = baseUrl;
                        }
                    }
                    else
                    {
                        if (Config.BaseUrlMask.IsNotSet())
                        {
                            throw new BaseUrlMaskRequiredException(
                                "Since there is no active context, a base url mask is required. Please specify in the AppSettings in your web.config: YAF.BaseUrlMask");
                        }

                        baseUrl = TreatBaseUrl(Config.BaseUrlMask);
                    }
                }
                catch (Exception)
                {
                    baseUrl = GetBaseUrlFromVariables();
                }

                return baseUrl;
            }
        }

        /// <summary>
        /// Gets ClientFileRoot.
        /// </summary>
        public static string ClientFileRoot
        {
            get
            {
                var altRoot = Config.ClientFileRoot;

                if (altRoot.IsNotSet() && Config.AppRoot.IsSet())
                {
                    // default to "AppRoot" if no file root specified and AppRoot specified...
                    altRoot = Config.AppRoot;
                }

                return TreatPathStr(altRoot);
            }
        }

        /// <summary>
        /// Gets ServerFileRoot.
        /// </summary>
        public static string ServerFileRoot
        {
            get
            {
                var altRoot = Config.ServerFileRoot;

                if (altRoot.IsNotSet() && Config.AppRoot.IsSet())
                {
                    // default to "AppRoot" if no file root specified and AppRoot specified...
                    altRoot = Config.AppRoot;
                }

                return TreatPathStr(altRoot);
            }
        }

        /// <summary>
        /// Gets App Path.
        /// </summary>
        public static string AppPath
        {
            get
            {
                return TreatPathStr(Config.AppRoot);
            }
        }

        /// <summary>
        /// Gets ScriptName.
        /// </summary>
        public static string ScriptName
        {
            get
            {
                string scriptName = HttpContext.Current.Request.FilePath.ToLower();
                return scriptName.Substring(scriptName.LastIndexOf('/') + 1);
            }
        }

        /// <summary>
        /// Gets ScriptNamePath.
        /// </summary>
        public static string ScriptNamePath
        {
            get
            {
                string scriptName = HttpContext.Current.Request.FilePath.ToLower();
                return scriptName.Substring(0, scriptName.LastIndexOf('/'));
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the base URL from variables.
        /// </summary>
        /// <returns>
        /// The get base url from variables.
        /// </returns>
        [NotNull]
        public static string GetBaseUrlFromVariables()
        {
            var url = new StringBuilder();

            long serverPort = long.Parse(HttpContext.Current.Request.ServerVariables["SERVER_PORT"]);
            bool isSecure = HttpContext.Current.Request.ServerVariables["HTTPS"].ToUpper() == "ON" || serverPort == 443;

            url.Append(isSecure ? "https" : "http");

            url.AppendFormat("://{0}", HttpContext.Current.Request.ServerVariables["SERVER_NAME"]);

            if ((!isSecure && serverPort != 80) || (isSecure && serverPort != 443))
            {
                url.AppendFormat(":{0}", serverPort);
            }

            return url.ToString();
        }

        #endregion

        #region Implemented Interfaces

        #region IUrlBuilder

        /// <summary>
        /// Builds the URL.
        /// </summary>
        /// <param name="url">The url.</param>
        /// <returns>
        /// Returns the URL
        /// </returns>
        public abstract string BuildUrl(string url);

        /// <summary>
        /// Builds the URL.
        /// </summary>
        /// <param name="boardSettings">The board settings.</param>
        /// <param name="url">The url.</param>
        /// <returns>
        /// Returns the URL
        /// </returns>
        public abstract string BuildUrl(object boardSettings, string url);

        /// <summary>
        /// Builds the Full URL.
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <returns>
        /// Returns the URL.
        /// </returns>
        public virtual string BuildUrlFull(string url)
        {
            // append the full base server url to the beginning of the url (e.g. http://mydomain.com)
            return "{0}{1}".FormatWith(BaseUrl, this.BuildUrl(url));
        }

        /// <summary>
        /// Builds the Full URL.
        /// </summary>
        /// <param name="boardSettings">The board settings.</param>
        /// <param name="url">The url.</param>
        /// <returns>
        /// Returns the URL.
        /// </returns>
        public virtual string BuildUrlFull(object boardSettings, string url)
        {
            // append the full base server url to the beginning of the url (e.g. http://mydomain.com)
            return "{0}{1}".FormatWith(BaseUrl, this.BuildUrl(url));
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Treats the base URL.
        /// </summary>
        /// <param name="baseUrl">The base url.</param>
        /// <returns>
        /// The treat base url.
        /// </returns>
        protected static string TreatBaseUrl(string baseUrl)
        {
            if (baseUrl.EndsWith("/"))
            {
                // remove ending slash...
                baseUrl = baseUrl.Remove(baseUrl.Length - 1, 1);
            }

            return baseUrl;
        }

        /// <summary>
        /// Treats the path string.
        /// </summary>
        /// <param name="altRoot">The alt root.</param>
        /// <returns>
        /// The treat path string.
        /// </returns>
        protected static string TreatPathStr(string altRoot)
        {
            var _pathBuilder = new StringBuilder();

            try
            {
                _pathBuilder.Append(HostingEnvironment.ApplicationVirtualPath);

                if (!HostingEnvironment.ApplicationVirtualPath.EndsWith("/"))
                {
                    _pathBuilder.Append("/");
                }

                if (altRoot.IsSet())
                {
                    _pathBuilder.Clear();

                    // use specified root
                    _pathBuilder.Append(altRoot);

                    if (altRoot.StartsWith("~"))
                    {
                        // transform with application path...
                        _pathBuilder = _pathBuilder.Replace("~", HostingEnvironment.ApplicationVirtualPath);
                    }

                    if (_pathBuilder[0] != '/')
                    {
                        _pathBuilder = _pathBuilder.Insert(0, "/");
                    }
                }
                else if (Config.IsDotNetNuke)
                {
                    _pathBuilder.Append("DesktopModules/YetAnotherForumDotNet/");
                }
                else if (Config.IsRainbow)
                {
                    _pathBuilder.Append("DesktopModules/Forum/");
                }
                else if (Config.IsPortal)
                {
                    _pathBuilder.Append("Modules/Forum/");
                }

                if (!_pathBuilder.ToString().EndsWith("/"))
                {
                    _pathBuilder.Append("/");
                }

                // remove redundant slashes...
                while (_pathBuilder.ToString().Contains("//"))
                {
                    _pathBuilder = _pathBuilder.Replace("//", "/");
                }
            }
            catch (Exception)
            {
                _pathBuilder.Append("/");
            }

            return _pathBuilder.ToString();
        }

        #endregion
    }
}