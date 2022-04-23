// ***********************************************************************
// <copyright file="ConnectionInfo.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
namespace ServiceStack;

/// <summary>
/// Class ConnectionInfo.
/// </summary>
public class ConnectionInfo
{
    /// <summary>
    /// Gets or sets the named connection.
    /// </summary>
    /// <value>The named connection.</value>
    public string NamedConnection { get; set; }
    /// <summary>
    /// Gets or sets the connection string.
    /// </summary>
    /// <value>The connection string.</value>
    public string ConnectionString { get; set; }
    /// <summary>
    /// Gets or sets the name of the provider.
    /// </summary>
    /// <value>The name of the provider.</value>
    public string ProviderName { get; set; }
}