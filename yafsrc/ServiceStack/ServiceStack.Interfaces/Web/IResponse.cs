// ***********************************************************************
// <copyright file="IResponse.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceStack.Web
{
    /// <summary>
    /// A thin wrapper around each host's Response e.g: ASP.NET, HttpListener, MQ, etc
    /// </summary>
    public interface IResponse
    {
        /// <summary>
        /// The underlying ASP.NET, .NET Core or HttpListener HttpResponse
        /// </summary>
        /// <value>The original response.</value>
        object OriginalResponse { get; }

        /// <summary>
        /// The corresponding IRequest API for this Response
        /// </summary>
        /// <value>The request.</value>
        IRequest Request { get; }

        /// <summary>
        /// The Response Status Code
        /// </summary>
        /// <value>The status code.</value>
        int StatusCode { get; set; }

        /// <summary>
        /// The Response Status Description
        /// </summary>
        /// <value>The status description.</value>
        string StatusDescription { get; set; }

        /// <summary>
        /// The Content-Type for this Response
        /// </summary>
        /// <value>The type of the content.</value>
        string ContentType { get; set; }

        /// <summary>
        /// Add a Header to this Response
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        void AddHeader(string name, string value);

        /// <summary>
        /// Remove an existing Header added on this Response
        /// </summary>
        /// <param name="name">The name.</param>
        void RemoveHeader(string name);

        /// <summary>
        /// Get an existing Header added to this Response
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        string GetHeader(string name);

        /// <summary>
        /// Return a Redirect Response to the URL specified
        /// </summary>
        /// <param name="url">The URL.</param>
        void Redirect(string url);

        /// <summary>
        /// The Response Body Output Stream
        /// </summary>
        /// <value>The output stream.</value>
        Stream OutputStream { get; }

        /// <summary>
        /// The Response DTO
        /// </summary>
        /// <value>The dto.</value>
        object Dto { get; set; }

        /// <summary>
        /// Buffer the Response OutputStream so it can be written in 1 batch
        /// </summary>
        /// <value><c>true</c> if [use buffered stream]; otherwise, <c>false</c>.</value>
        bool UseBufferedStream { get; set; }

        /// <summary>
        /// Signal that this response has been handled and no more processing should be done.
        /// When used in a request or response filter, no more filters or processing is done on this request.
        /// </summary>
        void Close();

        /// <summary>
        /// Close this Response Output Stream Async
        /// </summary>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task.</returns>
        Task CloseAsync(CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Calls Response.End() on ASP.NET HttpResponse otherwise is an alias for Close().
        /// Useful when you want to prevent ASP.NET to provide it's own custom error page.
        /// </summary>
        void End();

        /// <summary>
        /// Response.Flush() and OutputStream.Flush() seem to have different behaviour in ASP.NET
        /// </summary>
        void Flush();

        /// <summary>
        /// Flush this Response Output Stream Async
        /// </summary>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task.</returns>
        Task FlushAsync(CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Gets a value indicating whether this instance is closed.
        /// </summary>
        /// <value><c>true</c> if this instance is closed; otherwise, <c>false</c>.</value>
        bool IsClosed { get; }

        /// <summary>
        /// Set the Content Length in Bytes for this Response
        /// </summary>
        /// <param name="contentLength">Length of the content.</param>
        void SetContentLength(long contentLength);

        /// <summary>
        /// Whether the underlying TCP Connection for this Response should remain open
        /// </summary>
        /// <value><c>true</c> if [keep alive]; otherwise, <c>false</c>.</value>
        bool KeepAlive { get; set; }

        /// <summary>
        /// Whether the HTTP Response Headers have already been written.
        /// </summary>
        /// <value><c>true</c> if this instance has started; otherwise, <c>false</c>.</value>
        bool HasStarted { get; }

        //Add Metadata to Response
        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>The items.</value>
        Dictionary<string, object> Items { get; }
    }
}