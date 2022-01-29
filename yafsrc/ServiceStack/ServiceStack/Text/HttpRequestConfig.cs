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
    public KeyValuePair<string,string>? Authorization { get; set; }
    public Dictionary<string, string>? Headers { get; set; }

    public string AuthBearer
    {
        set => Authorization = new("Bearer", value);
    }
    
    public KeyValuePair<string, string> AuthBasic
    {
        set => Authorization = new("Basic",
            Convert.ToBase64String(Encoding.UTF8.GetBytes(value.Key + ":" + value.Value)));
    }
}
