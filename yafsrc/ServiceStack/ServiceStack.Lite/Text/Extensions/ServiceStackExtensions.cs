// ***********************************************************************
// <copyright file="ServiceStackExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

#if !NETCORE
namespace ServiceStack.Text.Extensions
{
    using System;

    /// <summary>
    /// Move conflicting extension methods into 'ServiceStack.Extensions' namespace
    /// </summary>
    public static class ServiceStackExtensions
    {
        //Ambiguous definitions in .NET Core 3.0 System MemoryExtensions.cs 
        /// <summary>
        /// Trims the specified span.
        /// </summary>
        /// <param name="span">The span.</param>
        /// <returns>ReadOnlyMemory&lt;System.Char&gt;.</returns>
        public static ReadOnlyMemory<char> Trim(this ReadOnlyMemory<char> span)
        {
            return span.TrimStart().TrimEnd();
        }

        /// <summary>
        /// Trims the start.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>ReadOnlyMemory&lt;System.Char&gt;.</returns>
        public static ReadOnlyMemory<char> TrimStart(this ReadOnlyMemory<char> value)
        {
            if (value.IsEmpty) return TypeConstants.NullStringMemory;
            var span = value.Span;
            int start = 0;
            for (; start < span.Length; start++)
            {
                if (!char.IsWhiteSpace(span[start]))
                    break;
            }
            return value.Slice(start);
        }

        /// <summary>
        /// Trims the end.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>ReadOnlyMemory&lt;System.Char&gt;.</returns>
        public static ReadOnlyMemory<char> TrimEnd(this ReadOnlyMemory<char> value)
        {
            if (value.IsEmpty) return TypeConstants.NullStringMemory;
            var span = value.Span;
            int end = span.Length - 1;
            for (; end >= 0; end--)
            {
                if (!char.IsWhiteSpace(span[end]))
                    break;
            }
            return value.Slice(0, end + 1);
        }
    }
}

#endif