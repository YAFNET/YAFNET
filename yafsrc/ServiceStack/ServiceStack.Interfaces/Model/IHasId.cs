// ***********************************************************************
// <copyright file="IHasId.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.Model
{
    /// <summary>
    /// Interface IHasId
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHasId<T>
    {
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        T Id { get; }
    }
}