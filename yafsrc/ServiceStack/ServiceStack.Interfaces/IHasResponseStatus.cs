// ***********************************************************************
// <copyright file="IHasResponseStatus.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack
{
    /// <summary>
    /// Contract indication that the Response DTO has a ResponseStatus
    /// </summary>
    public interface IHasResponseStatus
    {
        /// <summary>
        /// Gets or sets the response status.
        /// </summary>
        /// <value>The response status.</value>
        ResponseStatus ResponseStatus { get; set; }
    }
}