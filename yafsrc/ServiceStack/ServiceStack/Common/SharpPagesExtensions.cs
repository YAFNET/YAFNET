// ***********************************************************************
// <copyright file="SharpPagesExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System.Threading.Tasks;
using ServiceStack.Text;
using ServiceStack.Web;

namespace ServiceStack;

/// <summary>
/// Class SharpPagesExtensions.
/// </summary>
public static class SharpPagesExtensions
{
    /// <summary>
    /// Render to string as an asynchronous operation.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <returns>A Task&lt;System.String&gt; representing the asynchronous operation.</returns>
    public static async Task<string> RenderToStringAsync(this IStreamWriterAsync writer)
    {
        using var ms = MemoryStreamFactory.GetStream();
        await writer.WriteToAsync(ms);
        return await ms.ReadToEndAsync();
    }
}