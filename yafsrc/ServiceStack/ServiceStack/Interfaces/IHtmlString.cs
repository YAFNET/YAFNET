// ***********************************************************************
// <copyright file="IHtmlString.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
namespace ServiceStack;

/// <summary>
/// Interface IHtmlString
/// </summary>
public interface IHtmlString
{
    /// <summary>
    /// Converts to htmlstring.
    /// </summary>
    /// <returns>System.String.</returns>
    string ToHtmlString();
}

/// <summary>
/// Interface IRawString
/// </summary>
public interface IRawString
{
    /// <summary>
    /// Converts to rawstring.
    /// </summary>
    /// <returns>System.String.</returns>
    string ToRawString();
}