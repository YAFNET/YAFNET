// ***********************************************************************
// <copyright file="Feature.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

#nullable enable
using System;

namespace ServiceStack;

/// <summary>
/// Enum Feature
/// </summary>
[Flags]
public enum Feature
{
    /// <summary>
    /// The none
    /// </summary>
    None = 0,

    /// <summary>
    /// All
    /// </summary>
    All = int.MaxValue,

    /// <summary>
    /// The SOAP
    /// </summary>
    Soap = Soap11 | Soap12,

    /// <summary>
    /// The metadata
    /// </summary>
    Metadata = 1 << 0,

    /// <summary>
    /// The predefined routes
    /// </summary>
    PredefinedRoutes = 1 << 1,

    /// <summary>
    /// The request information
    /// </summary>
    RequestInfo = 1 << 2,

    /// <summary>
    /// The json
    /// </summary>
    Json = 1 << 3,

    /// <summary>
    /// The XML
    /// </summary>
    Xml = 1 << 4,

    /// <summary>
    /// The JSV
    /// </summary>
    Jsv = 1 << 5,

    /// <summary>
    /// The soap11
    /// </summary>
    Soap11 = 1 << 6,
    /// <summary>
    /// The soap12
    /// </summary>
    Soap12 = 1 << 7,

    /// <summary>
    /// The CSV
    /// </summary>
    Csv = 1 << 8,

    /// <summary>
    /// The HTML
    /// </summary>
    Html = 1 << 9,

    /// <summary>
    /// The razor
    /// </summary>
    Razor = 1 << 12,

    /// <summary>
    /// The wire
    /// </summary>
    Wire = 1 << 15
}