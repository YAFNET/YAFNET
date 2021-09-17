// ***********************************************************************
// <copyright file="IRequest.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;
using ServiceStack.Configuration;

namespace ServiceStack.Web
{
    /// <summary>
    /// A thin wrapper around each host's Request e.g: ASP.NET, HttpListener, MQ, etc
    /// </summary>
    public interface IRequest : IResolver
    {
        /// <summary>
        /// The underlying ASP.NET or HttpListener HttpRequest
        /// </summary>
        /// <value>The original request.</value>
        object OriginalRequest { get; }

        /// <summary>
        /// The Response API for this Request
        /// </summary>
        /// <value>The response.</value>
        IResponse Response { get; }

        /// <summary>
        /// The name of the service being called (e.g. Request DTO Name)
        /// </summary>
        /// <value>The name of the operation.</value>
        string OperationName { get; set; }

        /// <summary>
        /// The Verb / HttpMethod or Action for this request
        /// </summary>
        /// <value>The verb.</value>
        string Verb { get; }

        /// <summary>
        /// Different Attribute Enum flags classifying this Request
        /// </summary>
        /// <value>The request attributes.</value>
        RequestAttributes RequestAttributes { get; set; }

        /// <summary>
        /// Optional preferences for the processing of this Request
        /// </summary>
        /// <value>The request preferences.</value>
        IRequestPreferences RequestPreferences { get; }

        /// <summary>
        /// The Request DTO, after it has been deserialized.
        /// </summary>
        /// <value>The dto.</value>
        object Dto { get; set; }

        /// <summary>
        /// The request ContentType
        /// </summary>
        /// <value>The type of the content.</value>
        string ContentType { get; }

        /// <summary>
        /// Whether this was an Internal Request
        /// </summary>
        /// <value><c>true</c> if this instance is local; otherwise, <c>false</c>.</value>
        bool IsLocal { get; }

        /// <summary>
        /// The UserAgent for the request
        /// </summary>
        /// <value>The user agent.</value>
        string UserAgent { get; }

        /// <summary>
        /// A Dictionary of HTTP Cookies sent with this Request
        /// </summary>
        /// <value>The cookies.</value>
        IDictionary<string, System.Net.Cookie> Cookies { get; }

        /// <summary>
        /// The expected Response ContentType for this request
        /// </summary>
        /// <value>The type of the response content.</value>
        string ResponseContentType { get; set; }

        /// <summary>
        /// Whether the ResponseContentType has been explicitly overridden or whether it was just the default
        /// </summary>
        /// <value><c>true</c> if this instance has explicit response content type; otherwise, <c>false</c>.</value>
        bool HasExplicitResponseContentType { get; }

        /// <summary>
        /// Attach any data to this request that all filters and services can access.
        /// </summary>
        /// <value>The items.</value>
        Dictionary<string, object> Items { get; }

        /// <summary>
        /// The HTTP Headers in a NameValueCollection
        /// </summary>
        /// <value>The headers.</value>
        NameValueCollection Headers { get; }

        /// <summary>
        /// The ?query=string in a NameValueCollection
        /// </summary>
        /// <value>The query string.</value>
        NameValueCollection QueryString { get; }

        /// <summary>
        /// The HTTP POST'ed Form Data in a NameValueCollection
        /// </summary>
        /// <value>The form data.</value>
        NameValueCollection FormData { get; }
        /// <summary>
        /// Buffer the Request InputStream so it can be re-read
        /// </summary>
        /// <value><c>true</c> if [use buffered stream]; otherwise, <c>false</c>.</value>
        bool UseBufferedStream { get; set; }

        /// <summary>
        /// The entire string contents of Request.InputStream
        /// </summary>
        /// <returns>System.String.</returns>
        string GetRawBody();

        /// <summary>
        /// The entire string contents of Request.InputStream async
        /// </summary>
        /// <returns>Task&lt;System.String&gt;.</returns>
        Task<string> GetRawBodyAsync();

        /// <summary>
        /// Relative URL containing /path/info?query=string
        /// </summary>
        /// <value>The raw URL.</value>
        string RawUrl { get; }

        /// <summary>
        /// The Absolute URL for the request
        /// </summary>
        /// <value>The absolute URI.</value>
        string AbsoluteUri { get; }

        /// <summary>
        /// The Remote IP as reported by Request.UserHostAddress
        /// </summary>
        /// <value>The user host address.</value>
        string UserHostAddress { get; }

        /// <summary>
        /// The Remote Ip as reported by X-Forwarded-For, X-Real-IP or Request.UserHostAddress
        /// </summary>
        /// <value>The remote ip.</value>
        string RemoteIp { get; }

        /// <summary>
        /// The value of the Authorization Header used to send the Api Key, null if not available
        /// </summary>
        /// <value>The authorization.</value>
        string Authorization { get; }

        /// <summary>
        /// e.g. is https or not
        /// </summary>
        /// <value><c>true</c> if this instance is secure connection; otherwise, <c>false</c>.</value>
        bool IsSecureConnection { get; }

        /// <summary>
        /// Array of different Content-Types accepted by the client
        /// </summary>
        /// <value>The accept types.</value>
        string[] AcceptTypes { get; }

        /// <summary>
        /// The normalized /path/info for the request
        /// </summary>
        /// <value>The path information.</value>
        string PathInfo { get; }

        /// <summary>
        /// The original /path/info as sent
        /// </summary>
        /// <value>The original path information.</value>
        string OriginalPathInfo { get; }

        /// <summary>
        /// The Request Body Input Stream
        /// </summary>
        /// <value>The input stream.</value>
        Stream InputStream { get; }

        /// <summary>
        /// The size of the Request Body if provided
        /// </summary>
        /// <value>The length of the content.</value>
        long ContentLength { get; }

        /// <summary>
        /// Access to the multi-part/formdata files posted on this request
        /// </summary>
        /// <value>The files.</value>
        IHttpFile[] Files { get; }

        /// <summary>
        /// The value of the Referrer, null if not available
        /// </summary>
        /// <value>The URL referrer.</value>
        Uri UrlReferrer { get; }
    }
}
