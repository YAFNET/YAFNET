// ***********************************************************************
// <copyright file="HttpRequestConfig.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

#nullable enable

using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceStack;

/// <summary>
/// Class HttpRequestConfig.
/// </summary>
public class HttpRequestConfig
{
    /// <summary>
    /// Gets or sets the accept.
    /// </summary>
    /// <value>The accept.</value>
    public string? Accept { get; set; }

    /// <summary>
    /// Gets or sets the user agent.
    /// </summary>
    /// <value>The user agent.</value>
    public string? UserAgent { get; set; }

    /// <summary>
    /// Gets or sets the type of the content.
    /// </summary>
    /// <value>The type of the content.</value>
    public string? ContentType { get; set; }
    /// <summary>
    /// Gets or sets the referer.
    /// </summary>
    /// <value>The referer.</value>
    public string? Referer { get; set; }
    /// <summary>
    /// Gets or sets the expect.
    /// </summary>
    /// <value>The expect.</value>
    public string? Expect { get; set; }
    /// <summary>
    /// Gets or sets the transfer encoding.
    /// </summary>
    /// <value>The transfer encoding.</value>
    public string[]? TransferEncoding { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether [transfer encoding chunked].
    /// </summary>
    /// <value><c>null</c> if [transfer encoding chunked] contains no value, <c>true</c> if [transfer encoding chunked]; otherwise, <c>false</c>.</value>
    public bool? TransferEncodingChunked { get; set; }
    /// <summary>
    /// Gets or sets the authorization.
    /// </summary>
    /// <value>The authorization.</value>
    public NameValue? Authorization { get; set; }
    /// <summary>
    /// Gets or sets the range.
    /// </summary>
    /// <value>The range.</value>
    public LongRange? Range { get; set; }
    /// <summary>
    /// Gets or sets the headers.
    /// </summary>
    /// <value>The headers.</value>
    public List<NameValue> Headers { get; set; } = new();
    /// <summary>
    /// Sets the authentication bearer.
    /// </summary>
    /// <param name="value">The value.</param>
    public void SetAuthBearer(string value) => Authorization = new("Bearer", value);
    /// <summary>
    /// Sets the authentication basic.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    public void SetAuthBasic(string name, string value) =>
        Authorization = new("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(name + ":" + value)));
    /// <summary>
    /// Sets the range.
    /// </summary>
    /// <param name="from">From.</param>
    /// <param name="to">To.</param>
    public void SetRange(long from, long? to = null) => Range = new(from, to);
    /// <summary>
    /// Adds the header.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    public void AddHeader(string name, string value) => Headers.Add(new(name, value));
    /// <summary>
    /// Class NameValue.
    /// </summary>
    public record NameValue(string Name, string Value)
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; } = Name;

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value { get; } = Value;

        /// <summary>
        /// Deconstructs the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void Deconstruct(out string name, out string value)
        {
            name = this.Name;
            value = this.Value;
        }
    }

    /// <summary>
    /// Class LongRange.
    /// </summary>
    public record LongRange(long From, long? To = null)
    {
        /// <summary>
        /// Gets from.
        /// </summary>
        /// <value>From.</value>
        public long From { get; } = From;

        /// <summary>
        /// Gets to.
        /// </summary>
        /// <value>To.</value>
        public long? To { get; } = To;

        /// <summary>
        /// Deconstructs the specified from.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        public void Deconstruct(out long from, out long? to)
        {
            from = this.From;
            to = this.To;
        }
    }
}