// ***********************************************************************
// <copyright file="StopExecutionException.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;

namespace ServiceStack
{
    /// <summary>
    /// Class StopExecutionException.
    /// Implements the <see cref="System.Exception" />
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class StopExecutionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StopExecutionException"/> class.
        /// </summary>
        public StopExecutionException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StopExecutionException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public StopExecutionException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StopExecutionException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<see langword="Nothing" /> in Visual Basic) if no inner exception is specified.</param>
        public StopExecutionException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}