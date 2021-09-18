// ***********************************************************************
// <copyright file="ScriptTypes.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
namespace ServiceStack.Script
{
    /// <summary>
    /// Class RawString.
    /// Implements the <see cref="ServiceStack.IRawString" />
    /// </summary>
    /// <seealso cref="ServiceStack.IRawString" />
    public class RawString : IRawString
    {
        /// <summary>
        /// The empty
        /// </summary>
        public static RawString Empty = new RawString("");

        /// <summary>
        /// The value
        /// </summary>
        private readonly string value;
        /// <summary>
        /// Initializes a new instance of the <see cref="RawString"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public RawString(string value) => this.value = value;
        /// <summary>
        /// Converts to rawstring.
        /// </summary>
        /// <returns>System.String.</returns>
        public string ToRawString() => value;
    }
}