// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2007 Intelligencia
// Copyright 2007 Seth Yates
// 

namespace Intelligencia.UrlRewriter.Transforms
{
    using System;
    using System.Text;

    using Intelligencia.UrlRewriter.Utilities;

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
}