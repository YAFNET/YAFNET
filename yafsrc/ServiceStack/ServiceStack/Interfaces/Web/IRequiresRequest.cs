// ***********************************************************************
// <copyright file="IRequiresRequest.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
namespace ServiceStack.Web
{
    /// <summary>
    /// Implement on services that need access to the RequestContext
    /// </summary>
    public interface IRequiresRequest
    {
        /// <summary>
        /// Gets or sets the request.
        /// </summary>
        /// <value>The request.</value>
        IRequest Request { get; set; }
    }
}