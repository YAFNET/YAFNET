// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMutId.cs" company="ServiceStack, Inc.">
//   Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>
//   Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ServiceStack.Model;

/// <summary>
/// Interface IMutId
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IMutId<T>
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>The identifier.</value>
    T Id { get; set; }
}

/// <summary>
/// Interface IMutLongId
/// Implements the <see cref="ServiceStack.Model.IMutId{System.Int64}" />
/// </summary>
/// <seealso cref="ServiceStack.Model.IMutId{System.Int64}" />
public interface IMutLongId : IMutId<long>
{
}

/// <summary>
/// Interface IMutIntId
/// Implements the <see cref="ServiceStack.Model.IMutId{System.Int32}" />
/// </summary>
/// <seealso cref="ServiceStack.Model.IMutId{System.Int32}" />
public interface IMutIntId : IMutId<int>
{
}

/// <summary>
/// Interface IMutStringId
/// Implements the <see cref="ServiceStack.Model.IMutId{System.String}" />
/// </summary>
/// <seealso cref="ServiceStack.Model.IMutId{System.String}" />
public interface IMutStringId : IMutId<string>
{
}