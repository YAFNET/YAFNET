// ***********************************************************************
// <copyright file="IResponseStatusConvertible.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.Model
{
    //Allow Exceptions to Customize ResponseStatus returned
    /// <summary>
    /// Interface IResponseStatusConvertible
    /// </summary>
    public interface IResponseStatusConvertible
    {
        /// <summary>
        /// Converts to responsestatus.
        /// </summary>
        /// <returns>ResponseStatus.</returns>
        ResponseStatus ToResponseStatus();
    }
}