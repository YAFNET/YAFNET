// ***********************************************************************
// <copyright file="RequestLogEntry.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using ServiceStack.DataAnnotations;

namespace ServiceStack
{
    /// <summary>
    /// A log entry added by the IRequestLogger
    /// </summary>
    public class RequestLogEntry : IMeta
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [AutoIncrement]
        public long Id { get; set; }
        /// <summary>
        /// Gets or sets the date time.
        /// </summary>
        /// <value>The date time.</value>
        public DateTime DateTime { get; set; }
        /// <summary>
        /// Gets or sets the status code.
        /// </summary>
        /// <value>The status code.</value>
        public int StatusCode { get; set; }
        /// <summary>
        /// Gets or sets the status description.
        /// </summary>
        /// <value>The status description.</value>
        public string StatusDescription { get; set; }
        /// <summary>
        /// Gets or sets the HTTP method.
        /// </summary>
        /// <value>The HTTP method.</value>
        public string HttpMethod { get; set; }
        /// <summary>
        /// Gets or sets the absolute URI.
        /// </summary>
        /// <value>The absolute URI.</value>
        public string AbsoluteUri { get; set; }
        /// <summary>
        /// Gets or sets the path information.
        /// </summary>
        /// <value>The path information.</value>
        public string PathInfo { get; set; }
        /// <summary>
        /// Gets or sets the request body.
        /// </summary>
        /// <value>The request body.</value>
        [StringLength(StringLengthAttribute.MaxText)]
        public string RequestBody { get; set; }
        /// <summary>
        /// Gets or sets the request dto.
        /// </summary>
        /// <value>The request dto.</value>
        public object RequestDto { get; set; }
        /// <summary>
        /// Gets or sets the user authentication identifier.
        /// </summary>
        /// <value>The user authentication identifier.</value>
        public string UserAuthId { get; set; }
        /// <summary>
        /// Gets or sets the session identifier.
        /// </summary>
        /// <value>The session identifier.</value>
        public string SessionId { get; set; }
        /// <summary>
        /// Gets or sets the ip address.
        /// </summary>
        /// <value>The ip address.</value>
        public string IpAddress { get; set; }
        /// <summary>
        /// Gets or sets the forwarded for.
        /// </summary>
        /// <value>The forwarded for.</value>
        public string ForwardedFor { get; set; }
        /// <summary>
        /// Gets or sets the referer.
        /// </summary>
        /// <value>The referer.</value>
        public string Referer { get; set; }
        /// <summary>
        /// Gets or sets the headers.
        /// </summary>
        /// <value>The headers.</value>
        public Dictionary<string, string> Headers { get; set; }
        /// <summary>
        /// Gets or sets the form data.
        /// </summary>
        /// <value>The form data.</value>
        public Dictionary<string, string> FormData { get; set; }
        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        public Dictionary<string, string> Items { get; set; }
        /// <summary>
        /// Gets or sets the session.
        /// </summary>
        /// <value>The session.</value>
        public object Session { get; set; }
        /// <summary>
        /// Gets or sets the response dto.
        /// </summary>
        /// <value>The response dto.</value>
        public object ResponseDto { get; set; }
        /// <summary>
        /// Gets or sets the error response.
        /// </summary>
        /// <value>The error response.</value>
        public object ErrorResponse { get; set; }
        /// <summary>
        /// Gets or sets the exception source.
        /// </summary>
        /// <value>The exception source.</value>
        public string ExceptionSource { get; set; }
        /// <summary>
        /// Gets or sets the exception data.
        /// </summary>
        /// <value>The exception data.</value>
        public IDictionary ExceptionData { get; set; }
        /// <summary>
        /// Gets or sets the duration of the request.
        /// </summary>
        /// <value>The duration of the request.</value>
        public TimeSpan RequestDuration { get; set; }
        /// <summary>
        /// Gets or sets the meta.
        /// </summary>
        /// <value>The meta.</value>
        public Dictionary<string, string> Meta { get; set; }
    }
}