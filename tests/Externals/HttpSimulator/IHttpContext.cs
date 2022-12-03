/* httpcontext-simulator 
 * a simulator used to simulate http context during integration testing
 *
 * Copyright (C) Phil Haack 
 * http://code.google.com/p/httpcontext-simulator/
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
 * documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
 * the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
 * to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions 
 * of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
 * CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
 * DEALINGS IN THE SOFTWARE.
*/

namespace HttpSimulator
{
    using System;
    using System.Collections;
    using System.Security.Permissions;
    using System.Security.Principal;
    using System.Web;
    using System.Web.Caching;
    using System.Web.Profile;
    using System.Web.SessionState;

    /// <summary>
    /// The i http context.
    /// </summary>
    public interface IHttpContext
    {
        // Properties

        /// <summary>
        /// Gets AllErrors.
        /// </summary>
        Exception[] AllErrors { get; }

        /// <summary>
        /// Gets Application.
        /// </summary>
        HttpApplicationState Application { get; }

        /// <summary>
        /// Gets or sets ApplicationInstance.
        /// </summary>
        HttpApplication ApplicationInstance { get; set; }

        /// <summary>
        /// Gets Cache.
        /// </summary>
        Cache Cache { get; }

        /// <summary>
        /// Gets CurrentHandler.
        /// </summary>
        IHttpHandler CurrentHandler { get; }

        /// <summary>
        /// Gets CurrentNotification.
        /// </summary>
        RequestNotification CurrentNotification { get; }

        /// <summary>
        /// Gets Error.
        /// </summary>
        Exception Error { get; }

        /// <summary>
        /// Gets or sets Handler.
        /// </summary>
        IHttpHandler Handler { get; set; }

        /// <summary>
        /// Gets a value indicating whether IsCustomErrorEnabled.
        /// </summary>
        bool IsCustomErrorEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether IsDebuggingEnabled.
        /// </summary>
        bool IsDebuggingEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether IsPostNotification.
        /// </summary>
        bool IsPostNotification { get; }

        /// <summary>
        /// Gets Items.
        /// </summary>
        IDictionary Items { get; }

        /// <summary>
        /// Gets PreviousHandler.
        /// </summary>
        IHttpHandler PreviousHandler { get; }

        /// <summary>
        /// Gets Profile.
        /// </summary>
        ProfileBase Profile { get; }

        /// <summary>
        /// Gets Request.
        /// </summary>
        IHttpRequest Request { get; }

        /// <summary>
        /// Gets Response.
        /// </summary>
        IHttpResponse Response { get; }

        /// <summary>
        /// Gets Server.
        /// </summary>
        HttpServerUtility Server { get; }

        /// <summary>
        /// Gets Session.
        /// </summary>
        HttpSessionState Session { get; }

        /// <summary>
        /// Gets or sets a value indicating whether SkipAuthorization.
        /// </summary>
        bool SkipAuthorization
        {
            get;
            [SecurityPermission(SecurityAction.Demand, ControlPrincipal = true)]
            set;
        }

        /// <summary>
        /// Gets Timestamp.
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// Gets Trace.
        /// </summary>
        TraceContext Trace { get; }

        /// <summary>
        /// Gets or sets User.
        /// </summary>
        IPrincipal User
        {
            get;
            [SecurityPermission(SecurityAction.Demand, ControlPrincipal = true)]
            set;
        }

        /// <summary>
        /// The add error.
        /// </summary>
        /// <param name="errorInfo">
        /// The error info.
        /// </param>
        void AddError(Exception errorInfo);

        /// <summary>
        /// The clear error.
        /// </summary>
        void ClearError();

        /// <summary>
        /// The get section.
        /// </summary>
        /// <param name="sectionName">
        /// The section name.
        /// </param>
        /// <returns>
        /// The get section.
        /// </returns>
        object GetSection(string sectionName);

        /// <summary>
        /// The rewrite path.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        void RewritePath(string path);

        /// <summary>
        /// The rewrite path.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="rebaseClientPath">
        /// The rebase client path.
        /// </param>
        void RewritePath(string path, bool rebaseClientPath);

        /// <summary>
        /// The rewrite path.
        /// </summary>
        /// <param name="filePath">
        /// The file path.
        /// </param>
        /// <param name="pathInfo">
        /// The path info.
        /// </param>
        /// <param name="queryString">
        /// The query string.
        /// </param>
        void RewritePath(string filePath, string pathInfo, string queryString);

        /// <summary>
        /// The rewrite path.
        /// </summary>
        /// <param name="filePath">
        /// The file path.
        /// </param>
        /// <param name="pathInfo">
        /// The path info.
        /// </param>
        /// <param name="queryString">
        /// The query string.
        /// </param>
        /// <param name="setClientFilePath">
        /// The set client file path.
        /// </param>
        void RewritePath(string filePath, string pathInfo, string queryString, bool setClientFilePath);
    }
}