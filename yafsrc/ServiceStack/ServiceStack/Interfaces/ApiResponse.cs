// ***********************************************************************
// <copyright file="ApiResponse.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack;

using System;
using System.Net;

/// <summary>
/// Interface IApiResponseDescription
/// </summary>
public interface IApiResponseDescription
{
    /// <summary>
    /// The status code of a response
    /// </summary>
    /// <value>The status code.</value>
    int StatusCode { get; }

    /// <summary>
    /// The description of a response status code
    /// </summary>
    /// <value>The description.</value>
    string Description { get; }
}

/// <summary>
/// Class ApiResponseAttribute.
/// Implements the <see cref="ServiceStack.AttributeBase" />
/// Implements the <see cref="ServiceStack.IApiResponseDescription" />
/// </summary>
/// <seealso cref="ServiceStack.AttributeBase" />
/// <seealso cref="ServiceStack.IApiResponseDescription" />
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class ApiResponseAttribute : AttributeBase, IApiResponseDescription
{
    /// <summary>
    /// HTTP status code of response
    /// </summary>
    /// <value>The status code.</value>
    public int StatusCode { get; set; }

    /// <summary>
    /// End-user description of the data which is returned by response
    /// </summary>
    /// <value>The description.</value>
    public string Description { get; set; }

    /// <summary>
    /// If set to true, the response is default for all non-explicitly defined status codes
    /// </summary>
    /// <value><c>true</c> if this instance is default response; otherwise, <c>false</c>.</value>
    public bool IsDefaultResponse { get; set; }

    /// <summary>
    /// Open API schema definition type for response
    /// </summary>
    /// <value>The type of the response.</value>
    public Type ResponseType { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiResponseAttribute"/> class.
    /// </summary>
    public ApiResponseAttribute() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiResponseAttribute"/> class.
    /// </summary>
    /// <param name="statusCode">The status code.</param>
    /// <param name="description">The description.</param>
    public ApiResponseAttribute(HttpStatusCode statusCode, string description)
    {
        StatusCode = (int)statusCode;
        Description = description;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiResponseAttribute"/> class.
    /// </summary>
    /// <param name="statusCode">The status code.</param>
    /// <param name="description">The description.</param>
    public ApiResponseAttribute(int statusCode, string description)
    {
        StatusCode = statusCode;
        Description = description;
    }
}