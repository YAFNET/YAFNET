// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Intelligencia" file="Base64Transform.cs">
//   Copyright (c)2011 Seth Yates
//   //   Author Seth Yates
//   //   Author Stewart Rae
// </copyright>
// <summary>
//   Forked Version for YAF.NET
//   Original can be found at https://github.com/sethyates/urlrewriter
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YAF.UrlRewriter.Transforms;

using System;
using System.Text;

using YAF.UrlRewriter.Utilities;

/// <summary>
/// Base 64 encodes the input.
/// </summary>
public sealed class Base64DecodeTransform : IRewriteTransform
{
    /// <summary>
    /// Applies a transformation to the input string.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The transformed string.</returns>
    public string ApplyTransform(string input)
    {
        return Encoding.UTF8.GetString(Convert.FromBase64String(input));
    }

    /// <summary>
    /// The name of the action.
    /// </summary>
    public string Name => Constants.TransformBase64;
}