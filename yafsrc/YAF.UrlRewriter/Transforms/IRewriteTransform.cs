// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Intelligencia" file="IRewriteTransform.cs">
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

/// <summary>
/// Interface for transforming replacements.
/// </summary>
public interface IRewriteTransform
{
    /// <summary>
    /// Gets the name of the transform.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Applies a transformation to the input string.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The transformed string.</returns>
    string ApplyTransform(string input);
}