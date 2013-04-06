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
    #region

    using System;
    using System.Collections.Specialized;
    using System.IO;
    using System.Security.Permissions;
    using System.Security.Principal;
    using System.Text;
    using System.Web;

    #endregion

    /// <summary>
    /// The i http request.
    /// </summary>
    public interface IHttpRequest
    {
        // Properties
        #region Public Properties

        /// <summary>
        /// Gets AcceptTypes.
        /// </summary>
        string[] AcceptTypes { get; }

        /// <summary>
        /// Gets AnonymousID.
        /// </summary>
        string AnonymousID { get; }

        /// <summary>
        /// Gets AppRelativeCurrentExecutionFilePath.
        /// </summary>
        string AppRelativeCurrentExecutionFilePath { get; }

        /// <summary>
        /// Gets ApplicationPath.
        /// </summary>
        string ApplicationPath { get; }

        /// <summary>
        /// Gets or sets Browser.
        /// </summary>
        HttpBrowserCapabilities Browser { get; set; }

        /// <summary>
        /// Gets ClientCertificate.
        /// </summary>
        HttpClientCertificate ClientCertificate { [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Low)]
        get; }

        /// <summary>
        /// Gets or sets ContentEncoding.
        /// </summary>
        Encoding ContentEncoding { get; set; }

        /// <summary>
        /// Gets ContentLength.
        /// </summary>
        int ContentLength { get; }

        /// <summary>
        /// Gets or sets ContentType.
        /// </summary>
        string ContentType { get; set; }

        /// <summary>
        /// Gets Cookies.
        /// </summary>
        HttpCookieCollection Cookies { get; }

        /// <summary>
        /// Gets CurrentExecutionFilePath.
        /// </summary>
        string CurrentExecutionFilePath { get; }

        /// <summary>
        /// Gets FilePath.
        /// </summary>
        string FilePath { get; }

        /// <summary>
        /// Gets Files.
        /// </summary>
        HttpFileCollection Files { get; }

        /// <summary>
        /// Gets or sets Filter.
        /// </summary>
        Stream Filter { get; set; }

        /// <summary>
        /// Gets Form.
        /// </summary>
        NameValueCollection Form { get; }

        /// <summary>
        /// Gets Headers.
        /// </summary>
        NameValueCollection Headers { get; }

        /// <summary>
        /// Gets HttpMethod.
        /// </summary>
        string HttpMethod { get; }

        /// <summary>
        /// Gets InputStream.
        /// </summary>
        Stream InputStream { get; }

        /// <summary>
        /// Gets a value indicating whether IsAuthenticated.
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// Gets a value indicating whether IsLocal.
        /// </summary>
        bool IsLocal { get; }

        /// <summary>
        /// Gets a value indicating whether IsSecureConnection.
        /// </summary>
        bool IsSecureConnection { get; }

        /// <summary>
        /// Gets LogonUserIdentity.
        /// </summary>
        WindowsIdentity LogonUserIdentity { [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Medium)]
        get; }

        /// <summary>
        /// Gets Params.
        /// </summary>
        NameValueCollection Params { get; }

        /// <summary>
        /// Gets Path.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Gets PathInfo.
        /// </summary>
        string PathInfo { get; }

        /// <summary>
        /// Gets PhysicalApplicationPath.
        /// </summary>
        string PhysicalApplicationPath { get; }

        /// <summary>
        /// Gets PhysicalPath.
        /// </summary>
        string PhysicalPath { get; }

        /// <summary>
        /// Gets QueryString.
        /// </summary>
        NameValueCollection QueryString { get; }

        /// <summary>
        /// Gets RawUrl.
        /// </summary>
        string RawUrl { get; }

        /// <summary>
        /// Gets or sets RequestType.
        /// </summary>
        string RequestType { get; set; }

        /// <summary>
        /// Gets ServerVariables.
        /// </summary>
        NameValueCollection ServerVariables { get; }

        /// <summary>
        /// Gets TotalBytes.
        /// </summary>
        int TotalBytes { get; }

        /// <summary>
        /// Gets Url.
        /// </summary>
        Uri Url { get; }

        /// <summary>
        /// Gets UrlReferrer.
        /// </summary>
        Uri UrlReferrer { get; }

        /// <summary>
        /// Gets UserAgent.
        /// </summary>
        string UserAgent { get; }

        /// <summary>
        /// Gets UserHostAddress.
        /// </summary>
        string UserHostAddress { get; }

        /// <summary>
        /// Gets UserHostName.
        /// </summary>
        string UserHostName { get; }

        /// <summary>
        /// Gets UserLanguages.
        /// </summary>
        string[] UserLanguages { get; }

        #endregion

        #region Public Indexers

        /// <summary>
        /// The this.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        string this[string key] { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The binary read.
        /// </summary>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <returns>
        /// </returns>
        byte[] BinaryRead(int count);

        /// <summary>
        /// The map image coordinates.
        /// </summary>
        /// <param name="imageFieldName">
        /// The image field name.
        /// </param>
        /// <returns>
        /// </returns>
        int[] MapImageCoordinates(string imageFieldName);

        /// <summary>
        /// The map path.
        /// </summary>
        /// <param name="virtualPath">
        /// The virtual path.
        /// </param>
        /// <returns>
        /// The map path.
        /// </returns>
        string MapPath(string virtualPath);

        /// <summary>
        /// The map path.
        /// </summary>
        /// <param name="virtualPath">
        /// The virtual path.
        /// </param>
        /// <param name="baseVirtualDir">
        /// The base virtual dir.
        /// </param>
        /// <param name="allowCrossAppMapping">
        /// The allow cross app mapping.
        /// </param>
        /// <returns>
        /// The map path.
        /// </returns>
        string MapPath(string virtualPath, string baseVirtualDir, bool allowCrossAppMapping);

        /// <summary>
        /// The save as.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <param name="includeHeaders">
        /// The include headers.
        /// </param>
        void SaveAs(string filename, bool includeHeaders);

        /// <summary>
        /// The validate input.
        /// </summary>
        void ValidateInput();

        #endregion
    }
}