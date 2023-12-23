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
    public NameValue? Authorization { get; set; }
    public LongRange? Range { get; set; }
    public List<NameValue> Headers { get; set; } = new();
    public void SetRange(long from, long? to = null) => Range = new(from, to);
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