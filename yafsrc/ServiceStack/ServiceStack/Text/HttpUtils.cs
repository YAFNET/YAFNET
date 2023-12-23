// ***********************************************************************
// <copyright file="HttpUtils.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace ServiceStack;

/// <summary>
/// Class HttpUtils.
/// </summary>
public static partial class HttpUtils
{
    /// <summary>
    /// Gets or sets the use encoding.
    /// </summary>
    /// <value>The use encoding.</value>
    public static Encoding UseEncoding { get; set; } = new UTF8Encoding(false);

}