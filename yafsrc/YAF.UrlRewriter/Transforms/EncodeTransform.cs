// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Intelligencia" file="EncodeTransform.cs">
//   Copyright (c)2011 Seth Yates
//   //   Author Seth Yates
//   //   Author Stewart Rae
// </copyright>
// <summary>
//   Forked Version for YAF.NET
//   Original can be found at https://github.com/sethyates/urlrewriter
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YAF.UrlRewriter.Transforms
{
    using System.Web;

    using YAF.UrlRewriter.Utilities;

    /// <summary>
    /// Url encodes the input.
    /// </summary>
    public sealed class EncodeTransform : IRewriteTransform
    {
        /// <summary>
        /// Applies a transformation to the input string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The transformed string.</returns>
        public string ApplyTransform(string input)
        {
            return HttpUtility.UrlEncode(input);
        }

        /// <summary>
        /// The name of the action.
        /// </summary>
        public string Name => Constants.TransformEncode;
    }
}
