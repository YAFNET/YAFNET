﻿/* Yet Another Forum.NET
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

namespace YAF.Configuration;

using System.Web.Hosting;

/// <summary>
/// The base url builder.
/// </summary>
public abstract class BaseUrlBuilder : IUrlBuilder
{
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
    public static string AppPath => TreatPathStr(Config.AppRoot);

    /// <summary>
    /// Gets ScriptName.
    /// </summary>
    public static string ScriptName
    {
        get
        {
            var scriptName = HttpContext.Current.Request.FilePath.ToLower();
            return scriptName.Substring(scriptName.LastIndexOf('/') + 1);
        }
    }

    /// <summary>
    /// Gets BaseUrl.
    /// </summary>
    public static string BaseUrl
    {
        get
        {
            string baseUrlMask;

            try
            {
                var boardSettings = MemoryCache.Default["BoardSettings$1"] as BoardSettings;

                baseUrlMask = boardSettings.BaseUrlMask.IsSet()
                                  ? TreatBaseUrl(boardSettings.BaseUrlMask)
                                  : GetBaseUrlFromVariables();
            }
            catch (Exception)
            {
                baseUrlMask = GetBaseUrlFromVariables();
            }

            return baseUrlMask;
        }
    }

    /// <summary>
    /// Gets the base URL from variables.
    /// </summary>
    /// <returns>
    /// The get base url from variables.
    /// </returns>
    
    public static string GetBaseUrlFromVariables()
    {
        var url = new StringBuilder();

        var serverPort = long.Parse(HttpContext.Current.Request.ServerVariables["SERVER_PORT"]);
        var isSecure = HttpContext.Current.Request.IsSecureConnection || serverPort == 443;

        url.Append(isSecure ? "https" : "http");

        url.AppendFormat("://{0}", HttpContext.Current.Request.ServerVariables["SERVER_NAME"]);

        if (!isSecure && serverPort != 80 || isSecure && serverPort != 443)
        {
            url.AppendFormat(":{0}", serverPort);
        }

        return url.ToString();
    }

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
        return $"{BaseUrl}{this.BuildUrl(url)}";
    }

    /// <summary>
    /// Treats the base URL.
    /// </summary>
    /// <param name="baseUrl">The base url.</param>
    /// <returns>
    /// The treat base url.
    /// </returns>
    static protected string TreatBaseUrl(string baseUrl)
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
    static protected string TreatPathStr(string altRoot)
    {
        var pathBuilder = new StringBuilder();

        try
        {
            pathBuilder.Append(HostingEnvironment.ApplicationVirtualPath);

            if (!HostingEnvironment.ApplicationVirtualPath.EndsWith("/"))
            {
                pathBuilder.Append("/");
            }

            if (altRoot.IsSet())
            {
                pathBuilder.Clear();

                // use specified root
                pathBuilder.Append(altRoot);

                if (altRoot.StartsWith("~"))
                {
                    // transform with application path...
                    pathBuilder = pathBuilder.Replace("~", HostingEnvironment.ApplicationVirtualPath);
                }

                if (pathBuilder[0] != '/')
                {
                    pathBuilder = pathBuilder.Insert(0, "/");
                }
            }
            else if (Config.IsDotNetNuke)
            {
                pathBuilder.Append("DesktopModules/YetAnotherForumDotNet/");
            }
            else if (Config.IsPortal)
            {
                pathBuilder.Append("Modules/Forum/");
            }

            if (!pathBuilder.ToString().EndsWith("/"))
            {
                pathBuilder.Append("/");
            }

            // remove redundant slashes...
            while (pathBuilder.ToString().Contains("//"))
            {
                pathBuilder = pathBuilder.Replace("//", "/");
            }
        }
        catch (Exception)
        {
            pathBuilder.Append("/");
        }

        return pathBuilder.ToString();
    }
}