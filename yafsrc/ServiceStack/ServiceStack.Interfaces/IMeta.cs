// ***********************************************************************
// <copyright file="IMeta.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************


using System.Collections.Generic;

namespace ServiceStack
{
    /// <summary>
    /// Interface IMeta
    /// </summary>
    public interface IMeta
    {
        /// <summary>
        /// Gets or sets the meta.
        /// </summary>
        /// <value>The meta.</value>
        Dictionary<string, string> Meta { get; set; }
    }

    /// <summary>
    /// Interface IHasSessionId
    /// </summary>
    public interface IHasSessionId
    {
        /// <summary>
        /// Gets or sets the session identifier.
        /// </summary>
        /// <value>The session identifier.</value>
        string SessionId { get; set; }
    }

    /// <summary>
    /// Interface IHasBearerToken
    /// </summary>
    public interface IHasBearerToken
    {
        /// <summary>
        /// Gets or sets the bearer token.
        /// </summary>
        /// <value>The bearer token.</value>
        string BearerToken { get; set; }
    }

    /// <summary>
    /// Interface IHasVersion
    /// </summary>
    public interface IHasVersion
    {
        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        int Version { get; set; }
    }
}