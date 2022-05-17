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

public class HttpRequestConfig
{
    public string? Accept { get; set; }

    public string? UserAgent { get; set; }

    public string? ContentType { get; set; }
    public string? Referer { get; set; }
    public string? Expect { get; set; }
    public string[]? TransferEncoding { get; set; }
    public bool? TransferEncodingChunked { get; set; }
    public NameValue? Authorization { get; set; }
    public LongRange? Range { get; set; }
    public List<NameValue> Headers { get; set; } = new();
    public void SetAuthBearer(string value) => Authorization = new("Bearer", value);
    public void SetAuthBasic(string name, string value) =>
        Authorization = new("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(name + ":" + value)));
    public void SetRange(long from, long? to = null) => Range = new(from, to);
    public void AddHeader(string name, string value) => Headers.Add(new(name, value));
    public record NameValue(string Name, string Value)
    {
        public string Name { get; } = Name;

        public string Value { get; } = Value;

        public void Deconstruct(out string name, out string value)
        {
            name = this.Name;
            value = this.Value;
        }
    }

    public record LongRange(long From, long? To = null)
    {
        public long From { get; } = From;

        public long? To { get; } = To;

        public void Deconstruct(out long from, out long? to)
        {
            from = this.From;
            to = this.To;
        }
    }
}