// ***********************************************************************
// <copyright file="SqlMapper.LiteralToken.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
namespace ServiceStack.OrmLite.Dapper;

using System;
using System.Collections.Generic;

/// <summary>
/// Class SqlMapper.
/// </summary>
public static partial class SqlMapper
{
    /// <summary>
    /// Represents a placeholder for a value that should be replaced as a literal value in the resulting sql
    /// </summary>
    internal struct LiteralToken
    {
        /// <summary>
        /// The text in the original command that should be replaced
        /// </summary>
        /// <value>The token.</value>
        public string Token { get; }

        /// <summary>
        /// The name of the member referred to by the token
        /// </summary>
        /// <value>The member.</value>
        public string Member { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LiteralToken"/> struct.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="member">The member.</param>
        internal LiteralToken(string token, string member)
        {
            Token = token;
            Member = member;
        }

        /// <summary>
        /// The none
        /// </summary>
        internal static readonly IList<LiteralToken> None = Array.Empty<LiteralToken>();
    }
}