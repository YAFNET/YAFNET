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
}