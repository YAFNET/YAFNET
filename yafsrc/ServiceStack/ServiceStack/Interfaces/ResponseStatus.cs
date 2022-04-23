// ***********************************************************************
// <copyright file="ResponseStatus.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ServiceStack;

/// <summary>
/// Common ResponseStatus class that should be present on all response DTO's
/// </summary>
[DataContract]
public class ResponseStatus : IMeta
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResponseStatus" /> class.
    /// A response status without an errorcode == success
    /// </summary>
    public ResponseStatus() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResponseStatus" /> class.
    /// A response status with an errorcode == failure
    /// </summary>
    /// <param name="errorCode">The error code.</param>
    public ResponseStatus(string errorCode)
    {
        this.ErrorCode = errorCode;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResponseStatus" /> class.
    /// A response status with an errorcode == failure
    /// </summary>
    /// <param name="errorCode">The error code.</param>
    /// <param name="message">The message.</param>
    public ResponseStatus(string errorCode, string message)
        : this(errorCode)
    {
        this.Message = message;
    }

    /// <summary>
    /// Holds the custom ErrorCode enum if provided in ValidationException
    /// otherwise will hold the name of the Exception type, e.g. typeof(Exception).Name
    /// A value of non-null means the service encountered an error while processing the request.
    /// </summary>
    /// <value>The error code.</value>
    [DataMember(Order = 1)]
    public string ErrorCode { get; set; }

    /// <summary>
    /// A human friendly error message
    /// </summary>
    /// <value>The message.</value>
    [DataMember(Order = 2)]
    public string Message { get; set; }

    /// <summary>
    /// The Server StackTrace when DebugMode is enabled
    /// </summary>
    /// <value>The stack trace.</value>
    [DataMember(Order = 3)]
    public string StackTrace { get; set; }

    /// <summary>
    /// For multiple detailed validation errors.
    /// Can hold a specific error message for each named field.
    /// </summary>
    /// <value>The errors.</value>
    [DataMember(Order = 4)]
    public List<ResponseError> Errors { get; set; }

    /// <summary>
    /// For additional custom metadata about the error
    /// </summary>
    /// <value>The meta.</value>
    [DataMember(Order = 5)]
    public Dictionary<string, string> Meta { get; set; }
}