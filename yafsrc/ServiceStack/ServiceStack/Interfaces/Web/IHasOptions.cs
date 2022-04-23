// ***********************************************************************
// <copyright file="IHasOptions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System.Collections.Generic;

namespace ServiceStack.Web;

/// <summary>
/// Interface IHasOptions
/// </summary>
public interface IHasOptions
{
    /// <summary>
    /// Gets the options.
    /// </summary>
    /// <value>The options.</value>
    IDictionary<string, string> Options { get; }
}